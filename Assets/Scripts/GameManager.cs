using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    string[] PlayerNames;

    public static AudioSource AudioSource;
    public static AudioClip PlayerJoin;
    public static AudioClip Countdown;
    public static AudioClip Die;
    public static AudioClip Fall;
    public static AudioClip Leave;

    TextMeshProUGUI Total;
    TextMeshProUGUI Kills;
    TextMeshProUGUI Lost;

    GameObject Canvas;
    GameObject EndPanel;
    Animator EndAnimator;
    TextMeshProUGUI MoneyCounter;

    public bool GameStarted;
    bool GameOver;
    int StartingTotal;
    public static int Money = 0;

    public static int Level;
    void Awake()
    {
        Level = SceneManager.GetActiveScene().buildIndex;
        Total = GameObject.Find("Total").GetComponentInChildren<TextMeshProUGUI>();
        Kills = GameObject.Find("Kills").GetComponentInChildren<TextMeshProUGUI>();
        Lost = GameObject.Find("Lost").GetComponentInChildren<TextMeshProUGUI>();
        StartingTotal = FindObjectsByType<NPC>(FindObjectsSortMode.InstanceID).Length;

        EndPanel = GameObject.Find("EndPanel");
        EndAnimator = EndPanel.GetComponent<Animator>();
        MoneyCounter = GameObject.Find("Money").GetComponent<TextMeshProUGUI>();

        AudioSource = gameObject.AddComponent<AudioSource>();
        Countdown = Resources.Load<AudioClip>("SFX/Countdown");
        PlayerJoin = Resources.Load<AudioClip>("SFX/Join");
        Fall = Resources.Load<AudioClip>("SFX/Fall");
        Die = Resources.Load<AudioClip>("SFX/Die");
        Leave = Resources.Load<AudioClip>("SFX/Leave");

        NPC.NPCsEscaped = 0;
        NPC.NPCsKilled = 0;

        Canvas = GameObject.Find("Menu");
    }

    private void Start()
    {
        Instance = this;
        if (PlayerController.ComboverController != null && PlayerController.MeatheadController != null)
        {
            Meathead.Instance.NewLevel();
            Combover.Instance.NewLevel();
            GameObject StartPanel = GameObject.Find("StartPanel");
            StartPanel.GetComponent<Animator>().SetTrigger("Start");
            StartPanel.GetComponent<Animator>().speed = 2;
            string prefix = "$";
            if (Money <= 0)
            {
                MoneyCounter.color = Color.red;
                prefix = "-$";
            }
            for (int i = 0; i <= 150; i++)
            {
                MoneyCounter.text = prefix + Money;
            }
            EndAnimator.SetTrigger("Rise");
        }
        Money = 0;
    }


    private void Update()
    {

    }

    void FixedUpdate()
    {
        int totalLeft = StartingTotal - NPC.NPCsEscaped - NPC.NPCsKilled;
        Total.text = totalLeft.ToString();
        Kills.text = NPC.NPCsKilled.ToString();
        Lost.text = NPC.NPCsEscaped.ToString();

        if(!GameOver && totalLeft == 0)
        {
            StartCoroutine(EndGame());
        }
        
    }

    public IEnumerator StartGame()
    {
        GameObject StartPanel = GameObject.Find("StartPanel");
        TextMeshProUGUI StartText = StartPanel.GetComponentInChildren<TextMeshProUGUI>();
        Animator animator = GameObject.Find("Mask").GetComponent<Animator>();
        int KillerIndex = Random.Range(0, 2);
        if (KillerIndex == 1)
        {
            animator.SetTrigger("Meat");
            Meathead.Instance.Swap();
        }
        else
        {
            animator.SetTrigger("Comb");
            Combover.Instance.Swap();
        }
        StartText.GetComponent<Animator>().enabled = false;
        StartText.enabled = true;
        yield return new WaitForSeconds(.2f);
        StartText.text = "3";
        GameManager.AudioSource.PlayOneShot(GameManager.Countdown);
        yield return new WaitForSeconds(1);
        StartText.text = "2";
        GameManager.AudioSource.PlayOneShot(GameManager.Countdown);
        yield return new WaitForSeconds(1);
        StartText.text = "1";
        GameManager.AudioSource.PlayOneShot(GameManager.Countdown);
        yield return new WaitForSeconds(1);
        StartText.text = "";
        yield return new WaitForSeconds(.7f);
        StartPanel.GetComponent<Animator>().SetTrigger("Start");
        Meathead.Instance.enabled = true;
        Combover.Instance.enabled = true;
    }

    IEnumerator EndGame()
    {
        PauseManager.Pauseable = false;
        GameOver = true;
        Time.timeScale = .2f;
        MoneyCounter.text = "";
        yield return new WaitForSeconds(.3f);
        EndAnimator.SetTrigger("Start");
        yield return new WaitForSeconds(.3f);
        Time.timeScale = 1;
        string prefix = "$";
        if (Money <= 0)
        {
            MoneyCounter.color = Color.red;
            prefix = "-$";
        }
        for(int i = 0; i <= 150; i++)
        {
            MoneyCounter.text = prefix + Mathf.Abs((int)Mathf.Lerp(0,Money,i/150f));
            yield return new WaitForFixedUpdate();
        }
        Time.timeScale = 1;
        yield return new WaitForSeconds(3);

        NPC[] RemainingNPCs = FindObjectsByType<NPC>(FindObjectsSortMode.InstanceID);

        foreach (NPC npc in RemainingNPCs)
        {
            Destroy(npc.gameObject);
        }

        if(Money < 0 )
        {
            SceneManager.LoadScene(Level);
        }
        else if(Level < 5)
        {
            Level++;
            PlayerPrefs.SetInt("Level", Level);
            SceneManager.LoadScene(Level);
        }
        else
        {
            //Change to credits scroll
            SceneManager.LoadScene(0);
        }
    }
}
