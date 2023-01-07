using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleFluctuator : MonoBehaviour
{
    // La taille minimale de l'objet
    public float minScale = 0.75f;
    // La taille maximale de l'objet
    public float maxScale = 1f;
    // La vitesse à laquelle l'objet doit fluctuer de taille
    public float speed = 1f;

    void Update()
    {
        // Calculer la nouvelle taille de l'objet en utilisant Mathf.PingPong
        float newScale = Mathf.PingPong(Time.time * speed, maxScale - minScale) + minScale;
        // Mettre à jour la taille de l'objet en utilisant la nouvelle valeur de scale
        transform.localScale = new Vector3(newScale, newScale, newScale);
    }
}
