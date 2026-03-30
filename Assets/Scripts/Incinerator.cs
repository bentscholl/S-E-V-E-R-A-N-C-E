using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Incinerator : MonoBehaviour
{
    Material mat;
    Collider Meathead;
    ParticleSystem ParticleSystem;
    private void Start()
    {
        mat = GetComponentInParent<Renderer>().material;
        ParticleSystem = GetComponentInChildren<ParticleSystem>();
    }

    private void FixedUpdate()
    {
        if (Meathead != null && !Meathead.enabled)
        {
            mat.color = Color.white;
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.name.Equals("Corpse"))
        {
            if (other.GetComponent<NPC>().IsRelocated)
            {
                GameManager.AudioSource.PlayOneShot(GameManager.Burn);
                ParticleSystem.Play();
                Destroy(other.gameObject);
            }
        }
        else if (other.name.Equals("WindowBox"))
        {
            Meathead = (Collider)other;
            if (other.enabled)
                mat.color = Color.yellow;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.name.Equals("WindowBox"))
        {
            mat.color = Color.white;
        }
    }
}
