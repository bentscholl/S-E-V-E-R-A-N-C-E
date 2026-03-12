using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Meathead : Player
{
    public static Meathead Instance;

    bool IsCarrying;
    NPC Corpse;
    Transform CarriedTransform;

    GameObject Hands;

    AudioClip PickUp;
    AudioClip PutDown;

    public new void Start()
    {
        StartOffset = new Vector3(0, 1, .5f);
        base.Start();
        Instance = this;
        MovementSpeed = 2;
        OtherPlayer = FindAnyObjectByType<Combover>();
        OtherPlayer.OtherPlayer = this;
        Hands = transform.GetChild(1).GetChild(1).gameObject;
        Hands.SetActive(false);
        PickUp = Resources.Load<AudioClip>("SFX/PickUp");
        PutDown = Resources.Load<AudioClip>("SFX/PutDown");
    }

    private void FixedUpdate()
    {
        if (IsCarrying)
        {
            Corpse.SpriteRenderer.flipX = FacingRight;
        }
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
                Corpse = CarriedTransform.GetComponent<NPC>();
                Corpse.SortingGroup.enabled = false;
                CarriedTransform.parent = transform;
                CarriedTransform.localPosition = new Vector3(0, 0.27f, 0);
                IsCarrying = true;
                Hands.SetActive(true);
                Animator.SetBool("Carry", true);
            }
        }
        else if(IsCarrying)
        {
            GameManager.AudioSource.PlayOneShot(PickUp);
            CarriedTransform.GetComponent<NPC>().IsRelocated = true;
            CarriedTransform.localPosition = new Vector3(-SpriteTransform.right.x * .2f, 0, -.05f);
            Corpse.SpriteRenderer.enabled = false;
            Corpse.SortingGroup.enabled = true;
            CarriedTransform.parent = null;
            Corpse = null;
            CarriedTransform = null;
            IsCarrying = false;
            Hands.SetActive(false);
            Animator.SetBool("Carry", false);
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
