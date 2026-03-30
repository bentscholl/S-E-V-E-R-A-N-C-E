using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    bool credits;
    bool select;
    int UnlockedLevels;
    GameObject StartButton;
    GameObject DefaultElevator;
    Button[] MainButtons;

    private void Awake()
    {
        if (PlayerController.MeatheadController != null)
            Destroy(PlayerController.MeatheadController.gameObject);
        if (PlayerController.ComboverController != null)
            Destroy(PlayerController.ComboverController.gameObject);

        UnlockedLevels = PlayerPrefs.GetInt("Level");
        if (UnlockedLevels == 0)
            UnlockedLevels = 1;

        Button[] Floors = GameObject.Find("Floors").GetComponentsInChildren<Button>();

        for(int i = 0; i < UnlockedLevels; i++)
        {
            Floors[i].interactable = true;
            Floors[i].GetComponent<Image>().color = Color.white;
        }
    }
    private void Start()
    {
        credits = false;
        StartButton = GameObject.Find("Start");
        DefaultElevator = GameObject.Find("Floor 5");
        MainButtons = GameObject.Find("Main").GetComponentsInChildren<Button>();
    }

    private void Update()
    {
        if((Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0) && EventSystem.current.currentSelectedGameObject == null)
        {
            EventSystem.current.SetSelectedGameObject(EventSystem.current.firstSelectedGameObject);
        }
    }
    public void Select()
    {
        GameObject.Find("Elevator Panel").GetComponent<Animator>().SetBool("Toggle", true);
        EventSystem.current.firstSelectedGameObject = DefaultElevator;
        EventSystem.current.SetSelectedGameObject(DefaultElevator);

        foreach (Button button in MainButtons)
        {
            button.interactable = false;
        }

        if(credits)
        {
            ToggleCredits();
        }
    }

    public void Back()
    {
        GameObject.Find("Elevator Panel").GetComponent<Animator>().SetBool("Toggle", false);
        EventSystem.current.firstSelectedGameObject = StartButton;
        EventSystem.current.SetSelectedGameObject(StartButton);

        foreach (Button button in MainButtons)
        {
            button.interactable = true;
        }
    }

    public void Load(int Level)
    {
        if(PlayerPrefs.GetInt("Cutscene1Played") == 0)
        {
            PlayerPrefs.SetInt("Cutscene1Played", 1);
            Transition.Instance.FadeToScene("Cutscene1");
            return;
        }
        GameManager.Level = Level;
        Transition.Instance.FadeToScene(Level);
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

    public void Reset()
    {
        PlayerPrefs.SetInt("Level", 1);
        PlayerPrefs.SetInt("Cutscene1Played", 0);
        Button[] Floors = GameObject.Find("Floors").GetComponentsInChildren<Button>();

        for (int i = 1; i < Floors.Length; i++)
        {
            Floors[i].interactable = false;
            Floors[i].GetComponent<Image>().color = Color.gray;
        }
    }


}
