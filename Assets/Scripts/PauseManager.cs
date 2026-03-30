using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance;
    public static bool IsPaused = false;
    public static bool Pauseable;
    Button[] PauseButtons;
    Animator OptionsMenu;
    GameObject OptionsBackButton;
    GameObject ResumeButton;
    Animator Animator;
    PlayerInputManager PlayerInputManager;
    // Start is called before the first frame update
    void Start()
    {
        PlayerInputManager = FindObjectOfType<PlayerInputManager>();
        Pauseable = true;
        Instance = this;
        OptionsMenu = GameObject.Find("Settings").GetComponent<Animator>();
        OptionsBackButton = GameObject.Find("BackButton");
        ResumeButton = GameObject.Find("Resume");
        EventSystem.current.firstSelectedGameObject = ResumeButton;
        IsPaused = false;
        Animator = GetComponent<Animator>();
        Time.timeScale = 1;
        PauseButtons = GameObject.Find("Back").GetComponentsInChildren<Button>();
        foreach (var button in PauseButtons)
        {
            button.interactable = false;
        }
        if (Meathead.Instance != null)
        {
            Meathead.Instance.Animator.SetBool("Walking", false);
            Meathead.Instance.enabled = true;
        }
        if (Combover.Instance != null)
        {
            Combover.Instance.Animator.SetBool("Walking", false);
            Combover.Instance.enabled = true;
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            PauseButton();
        }
        if ((Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0) && EventSystem.current.currentSelectedGameObject == null)
        {
            EventSystem.current.SetSelectedGameObject(EventSystem.current.firstSelectedGameObject);
        }
        //print(EventSystem.current.currentSelectedGameObject);
    }

    public void OnNavigate()
    {

    }
    public void PauseButton()
    {
        if(!IsPaused)
        {
            Pause();
        }
    }


    public void Pause()
    {
        if (Pauseable)
        {
            if (Meathead.Instance != null)
            {
                Meathead.Instance.enabled = false;
            }
            if (Combover.Instance != null)
            {
                Combover.Instance.enabled = false;
            }
            PlayerInputManager.DisableJoining();
            Time.timeScale = 0;
            EventSystem.current.SetSelectedGameObject(ResumeButton);
            IsPaused = true;
            Animator.SetBool("Paused", true);

            foreach (var button in PauseButtons)
            {
                button.interactable = true;
            }
        }
    }

    public void Resume()
    {
        if (Meathead.Instance != null)
        {
            Meathead.Instance.Animator.SetBool("Walking", false);
            Meathead.Instance.enabled = true;
        }
        if (Combover.Instance != null)
        {
            Combover.Instance.Animator.SetBool("Walking", false);
            Combover.Instance.enabled = true;
        }
        PlayerInputManager.EnableJoining();
        Time.timeScale = 1;
        EventSystem.current.SetSelectedGameObject(null);
        IsPaused = false;
        Animator.SetBool("Paused", false);

        foreach (var button in PauseButtons)
        {
            button.interactable = false;
        }
    }

    public void Reset()
    {
        Transition.Instance.FadeToScene(GameManager.Level);
    }

    public void Options()
    {
        OptionsMenu.SetBool("Toggle", true);
        EventSystem.current.firstSelectedGameObject = OptionsBackButton;
        EventSystem.current.SetSelectedGameObject(OptionsBackButton);
        foreach (var button in PauseButtons)
        {
            button.interactable = false;
        }
    }

    public void OptionsBack()
    {
        OptionsMenu.SetBool("Toggle", false);
        EventSystem.current.firstSelectedGameObject = ResumeButton;
        EventSystem.current.SetSelectedGameObject(ResumeButton);
        foreach (var button in PauseButtons)
        {
            button.interactable = true;
        }
    }

    public void Quit()
    {
        Time.timeScale = 1;
        Transition.Instance.FadeToScene(0);
    }
}
