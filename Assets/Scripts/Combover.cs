using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock.LowLevel;

public class Combover : Player
{
    public static Combover Instance;
    public bool IsVenting;
    bool VentMoveReset;
    bool LastVentMoveRight;
    Vent Vent;
    Material VentMat;
    int VentIndex;
    public GameObject Sprite;
    BoxCollider BoxCollider;

    bool panning;
    Camera Camera;

    AudioClip VentFX;
    AudioClip Woosh;
    private new void Start()
    {
        StartOffset = new Vector3(0, 1, -.5f);
        base.Start();
        Instance = this;
        MovementSpeed = 3f;
        Sprite = transform.GetChild(1).gameObject;
        Camera = GetComponentInChildren<Camera>();
        BoxCollider = GetComponent<BoxCollider>();
        VentFX = Resources.Load<AudioClip>("SFX/Vent");
        Woosh = Resources.Load<AudioClip>("Woosh");
        this.enabled = false;
    }

    public new void OnMove(InputValue value)
    {
        if (!IsVenting)
        {
            base.OnMove(value);
        }
        else
        {
            if(value.Get() != null)
            {
                Vector2 vec = (Vector2)value.Get();
                if (vec.x > 0 && (VentMoveReset || !LastVentMoveRight))
                {
                    if(VentIndex < Vent.Vents.Count-1)
                    {
                        Vent.Select(++VentIndex);
                    }
                    else
                    {
                        VentIndex = 0;
                        Vent.Select(VentIndex);
                    }
                    LastVentMoveRight = true;
                    VentMoveReset = false;
                }
                else if (vec.x < 0 && (VentMoveReset || LastVentMoveRight))
                {
                    if (VentIndex > 0)
                    {
                        Vent.Select(--VentIndex);
                    }
                    else
                    {
                        VentIndex = Vent.Vents.Count-1;
                        Vent.Select(VentIndex);
                    }
                        LastVentMoveRight = false;
                    VentMoveReset = false;
                }
             }
            else
            {
                VentMoveReset = true;
            }
        }
        
    }

    private new void FixedUpdate()
    {
        base.FixedUpdate();
        bool VentSpotted = false;
        if (IsKiller && !IsVenting)
        {
            RaycastHit hit;
            Physics.Raycast(transform.position, -SpriteTransform.right, out hit, 1);
            if (hit.collider && hit.collider.name.Contains("Vent"))
            {
                VentSpotted = true;
                Vent Vent = hit.collider.GetComponent<Vent>();
                VentMat = Vent.Material;
                VentMat.color = Color.yellow;
            }
        }

        if(VentMat != null && !VentSpotted)
        {
            VentMat.color = Color.white;
            VentMat = null;
        }
    }

    protected override SpriteRenderer GetNPCRenderer(NPC npc)
    {
        return npc.COSpriteRenderer;
    }

    public new void OnWest(InputValue value)
    {
        if (!IsVenting)
        {
            base.OnWest(value);
        }
        else if (enabled)
        {
            MoveVents(VentIndex);
        }
    }

    public new void OnNorth(InputValue value)
    {
        if (!IsVenting)
        {
            base.OnNorth(value);
        }
    }

    public new void OnEast(InputValue value)
    {

    }

    public void MoveVents(int index)
    {
        if (!panning)
        {
            GameManager.AudioSource.PlayOneShot(Woosh);
            Vent.ToggleArrows(false);
            Vent = Vent.Vents[index];
            VentIndex = 0;
            Vent.ToggleArrows(true);
            panning = true;
            StartCoroutine(PanCamera());
        }
    }

    IEnumerator PanCamera()
    {
        Sprite.SetActive(false);
        Vector3 start;
        Vector3 end;
        start = Camera.transform.position;
        MeshAgent.Warp(Vent.transform.position + new Vector3(0, 1, 0));
        end = Camera.transform.position;
        for(int i = 0; i <= 50; i++)
        {
            Camera.transform.position = Vector3.Lerp(start, end, i/50f);
            yield return new WaitForFixedUpdate();
        }
        panning = false;
    }



    public new void OnSouth(InputValue value)
    {
        base.OnSouth(value);
        if (!Stabbing && enabled)
        {
            if (!IsVenting && IsKiller)
            {
                RaycastHit hit;
                Physics.Raycast(transform.position, -SpriteTransform.right, out hit, 1);
                if (hit.collider && hit.collider.name.Contains("Vent"))
                {
                    GameManager.AudioSource.PlayOneShot(VentFX);
                    Vent = hit.collider.GetComponent<Vent>();
                    MeshAgent.Warp(Vent.transform.position + new Vector3(0, 1, 0));
                    IsVenting = true;
                    MovementVector = Vector2.zero;
                    Animator.SetBool("Venting", true);
                    Animator.SetBool("Walking", false);
                    BoxCollider.enabled = false;
                    VentIndex = 0;
                    Vent.ToggleArrows(true);
                    Vent.Animator.SetTrigger("Open");
                }
            }
            else if (!panning && IsVenting)
            {
                GameManager.AudioSource.PlayOneShot(VentFX);
                Sprite.SetActive(true);
                Vent.ToggleArrows(false);
                Vent.Animator.SetTrigger("Open");
                Vent = null;
                Animator.SetBool("Venting", false);
                BoxCollider.enabled = true;
                IsVenting = false;
            }
        }
    }
}
