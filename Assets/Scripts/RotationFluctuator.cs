using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationFluctuator : MonoBehaviour
{
    // L'angle minimal de rotation de l'objet
    public float minAngle = 0f;
    // L'angle maximal de rotation de l'objet
    public float maxAngle = 360f;
    // La vitesse à laquelle l'objet doit fluctuer de rotation
    public float speed = 1f;

    // La rotation initiale de l'objet
    private Quaternion initialRotation;

    void Start()
    {
        // Stocke la rotation initiale de l'objet
        initialRotation = transform.rotation;
    }

    void Update()
    {
        // Calculer l'angle de rotation en utilisant Mathf.PingPong
        float newAngle = Mathf.PingPong(Time.time * speed, maxAngle - minAngle) + minAngle;
        // Mettre à jour la rotation de l'objet en utilisant la nouvelle valeur d'angle et la rotation initiale de l'objet
        transform.rotation = initialRotation * Quaternion.Euler(0f, newAngle, 0f);
    }
}
