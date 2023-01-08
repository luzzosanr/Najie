using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Character : BaseCharacter 
{
    [Header("Disruption")]
    public GameObject[] disruptions;
    GameObject lanterne;
    int clearedDisruptions = 0;

    [Header("Plant")]
    public GameObject preview; // Preview of the tree

    [Header("Time")]
    public double timeUtilNextDisruption = 0f;
    float timeUtilWeedHealing = 5f;

    [Header("Beginning and end of the game")]
    public GameObject worm;
    public GameObject beginningCanvas;
    public GameObject endCanvas;
    int beginningOrEnd = 0; // 0 = beginning, 1 = playing, 2 = end

	// Use this for initialization
	new void Start ()
    {
        base.Start();
        this.lanterne = this.transform.Find("Main Camera").Find("Lanterne").gameObject;
	}
	
	// Update is called once per frame
	new void Update ()
    {
        base.Update();

        if (beginningOrEnd == 0)
        {
            float distanceFromWorm = (worm.transform.position - this.transform.position).magnitude;
            if (distanceFromWorm < 50f)
            {
                beginningCanvas.SetActive(true);
            }
            else
            {
                beginningCanvas.SetActive(false);
            }

            // If the player press return, start the game
            if (Input.GetKeyDown(KeyCode.Return))
            {
                StartGame();
            }

            return;
        }
        else if (beginningOrEnd == 2)
        {
            float distanceFromWorm = (worm.transform.position - this.transform.position).magnitude;
            if (distanceFromWorm < 50f)
            {
                endCanvas.SetActive(true);
            }
            else
            {
                endCanvas.SetActive(false);
            }

            return;
        }

        if (timeUtilNextDisruption <= 0)
        {
            GenerateDisruptionCountdown();
            SpawnRandomDisruption();
        }
        else
        {
            timeUtilNextDisruption -= Time.deltaTime;
        }

        // for each disruption
        foreach (GameObject disruption in this.disruptions)
        {
            if (disruption.activeSelf && disruption.GetComponent<Disruption>().type == "tree")
            {
                this.TreePlantationUpdate(disruption);
            }
            else if (disruption.activeSelf && disruption.GetComponent<Disruption>().type == "sunflower")
            {
                this.SunflowerPlantationUpdate(disruption);
            }
            else if (disruption.activeSelf && disruption.GetComponent<Disruption>().type == "weeds")
            {
                this.WeedHealingUpdate(disruption);
            }
        }
    }

    void StartGame()
    {
        beginningCanvas.SetActive(false);
        worm.SetActive(false);
        timeUtilNextDisruption = 0f;
        beginningOrEnd = 1;
    }

    void SpawnRandomDisruption()
    {
        // Get all unactive disruptions
        List<GameObject> disruptions = new List<GameObject>();
        foreach (GameObject d in this.disruptions)
        {
            if (!d.activeSelf)
            {
                disruptions.Add(d);
            }
        }

        // Log if no disruptions available, maybe add a penalty
        if (disruptions.Count == 0)
        {
            Debug.Log("No more disruption available");
            return;
        }
        
        // Get a random disruption
        int index = UnityEngine.Random.Range(0, disruptions.Count);
        GameObject disruption = disruptions[index];

        // Spawn it
        disruption.SetActive(true);
    }

    void TreePlantationUpdate(GameObject disruption)
    {
        // Get position of the cursor in the world on the ground with raycast
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // Get object where ray hits
        Physics.Raycast(ray, out RaycastHit hit);
        bool canPlant = false;

        if (hit.transform != null && hit.transform.tag == "Ground")
        {
            canPlant = disruption.GetComponent<Disruption>().IsInSaveZone(hit.transform.InverseTransformPoint(hit.point));
        }

        if (canPlant)
        {
            // Display a preview
            preview.transform.position = hit.point;
            preview.SetActive(true);
            if (Input.GetKeyDown(KeyCode.I))
            {
                // Plant a tree
                disruption.GetComponent<Disruption>().PlantTree(preview.transform.localPosition);
            }
        }
        else if (preview.activeSelf)
        {
            // Hide preview
            preview.SetActive(false);
        }
    }

    void SunflowerPlantationUpdate(GameObject disruption)
    {
        GameObject GetNearestSunflower(GameObject discruption, float max = float.MaxValue)
        {
            /**
            * Get the position of the nearest sunflower or null if too far
            */

            GameObject[] sunflowers = GameObject.FindGameObjectsWithTag("Sunflower");
            GameObject nearestSunflower = null;
            float nearestDistance = max;
            foreach (GameObject sunflower in sunflowers)
            {
                Vector3 difference = sunflower.transform.position - transform.position;
                difference.y = 0;
                float distance = difference.magnitude;
                if (distance < nearestDistance && sunflower.transform.Find("Tournesol_pas_ok").gameObject.activeSelf && disruption.GetComponent<Disruption>().IsInSaveZone(sunflower.transform.localPosition + sunflower.transform.parent.localPosition))
                {
                    nearestSunflower = sunflower;
                    nearestDistance = distance;
                }
            }

            return nearestSunflower;
        }

        // Set fog unity fog on
        if (disruption.GetComponent<Disruption>().IsInZone(transform.localPosition))
        {
            RenderSettings.fog = true;
        }
        else if (RenderSettings.fog)
        {
            RenderSettings.fog = false;
        }

        // True if the player can heal a sunflower
        bool canHeal = disruption.GetComponent<Disruption>().IsInSaveZone(this.transform.localPosition);

        if (canHeal)
        {
            // Display the lamp
            this.lanterne.SetActive(true);
            // Heal the nearest sunflower
            GameObject sunflower = GetNearestSunflower(disruption, 60f);
            Healing compo = sunflower == null ? null : sunflower.GetComponent<Healing>();
            if (sunflower != null && compo == null)
            {
                sunflower.AddComponent<Healing>();
            }
            else if (sunflower != null)
            {
                compo.Progress(Time.deltaTime);
                if (compo.IsHealed())
                {
                    Destroy(compo);
                    disruption.GetComponent<Disruption>().ReduceRadius("Sunflower");
                }
            }
        }
        else if (this.lanterne.activeSelf)
        {
            // Hide the lamp
            this.lanterne.SetActive(false);
        }
    }

    void WeedHealingUpdate(GameObject disruption)
    {
        bool canHeal = disruption.GetComponent<Disruption>().IsInSaveZone(this.transform.localPosition);

        if (canHeal)
        {
            this.timeUtilWeedHealing -= Time.deltaTime;
        }

        if (this.timeUtilWeedHealing <= 0)
        {
            this.timeUtilWeedHealing = 5f;
            disruption.GetComponent<Disruption>().ReduceRadius("Weed");
        }
    }

    public void SetLanterneOff()
    {
        this.lanterne.SetActive(false);
    }

    void GenerateDisruptionCountdown()
    {
        /**
        * Generate a disruption timer with a random time
        * Normal distribution
        * Sigma = 3
        * Mean = 2
        * Reshuffle if the time is below 0
        */
        
        double time = 0;
        double mean = 2;
        double stdDev = 1.5;
        while (time <= 0)
        {
            System.Random rand = new System.Random();
            double u1 = 1.0-rand.NextDouble(); //uniform(0,1] random doubles
            double u2 = 1.0-rand.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
            time = mean + stdDev * randStdNormal; //random normal(mean,stdDev^2)
        }
        this.timeUtilNextDisruption = time * 60;
    }

    public void ClearDisruption()
    {
        this.clearedDisruptions += 1;
        if (this.clearedDisruptions >= 6)
        {
            foreach (GameObject d in this.disruptions)
            {
                d.SetActive(false);
            }
            this.beginningOrEnd = 2;
            this.worm.SetActive(true);
        }
    }
}