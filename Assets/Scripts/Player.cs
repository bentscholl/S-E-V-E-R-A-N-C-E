using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using static UnityEngine.InputSystem.InputAction;

public abstract class Player : MonoBehaviour
{
    protected bool FacingRight;
    public Vector2 MovementVector;
    public float MovementSpeed;
    public PlayerInput input;
    protected Transform SpriteTransform;
    private BoxCollider StabCollider;

    public Player OtherPlayer;
    public bool Swapping;
    public bool SwapOnCooldown;

    public bool IsKiller;
    public Animator Animator;
    ParticleSystem ParticleSystem;

    protected NavMeshAgent MeshAgent;

    public static Transform KillerTransform;
    private bool StabReady;
    protected bool Stabbing;

    AudioClip StabFX;
    AudioClip InteractFX;
    AudioClip PoofFX;

    public AudioSource AudioSource;

    protected Vector3 StartOffset;

    public NPC FollowingNPC;
    public Material NPCHighlight;

    public LayerMask RaycastMask;

    // Start is called before the first frame update
    protected void Start()
    {
        input = GetComponent<PlayerInput>();
        MovementVector = Vector2.zero;
        SpriteTransform = GetComponentInChildren<SpriteRenderer>().transform;
        Animator = GetComponent<Animator>();
        ParticleSystem = GetComponent<ParticleSystem>();
        MeshAgent = GetComponent<NavMeshAgent>();
        SpriteTransform.eulerAngles = new Vector3(0, 180, 0);
        StabReady = true;

        AudioSource = gameObject.AddComponent<AudioSource>();
        AudioSource.outputAudioMixerGroup = Resources.Load<AudioMixer>("AudioMixer").FindMatchingGroups("Master")[0];
        StabFX = Resources.Load<AudioClip>("SFX/Knife");
        PoofFX = Resources.Load<AudioClip>("SFX/Poof");
        NewLevel();
    }

    // Update is called once per frame
    void Update()
    {
        if (!Stabbing)
        {
            transform.Translate(MovementSpeed * Time.deltaTime * new Vector3(MovementVector.x, 0, MovementVector.y).normalized);

            if (MovementVector.x > 0)
            {
                SpriteTransform.eulerAngles = new Vector3(0, 180, 0);
                FacingRight = true;
            }
            else if (MovementVector.x < 0)
            {
                SpriteTransform.eulerAngles = Vector3.zero;
                FacingRight = false;
            }
        } 
    }

    protected void FixedUpdate()
    {
        if (FollowingNPC == null)
        {
            RaycastHit hit;
            Physics.Raycast(transform.position, -SpriteTransform.right, out hit, 1.5f, RaycastMask);
            bool seenNPC = false;

            if (hit.collider != null && hit.collider.GetComponent<NPC>() != null)
            {
                NPC npc = hit.collider.GetComponent<NPC>();
                if (npc != null && !npc.IsDead)
                {
                    seenNPC = true;
                    SpriteRenderer renderer = GetNPCRenderer(npc);
                    if (NPCHighlight != null && NPCHighlight != renderer.GetComponent<Renderer>().material)
                        NPCHighlight.SetInt("_Highlight", 0);

                    NPCHighlight = renderer.GetComponent<Renderer>().material;
                    if (!IsKiller && npc.Behavior != NPC.FiniteState.Escape && npc.Behavior != NPC.FiniteState.Investigate)
                    {
                        NPCHighlight.SetColor("_Color", Color.yellow);
                        NPCHighlight.SetInt("_Highlight", 1);
                    }
                    else if (IsKiller && StabReady && !npc.IsDead)
                    {
                        NPCHighlight.SetColor("_Color", Color.red);
                        NPCHighlight.SetInt("_Highlight", 1);
                    }
                }
            }

            if (NPCHighlight != null && !seenNPC)
            {
                NPCHighlight.SetInt("_Highlight", 0);
                NPCHighlight = null;
            }
        }
    }

    protected abstract SpriteRenderer GetNPCRenderer(NPC npc);

    public void NewLevel()
    {
        Transform Start = GameObject.Find("StartPoint").transform;
        MeshAgent.Warp(Start.position + StartOffset);
    }

    public void OnMove(InputValue value)
    {
        if (Time.timeScale > 0)
        {
            if (value.Get() != null)
            {
                MovementVector = (Vector2)value.Get();
                Animator.SetBool("Walking", true);
            }
            else
            {
                MovementVector = Vector2.zero;
                Animator.SetBool("Walking", false);
            }
        }
    }

    public void OnPause()
    {
        MovementVector = Vector2.zero;
        if (OtherPlayer != null)
        {
            OtherPlayer.MovementVector = Vector2.zero;
        }
        PauseManager.Instance.PauseButton();
    }

    /*
    public void OnEast(InputValue value)
    {
        print(name + " Yelled");
        if (!YellCollider.enabled)
            StartCoroutine(Yell());
    }*/

    public void OnWest(InputValue value)
    {
        if (IsKiller && enabled)
        {
            if (StabReady)
            {
                Animator.SetTrigger("Attack");
                print(name + " Attacked");
                StartCoroutine(Stab());
            }
        }
    }

    public void OnSouth(InputValue value)
    {
        if (!IsKiller && enabled)
        {
            if (FollowingNPC == null)
            {
                RaycastHit hit;
                Physics.Raycast(transform.position, -SpriteTransform.right, out hit, 1, RaycastMask);
                if (hit.collider)
                {
                    NPC npc = hit.collider.GetComponent<NPC>();
                    if (npc != null && !npc.IsDead && npc.Behavior != NPC.FiniteState.Escape)
                    {
                        npc.Follow(transform);
                        FollowingNPC = npc;
                    }
                }
            }
            else
            {
                FollowingNPC.Dismiss();
                FollowingNPC = null;
            }
        }
    }

    public IEnumerator Stab()
    {
        StabReady = false;
        Stabbing = true;
        RaycastHit hit;
        Physics.Raycast(transform.position, -SpriteTransform.right, out hit, 1.5f, RaycastMask);
        if(hit.collider != null &&  hit.collider.GetComponent<NPC>() != null)
        {
            hit.collider.GetComponent<NPC>().Stab();
        }
        AudioSource.PlayOneShot(StabFX);
        yield return new WaitForSeconds(.6f);
        Stabbing = false;
        yield return new WaitForSeconds(1f);
        StabReady = true;
    }

    public void OnNorth(InputValue value)
    {
        if (OtherPlayer != null && !SwapOnCooldown && enabled)
        {
            Swapping = value.isPressed;
            if (OtherPlayer.Swapping && Swapping && Vector3.Distance(transform.position, OtherPlayer.transform.position) < 2)
            {
                AudioSource.PlayOneShot(PoofFX);
                Swap();
                OtherPlayer.Swap();
            }
        }
    }

    public void Swap()
    {
        Swapping = false;
        IsKiller = !IsKiller;
        if(IsKiller)
        {
            Animator.SetTrigger("Killer");
            KillerTransform = transform;
        }
        else
        {
            Animator.SetTrigger("Bystander");
        }
        SwapOnCooldown = true;
        ParticleSystem.Play();
        Invoke("SwapCooldownDone", 1.5f);

        if(FollowingNPC != null)
        {
            FollowingNPC.Dismiss();
            FollowingNPC = null;
        }

        if(OtherPlayer.FollowingNPC != null)
        {
            OtherPlayer.FollowingNPC.Dismiss();
            OtherPlayer.FollowingNPC = null;
        }
    }

    public void SwapCooldownDone()
    {
        SwapOnCooldown = false;
    }

    
}
