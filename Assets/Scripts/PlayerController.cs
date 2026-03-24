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
            StartCoroutine(GameManager.Instance.StartGame());
            MeatheadController = this;
        }
    }

    

    
}
