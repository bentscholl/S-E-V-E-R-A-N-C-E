using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Door : MonoBehaviour
{
    Animator animator;
    int collisions;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<NavMeshAgent>())
        {
            animator.SetBool("Open", true);
            collisions++;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<NavMeshAgent>())
        {
            collisions--;
            if(collisions == 0)
                animator.SetBool("Open", false);
        }
    }
}
