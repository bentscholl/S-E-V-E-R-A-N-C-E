using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    bool credits;
    GameObject StartButton;

    private void Awake()
    {
        if (PlayerController.MeatheadController != null)
            Destroy(PlayerController.MeatheadController.gameObject);
        if (PlayerController.ComboverController != null)
            Destroy(PlayerController.ComboverController.gameObject);
    }
    private void Start()
    {
        credits = false;
        StartButton = GameObject.Find("Start");
    }

    private void Update()
    {
        if((Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0) && EventSystem.current.currentSelectedGameObject == null)
        {
            EventSystem.current.SetSelectedGameObject(StartButton);
        }
    }
    public void Begin()
    {
        GameManager.Level = 1;
        SceneManager.LoadScene(1);
    }

    public void End()
    {
        Application.Quit();
    }

    public void ToggleCredits() 
    {
        credits = !credits;
        GameObject.Find("CreditsNote").GetComponent<Animator>().SetBool("Toggle", credits);
    }
        
        
}
