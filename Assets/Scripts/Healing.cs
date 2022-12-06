using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healing : MonoBehaviour
{
    public float timeSinceBegining = 0f;

    public void Progress(float time)
    {
        timeSinceBegining += time;
    }
}