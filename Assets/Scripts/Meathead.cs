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
    Material CorpseHighlight;

    BoxCollider WindowCollider;

    public new void Start()
    {
        StartOffset = new Vector3(0, 1, .5f);
        base.Start();
        Instance = this;
        WindowCollider = transform.GetChild(1).GetComponentInChildren<BoxCollider>();
        WindowCollider.enabled = false;
        MovementSpeed = 2;
        OtherPlayer = FindAnyObjectByType<Combover>();
        OtherPlayer.OtherPlayer = this;
        Hands = transform.GetChild(1).GetChild(1).gameObject;
        Hands.SetActive(false);
        PickUp = Resources.Load<AudioClip>("SFX/PickUp");
        PutDown = Resources.Load<AudioClip>("SFX/PutDown");
    }

    private new void FixedUpdate()
    {
        if (!IsCarrying)
            base.FixedUpdate();
        else
        {
            Corpse.FlipRight = FacingRight;
        }
        RaycastHit hit;
        Physics.Raycast(transform.position, -SpriteTransform.right, out hit, 1f, RaycastMask | LayerMask.NameToLayer("Dead"));
        if (!IsCarrying && IsKiller && hit.collider && hit.collider.name.Contains("Corpse"))
        {
            Material newMat = hit.collider.GetComponent<NPC>().MHSpriteRenderer.material;
            if (CorpseHighlight != null && CorpseHighlight != newMat)
                CorpseHighlight.SetInt("_Highlight", 0);
            CorpseHighlight = newMat;
            CorpseHighlight.SetColor("_Color", Color.yellow);
            CorpseHighlight.SetInt("_Highlight", 1);
        }
        else if(CorpseHighlight != null)
        {
            CorpseHighlight.SetInt("_Highlight", 0);
            CorpseHighlight = null;
        }
    }

    protected override SpriteRenderer GetNPCRenderer(NPC npc)
    {
        return npc.MHSpriteRenderer;
    }

    public new void OnWest(InputValue value)
    {
        if(!IsCarrying)
        {
            base.OnWest(value);
        }
    }    

    public new void OnSouth(InputValue value)
    {
        base.OnSouth(value);
        if (!IsCarrying && IsKiller && enabled)
        {
            RaycastHit hit;
            Physics.Raycast(transform.position, -SpriteTransform.right, out hit, 1, RaycastMask | LayerMask.NameToLayer("Dead"));
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
                WindowCollider.enabled = true;
                if (CorpseHighlight != null)
                {
                    CorpseHighlight.SetInt("_Highlight", 0);
                    CorpseHighlight = null;
                }
            }
        }
        else if(IsCarrying)
        {
            GameManager.AudioSource.PlayOneShot(PickUp);
            CarriedTransform.GetComponent<NPC>().IsRelocated = true;
            CarriedTransform.localPosition = new Vector3(-SpriteTransform.right.x * .2f, 0, -.05f);
            Corpse.COSpriteRenderer.enabled = false;
            Corpse.MHSpriteRenderer.enabled = false;
            Corpse.SortingGroup.enabled = true;
            CarriedTransform.parent = null;
            Corpse = null;
            CarriedTransform = null;
            IsCarrying = false;
            WindowCollider.enabled = false;
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
