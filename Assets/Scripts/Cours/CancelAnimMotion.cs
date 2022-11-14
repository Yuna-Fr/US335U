using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CancelAnimMotion : MonoBehaviour
{
    private Vector3 startPosition;

    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = startPosition;
    }
}
