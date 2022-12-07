using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Disruption : MonoBehaviour
{
    /**
    * Empty GameObject that contains the disruption
    * Childs of this object are:
    * - Rift: Rift is the donut
    * - DisruptionSphere: Sphere that grows for visual
    * - VisualDisruption: Object that is displayed on the ground (For testing purpose)
    * - Zone: Zone where the disruption is active (not an object, defined by the radius), y isn't taken into account
    */

    [Header("Parameters")]
    public float sphereRadius;
    public float sphereExplosionSpeed;
    public float disruptionSpeed;
    public float plantableDistance; // Proportion of the radius where seeds are plantable
    public float reductionFactor; // Factor by which the radius is reduced when a tree is planted
    public float minTime; // TimeSinceBegining that if lower, disruption is destroyed
    public float maxTime; // TimeSinceBegining can't go over this value
    public float initTime; // TimeSinceBegining at the beginning of the disruption
    public string type;

    [Header("References")]
    public GameObject treePrefab;
    
    // Dinamic objects
    GameObject disruptionSphere;
    GameObject visualDisruption; // For testing purpose

    [Header("Variables")]
    public float timeSinceBegining;

    void Start()
    {
        this.disruptionSphere = this.transform.Find("Sphere").gameObject;
        
        this.CreateVisualDisruption();
    }

    void OnEnable()
    {
        this.timeSinceBegining = this.initTime;
    }

    void OnDisable()
    {
        // Code run whenever a player clears a disruption
    }

    void Update()
    {
        // Update time
        timeSinceBegining += Time.deltaTime;

        if (timeSinceBegining > maxTime)
        {
            timeSinceBegining = maxTime;
        }


        // Update sphere
        this.disruptionSphere.transform.localScale = new Vector3(sphereRadius, sphereRadius, sphereRadius) * 2 * (1 + timeSinceBegining * sphereExplosionSpeed);
        // Update visual disruption
        this.UpdateVisualDisruption();

        // Updates depending of type of disruption
        if (type == "tree")
        {
            this.TreesUpdate();
        }
        else if (type == "sunflower")
        {
            this.SunflowersUpdate();
        }

        // Destruction
        if (this.timeSinceBegining < minTime)
        {
            Destroy(this.gameObject);
        }
    }

    void TreesUpdate()
    {
        // Set all tree in range of disruption as dead
        GameObject[] trees = GameObject.FindGameObjectsWithTag("Tree");
        foreach (GameObject tree in trees)
        {
            Vector3 difference = tree.transform.position - transform.position;
            difference.y = 0;
            if (this.timeSinceBegining < minTime)
            {
                this.SetTreeAsAlive(tree);
            }
            else if (difference.magnitude < timeSinceBegining * disruptionSpeed)
            {
                this.SetTreeAsDead(tree);
            }
        }
    }

    void SunflowersUpdate()
    {
        // Set all sunflower in range of disruption as dead
        GameObject[] sunflowers = GameObject.FindGameObjectsWithTag("Sunflower");
        foreach (GameObject sunflower in sunflowers)
        {
            Vector3 difference = sunflower.transform.position - transform.position;
            difference.y = 0;
            if (this.timeSinceBegining < minTime)
            {
                this.SetSunflowerAsAlive(sunflower);
            }
            else if (difference.magnitude < timeSinceBegining * disruptionSpeed)
            {
                this.SetSunflowerAsDead(sunflower);
            }
        }
    }

    void SetTreeAsDead(GameObject tree)
    {
        tree.transform.Find("arbres_vivants").gameObject.SetActive(false);
        tree.transform.Find("arbres_morts").gameObject.SetActive(true);
    }

    void SetTreeAsAlive(GameObject tree)
    {
        tree.transform.Find("arbres_morts").gameObject.SetActive(false);
        tree.transform.Find("arbres_vivants").gameObject.SetActive(true);
    }

    void SetSunflowerAsDead(GameObject sunflower)
    {
        sunflower.transform.Find("Tournesol_bisbis").gameObject.SetActive(false);
        sunflower.transform.Find("Tournesol_pas_ok").gameObject.SetActive(true);
    }

    void SetSunflowerAsAlive(GameObject sunflower)
    {
        sunflower.transform.Find("Tournesol_pas_ok").gameObject.SetActive(false);
        sunflower.transform.Find("Tournesol_bisbis").gameObject.SetActive(true);
    }

    public bool IsInSaveZone(Vector3 position)
    {
        // returns whether a tree can be planted for exemple on the given coordinates (in the save zone)
        Vector3 difference = position - transform.localPosition;
        difference.y = 0;
        float distance = difference.magnitude;
        return distance <= timeSinceBegining * disruptionSpeed && distance > timeSinceBegining * disruptionSpeed * (1 - plantableDistance);
    }

    public void PlantTree(Vector3 position)
    {
        GameObject growingTree = new GameObject("GrowingTree");
        growingTree.transform.parent = this.transform;
        growingTree.transform.localScale = new Vector3(1f, 1f, 1f) * 0.05f;
        Instantiate(this.treePrefab, new Vector3(0f, 0f, 0f), Quaternion.identity, growingTree.transform).transform.tag = "GrowingTree";
        growingTree.transform.localPosition = position;
        growingTree.AddComponent<Growing>().setParameters(this.transform.parent.gameObject, this.gameObject);
    }

    public void ReduceRadius(string tag = "Tree")
    {
        this.timeSinceBegining *= this.reductionFactor;
        
        // Set all tree not in range as alive
        GameObject[] objects = GameObject.FindGameObjectsWithTag(tag);
        foreach (GameObject obj in objects)
        {
            Vector3 difference = obj.transform.position - transform.position;
            difference.y = 0;
            if (difference.magnitude > timeSinceBegining * disruptionSpeed)
            {
                if (tag == "Tree")
                {
                    this.SetTreeAsAlive(obj);
                }
                else if (tag == "Sunflower")
                {
                    this.SetSunflowerAsAlive(obj);
                }
            }
        }
    }

    // Visual disruption for testing
    void CreateVisualDisruption()
    {
        // Create new cylinder object, parent it to the disruption and move it to the right position
        this.visualDisruption = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        Destroy(this.visualDisruption.GetComponent<CapsuleCollider>());
        this.visualDisruption.GetComponent<MeshRenderer>().material.color = Color.red;
        this.visualDisruption.transform.SetParent(transform);
        this.visualDisruption.transform.localPosition = new Vector3(0f, - this.transform.localPosition.y, 0f);
        this.visualDisruption.transform.localScale = new Vector3(0f, 1f, 0f);
    }

    void UpdateVisualDisruption()
    {
        // Visual disruption
        this.visualDisruption.transform.localScale = new Vector3(timeSinceBegining * disruptionSpeed * 2, 1f, timeSinceBegining * disruptionSpeed * 2);
    }

    public bool IsInZone(Vector3 position)
    {
        Vector3 difference = position - transform.localPosition;
        difference.y = 0;
        float distance = difference.magnitude;

        return distance <= timeSinceBegining * disruptionSpeed;
    }
}