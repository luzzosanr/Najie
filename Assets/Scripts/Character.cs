using UnityEngine;
using System.Collections;

public class Character : BaseCharacter 
{
    [Header("Disruption")]
    public GameObject disruption;
    GameObject lanterne;

    [Header("Plant")]
    public GameObject preview; // Preview of the tree

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

        if (this.disruption.activeSelf && this.disruption.GetComponent<Disruption>().type == "tree")
        {
            this.TreePlantationUpdate();
        }
        else if (this.disruption.activeSelf && this.disruption.GetComponent<Disruption>().type == "sunflower")
        {
            this.SunflowerPlantationUpdate();
        }
    }

    void TreePlantationUpdate()
    {
        // Get position of the cursor in the world on the ground with raycast
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // Get object where ray hits
        Physics.Raycast(ray, out RaycastHit hit);
        bool canPlant = false;

        if (hit.transform != null && hit.transform.tag == "Ground")
        {
            canPlant = this.disruption.GetComponent<Disruption>().IsInSaveZone(hit.transform.InverseTransformPoint(hit.point));
        }

        if (canPlant)
        {
            // Display a preview
            preview.transform.position = hit.point;
            preview.SetActive(true);
            if (Input.GetKeyDown(KeyCode.I))
            {
                // Plant a tree
                this.disruption.GetComponent<Disruption>().PlantTree(preview.transform.localPosition);
            }
        }
        else if (preview.activeSelf)
        {
            // Hide preview
            preview.SetActive(false);
        }
    }

    void SunflowerPlantationUpdate()
    {
        GameObject GetNearestSunflower(float max = float.MaxValue)
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
                if (distance < nearestDistance && sunflower.transform.Find("Tournesol_pas_ok").gameObject.activeSelf && this.disruption.GetComponent<Disruption>().IsInSaveZone(sunflower.transform.localPosition))
                {
                    nearestSunflower = sunflower;
                    nearestDistance = distance;
                }
            }

            return nearestSunflower;
        }

        // Set fog unity fog on
        if (this.disruption.GetComponent<Disruption>().IsInZone(this.transform.localPosition))
        {
            RenderSettings.fog = true;
        }
        else if (RenderSettings.fog)
        {
            RenderSettings.fog = false;
        }

        // True if the player can heal a sunflower
        bool canHeal = this.disruption.GetComponent<Disruption>().IsInSaveZone(this.transform.localPosition);

        if (canHeal)
        {
            // Display the lamp
            this.lanterne.SetActive(true);
            // Heal the nearest sunflower
            GameObject sunflower = GetNearestSunflower(60f);
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
                    this.disruption.GetComponent<Disruption>().ReduceRadius("Sunflower");
                }
            }
        }
        else if (this.lanterne.activeSelf)
        {
            // Hide the lamp
            this.lanterne.SetActive(false);
        }
    }
}