using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoToObjectRootMotion : MonoBehaviour
{
    public Transform target;
    public float Smoothing = 0.5f;

    void LateUpdate()
    {
        //Direction de l'objet
        Vector3 dirToTarget = (target.position - transform.position);
        dirToTarget.y = 0; //On reste sur le plan x / z (on tourne uniquement autour de l'axe up)

        //Calcul de la rotation pour se tourner vers l'objet
        Quaternion rot = Quaternion.FromToRotation(
            transform.forward,
            dirToTarget.normalized);

        //On appliquer la rotation doucement en fonction d'un paramètre de smoothing
        transform.rotation = Quaternion.Lerp(transform.rotation,rot * transform.rotation,
            1.0f - Mathf.Pow(Mathf.Clamp01(Smoothing), Time.deltaTime));
    }
}
