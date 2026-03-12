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
    Vent Vent;
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
        MovementSpeed = 2.5f;
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
    }

    public new void OnWest(InputValue value)
    {
        if (!IsVenting)
        {
            base.OnWest(value);
        }
        else if (Vent.Vents.Count >= 1)
        {
            MoveVents(0);
        }
    }

    public new void OnNorth(InputValue value)
    {
        if (!IsVenting)
        {
            base.OnNorth(value);
        }
        else if (Vent.Vents.Count >= 2 && value.isPressed)
        {
            MoveVents(1);
        }
    }

    public new void OnEast(InputValue value)
    {
        if (IsVenting && Vent.Vents.Count >= 3)
        {
            MoveVents(2);
        }
    }

    public void MoveVents(int index)
    {
        if (!panning)
        {
            GameManager.AudioSource.PlayOneShot(Woosh);
            Vent.ToggleArrows(false);
            Vent = Vent.Vents[index];
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



    public void OnSouth(InputValue value)
    {
        if (StabReady)
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
