using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healing : MonoBehaviour
{
    public float timeSinceBegining = 0f;
    public const float healingTime = 5f;

    public void Progress(float time)
    {
        timeSinceBegining += time;
    }

    public bool IsHealed()
    {
        return timeSinceBegining > healingTime;
    }
}