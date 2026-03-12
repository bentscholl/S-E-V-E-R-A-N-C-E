using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    PlayerInput input;
    public static PlayerController ComboverController;
    public static PlayerController MeatheadController;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        GameManager.AudioSource.PlayOneShot(GameManager.PlayerJoin);
        input = GetComponent<PlayerInput>();
        if (input.playerIndex == 0)
        {
            Image ComboverSprite = GameObject.Find("ComboverMenu").GetComponent<Image>();
            ComboverSprite.color = Color.white;
            Instantiate(Resources.Load("Combover"), transform);
            ComboverController = this;
        }
        else
        {
            Image MeatheadSprite = GameObject.Find("MeatheadMenu").GetComponent<Image>();
            MeatheadSprite.color = Color.white;
            GameObject MeatheadObject = (GameObject) Instantiate(Resources.Load("Meathead"), transform);
            MeatheadObject.GetComponent<Meathead>().Start();
            Meathead.Instance.enabled = false;
            StartCoroutine(StartGame());
            MeatheadController = this;
        }
    }

    private IEnumerator StartGame()
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

    
}
