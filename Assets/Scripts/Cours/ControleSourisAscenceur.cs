using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControleSourisAscenceur : MonoBehaviour
{
    private Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            animator.SetTrigger("Monter");
            animator.ResetTrigger("Descendre");
        }

        if (Input.GetButtonDown("Fire2"))
        {
            animator.ResetTrigger("Monter");
            animator.SetTrigger("Descendre");
        }
    }
}
