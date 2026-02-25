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
        base.Start();
        Instance = this;
        MovementSpeed = 2.5f;
        MeshAgent.Warp(new Vector3(-15, 1, 46));
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
        else if (Vent.Vents.Count >= 2)
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
                Animator.SetBool("Walking", false);
                Sprite.SetActive(false);
                BoxCollider.enabled = false;
                Vent.ToggleArrows(true);
                Vent.Animator.SetTrigger("Open");
            }
        }
        else if (!panning && IsVenting)
        {
            GameManager.AudioSource.PlayOneShot(VentFX);
            Vent.ToggleArrows(false);
            Vent.Animator.SetTrigger("Open");
            Vent = null;
            Sprite.SetActive(true);
            BoxCollider.enabled = true;
            IsVenting = false;
        }
    }
}
