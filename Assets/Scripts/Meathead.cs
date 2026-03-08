using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Meathead : Player
{
    public static Meathead Instance;

    bool IsCarrying;
    SpriteRenderer CarriedSprite;
    Transform CarriedTransform;

    GameObject Corpse;

    AudioClip PickUp;
    AudioClip PutDown;
    public new void Start()
    {
        base.Start();
        Instance = this;
        MovementSpeed = 2;
        OtherPlayer = FindAnyObjectByType<Combover>();
        OtherPlayer.OtherPlayer = this;
        MeshAgent.Warp(new Vector3(-15, 1, 47));
        Corpse = transform.GetChild(1).GetChild(1).gameObject;
        Corpse.SetActive(false);
        PickUp = Resources.Load<AudioClip>("SFX/PickUp");
        PutDown = Resources.Load<AudioClip>("SFX/PutDown");
    }

    private void FixedUpdate()
    {
    }

    public new void OnWest(InputValue value)
    {
        if(!IsCarrying)
        {
            base.OnWest(value);
        }
    }    

    public void OnSouth(InputValue value)
    {
        if (!IsCarrying && IsKiller)
        {
            RaycastHit hit;
            Physics.Raycast(transform.position, -SpriteTransform.right, out hit, 1);
            if (hit.collider && hit.collider.name.Contains("Corpse"))
            {
                GameManager.AudioSource.PlayOneShot(PickUp);
                CarriedTransform = hit.collider.transform;
                CarriedTransform.GetComponent<NPC>().IsRelocated = false;
                CarriedSprite = CarriedTransform.GetComponentInChildren<SpriteRenderer>();
                CarriedSprite.enabled = false;
                CarriedTransform.parent = transform;
                CarriedTransform.localPosition = new Vector3(.05f, .2f, -.1f);
                IsCarrying = true;
                Corpse.SetActive(true);
            }
        }
        else if(IsCarrying)
        {
            GameManager.AudioSource.PlayOneShot(PickUp);
            CarriedTransform.GetComponent<NPC>().IsRelocated = true;
            CarriedTransform.localPosition = new Vector3(-SpriteTransform.right.x * .2f, 0, -.05f);
            CarriedTransform.parent = null;
            CarriedSprite = null;
            CarriedTransform = null;
            IsCarrying = false;
            Corpse.SetActive(false);
        }
    }

    public new void OnNorth(InputValue value)
    {
        if (!IsCarrying)
        {
            base.OnNorth(value);
        }
    }
}
