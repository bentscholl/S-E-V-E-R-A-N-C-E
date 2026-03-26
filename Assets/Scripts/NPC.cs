using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
using UnityEngine.U2D;
using Random = UnityEngine.Random;
public class NPC : MonoBehaviour
{
    NavMeshAgent Agent;

    public SpriteRenderer SpriteRenderer;
    public SortingGroup SortingGroup;
    Transform SpriteTransform;
    [HideInInspector]
    public bool FlipRight;
    Animator Animator;
    Sprite[] Sprites;
    public int SpriteIndex;

    public bool IsDead;
    [HideInInspector]
    public bool IsRelocated;
    SphereCollider DeathCall;
    SphereCollider CorpseRadius;
    ParticleSystem Splatter;
    static bool RecentDeath;

    public LayerMask BodySpotting;
    public LayerMask KillerSpotting;
    public enum FiniteState { Idle, Travel, Investigate, Escape, Follow, Dead };
    [SerializeField]
    public FiniteState Behavior;

    private Transform SpriteParent;
    private Transform Brows;
    private GameObject SuspiciousBrow;
    private GameObject ScaredBrow;

    private bool sawKiller;
    private bool Despawnable; //Added to fix the fact that setting agent destination is async, sometimes prematurely escaping
    Vector3 Escape;
    Room[] Rooms;

    [SerializeField]
    Room MyRoom;

    Transform FollowedPlayer;



    public static int NPCsEscaped;
    public static int NPCsKilled;
    [Header("Stats")]

    [SerializeField]
    private float Suspicion;
    [SerializeField]
    private float Patience;
    [SerializeField]
    private float Boredom;

    // Start is called before the first frame update
    void Start()
    {
        //SpriteAtlas Atlas = (SpriteAtlas)Resources.Load("SpriteAtlas/NPC" + Random.Range(1, 3));
        //Atlas.GetSprites(Sprites);
        Sprites = Resources.LoadAll<Sprite>("NPCs/NPC"+Random.Range(1,16));
        SpriteRenderer = transform.GetChild(0).GetChild(1).GetComponent<SpriteRenderer>();
        SortingGroup = GetComponent<SortingGroup>();
        Agent = GetComponent<NavMeshAgent>();
        Behavior = FiniteState.Idle;
        Animator = GetComponent<Animator>();

        CorpseRadius = transform.GetChild(2).GetComponent<SphereCollider>();
        DeathCall = transform.GetChild(1).GetComponent<SphereCollider>();
        Splatter = GetComponentInChildren<ParticleSystem>();
        Escape = GameObject.Find("Escape").transform.position;

        Rooms = GameObject.Find("Locales").GetComponentsInChildren<Room>();
        MyRoom = Rooms[Random.Range(0,Rooms.Length)];
        //Bathroom = GameObject.Find("Bathroom").GetComponent<Room>();

        while(MyRoom.Residents >= MyRoom.Capacity)
        {
            MyRoom = Rooms[Random.Range(0, Rooms.Length)];
        }
        MyRoom.Residents++;
        Vector3 location = MyRoom.transform.position + new Vector3(Random.Range(-MyRoom.XVariation, MyRoom.XVariation), 0, Random.Range(-MyRoom.ZVariation, MyRoom.ZVariation));
        Agent.SetDestination(location);
        NavMeshHit hit;
        NavMesh.SamplePosition(location, out hit, 10f, NavMesh.AllAreas);
        Agent.Warp(hit.position);

        Patience = Random.Range(10, 50);
        Boredom = 10000;

        SpriteParent = transform.GetChild(0);
        Brows = SpriteParent.transform.GetChild(0);
        SuspiciousBrow = Brows.GetChild(0).gameObject;
        ScaredBrow = Brows.GetChild(1).gameObject;
        SuspiciousBrow.SetActive(false);
        ScaredBrow.SetActive(false);
    }
    private void FixedUpdate()
    {
        try
        {
            if (IsRelocated)
                SpriteRenderer.enabled = true;

            SpriteRenderer.sprite = Sprites[SpriteIndex];

            if (!IsDead)
            {
                if (Agent.velocity.magnitude == 0)
                {
                    Animator.SetBool("Walking", false);
                }
                else
                {
                    Animator.SetBool("Walking", true);
                }

                if (Suspicion >= 100 && Behavior != FiniteState.Escape)
                {
                    MyRoom.Residents--;
                    Invoke("SetDespawnable", 1);
                    Agent.destination = Escape;
                    print(Escape);
                    FollowedPlayer = null;
                    Behavior = FiniteState.Escape;
                    SuspiciousBrow.SetActive(false);
                    ScaredBrow.SetActive(true);
                    Animator.SetTrigger("Run");
                    Agent.speed *= 2f;
                }

                if (Despawnable && Behavior == FiniteState.Escape && Agent.remainingDistance > 0 && Agent.remainingDistance < 1.1f)
                {
                    transform.position = Vector3.zero;
                    NPCsEscaped++;
                    GameManager.AudioSource.PlayOneShot(GameManager.Leave);
                    GameManager.Money -= 20000;
                    Destroy(this.gameObject);
                }
                else if (Behavior == FiniteState.Idle)
                {
                    Boredom += .06f;
                    if (Boredom >= Patience)
                    {
                        int i = Random.Range(0, 7);
                        if (i == 0)
                        {
                            MyRoom.Residents--;
                            MyRoom = Rooms[Random.Range(0, Rooms.Length)];
                            while (MyRoom.Residents >= MyRoom.Capacity)
                            {
                                MyRoom = Rooms[Random.Range(0, Rooms.Length)];
                            }
                            MyRoom.Residents++;
                            Vector3 location = MyRoom.transform.position + new Vector3(Random.Range(-MyRoom.XVariation, MyRoom.XVariation), 0, Random.Range(-MyRoom.ZVariation, MyRoom.ZVariation + 1));
                            Agent.SetDestination(location);
                            Boredom = 0;
                            Behavior = FiniteState.Travel;
                        }
                        else
                        {
                            Vector3 location = MyRoom.transform.position + new Vector3(Random.Range(-MyRoom.XVariation, MyRoom.XVariation), 0, Random.Range(-MyRoom.ZVariation, MyRoom.ZVariation + 1));
                            Agent.SetDestination(location);
                            Boredom = 0;
                        }
                        Behavior = FiniteState.Travel;
                    }
                }
                else if (Behavior == FiniteState.Investigate)
                {
                    if (Agent.remainingDistance < 1.5f)
                    {
                        Behavior = FiniteState.Travel;
                        Vector3 location = MyRoom.transform.position + new Vector3(Random.Range(-MyRoom.XVariation, MyRoom.XVariation), 0, Random.Range(-MyRoom.ZVariation, MyRoom.ZVariation + 1));
                        Agent.SetDestination(location);
                        Boredom = 0;
                        SuspiciousBrow.SetActive(false);
                    }
                }
                else if (Behavior == FiniteState.Travel)
                {
                    if (Agent.remainingDistance < 2)
                        Behavior = FiniteState.Idle;
                }
                else if(Behavior == FiniteState.Follow)
                {
                    Agent.SetDestination(FollowedPlayer.position);
                }    

                    //Sprite Housekeeping
                    SpriteParent.LookAt(SpriteParent.position - new Vector3(0, 0, -1));

                if (Agent.desiredVelocity.x > 0)
                {
                    FlipRight = true;
                    
                }
                else if (Agent.desiredVelocity.x < 0)
                {
                    FlipRight = false;
                    
                }

                if(FlipRight)
                {
                    SpriteParent.eulerAngles = new Vector3(0, 180, 0);
                }
                else
                {
                    SpriteParent.eulerAngles = Vector3.zero;
                }

                 Brows.localPosition = new Vector3(0, 0, -Brows.forward.z * .01f);

                //Check Surroundings
                if (Player.KillerTransform != null)
                {
                    float DistanceMeathead = Vector3.Distance(transform.position, Meathead.Instance.transform.position);
                    float DistanceCombover = Vector3.Distance(transform.position, Combover.Instance.transform.position);
                    sawKiller = false;
                    if (DistanceMeathead < 4.5f)
                    {
                        CheckSurroundings(Meathead.Instance.transform.position);
                    }
                    if (DistanceCombover < 4.5f)
                    {
                        CheckSurroundings(Combover.Instance.transform.position);
                    }
                    if (!sawKiller)
                    {
                        Suspicion -= .5f;
                        if (Suspicion < 0)
                        { Suspicion = 0; }
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.ToString());
        }
    }

    void CheckSurroundings(Vector3 position)
    {
        RaycastHit hit;
        Vector3 PlayerDir = position - transform.position;
        Physics.Raycast(transform.position, PlayerDir, out hit, 4, KillerSpotting);
        Player player;
        bool seesPlayer = PlayerDir.x <= 0 && !FlipRight || PlayerDir.x >= 0 && FlipRight;
        if (hit.collider != null && seesPlayer)
        {
            //print(hit.collider.name);
            hit.collider.TryGetComponent<Player>(out player);
            if (player != null)
            {
                if (player.IsKiller && Suspicion < 100)
                {
                    Suspicion += (4.5f - Vector3.Distance(transform.position, position))*2/5;
                    sawKiller = true;
                    if(Suspicion >= 100)
                    {
                        GameManager.Money -= 10000;
                    }
                    return;
                }
                else if(!player.IsKiller && Behavior == FiniteState.Idle)
                {
                    Boredom += .25f;
                }
            }
        }
    }
    // Update is called once per frame
    void Update()
    {

    }

    public void Follow(Transform p)
    {
        FollowedPlayer = p;
        Behavior = FiniteState.Follow;
    }

    public void Dismiss()
    {
        FollowedPlayer = null;
        Boredom = 0;
        Vector3 location = MyRoom.transform.position + new Vector3(Random.Range(-MyRoom.XVariation, MyRoom.XVariation), 0, Random.Range(-MyRoom.ZVariation, MyRoom.ZVariation + 1));
        Agent.SetDestination(location);
        Behavior = FiniteState.Travel;
    }

    public void Stab()
    {
        IsDead = true;
        Agent.enabled = false;
        gameObject.layer = LayerMask.NameToLayer("Dead");
        Animator.SetTrigger("Kill");
        Brows.gameObject.SetActive(false);
        if(FollowedPlayer != null)
        {
            FollowedPlayer.GetComponent<Player>().FollowingNPC = null;
        }    
        StartCoroutine(Die());
    }

    private void SetDespawnable()
    {
        Despawnable = true;
    }

    public IEnumerator Die()
    {
        MyRoom.Residents--;
        GameManager.Money += 35000;
        Splatter.Play();
        GameManager.AudioSource.PlayOneShot(GameManager.Die);
        name = "Corpse";
        yield return new WaitForSeconds(.3f);
        NPCsKilled++;
        DeathCall.enabled = true;
        CorpseRadius.enabled = true;
        RecentDeath = false;
        yield return new WaitForSeconds(.2f);
        DeathCall.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsDead)
        {
            if (Behavior != FiniteState.Escape)
            {
                if (other.name.Contains("DeathCry"))
                {
                    Agent.SetDestination(other.transform.position);
                    Behavior = FiniteState.Investigate;
                    SuspiciousBrow.SetActive(true);
                }

            }
            if (other.name.Contains("Stab") && !RecentDeath)
            {
                RecentDeath = true;
                Stab();
            }
        }

    }

    private void OnTriggerStay(Collider other)
    {
        if (!IsDead && Behavior != FiniteState.Escape && other.name.Contains("DeathRadius"))
        {
            RaycastHit hit;
            Vector3 Direction = other.transform.position - transform.position;
            bool seesBody = Direction.x <= 0 && !FlipRight || Direction.x >= 0 && FlipRight;
            if (seesBody)
            {
                Physics.Raycast(transform.position, Direction, out hit, 4, BodySpotting);
                if (hit.collider && hit.collider.gameObject.layer == LayerMask.NameToLayer("Dead"))
                {
                    Suspicion = 100;
                    GameManager.Money -= 10000;
                }
            }
        }
    }
}
