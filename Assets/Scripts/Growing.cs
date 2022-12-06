using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Growing : MonoBehaviour
{
    float growingTime = 10f;
    float timeSinceBegining = 0f;
    GameObject tree;
    GameObject disruption; // Refference to the disruption
    
    // Start is called before the first frame update
    void Start()
    {
        this.tree = this.FindChildByTag("GrowingTree");
    }

    // Update is called once per frame
    void Update()
    {
        timeSinceBegining += Time.deltaTime;

        this.transform.localScale = new Vector3(1, 1, 1) * timeSinceBegining / growingTime;

        if (timeSinceBegining > growingTime)
        {
            this.EndGrowing();
        }
    }

    GameObject FindChildByTag(string tag)
    {
        Transform[] children = this.gameObject.GetComponentsInChildren<Transform>();
        foreach (Transform child in children)
        {
            if (child.tag == tag)
            {
                return child.gameObject;
            }
        }
        return null;
    }

    void EndGrowing()
    {
        this.tree.transform.parent = null;
        this.tree.transform.tag = "Tree";
        this.disruption.GetComponent<Disruption>().ReduceRadius();
        Destroy(this.gameObject);
    }

    public void SetDisruption(GameObject disruption)
    {
        this.disruption = disruption;
    }
}
