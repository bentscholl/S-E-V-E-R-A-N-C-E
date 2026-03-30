using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ledge : MonoBehaviour
{
    Material mat;
    Collider Meathead;
    public Color Original;
    private void Start()
    {
        mat = GetComponentInParent<Renderer>().material;
        Original = mat.color;
    }

    private void FixedUpdate()
    {
        if (Meathead != null && !Meathead.enabled)
        {
            mat.color = Original;
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.name.Equals("Corpse"))
        {
            if (other.GetComponent<NPC>().IsRelocated)
            {
                GameManager.AudioSource.PlayOneShot(GameManager.Fall);
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
            mat.color = Original;
        }
    }
}
