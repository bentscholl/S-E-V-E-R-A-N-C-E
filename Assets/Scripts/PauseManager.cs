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
    Button[] Buttons;
    EventSystem EventSystem;
    GameObject PauseMenu;
    GameObject ResumeButton;
    Animator Animator;
    // Start is called before the first frame update
    void Start()
    {
        Pauseable = true;
        Instance = this;
        EventSystem = FindObjectOfType<EventSystem>();
        PauseMenu = GameObject.Find("PauseHandbook");
        ResumeButton = GameObject.Find("Resume");
        IsPaused = false;
        Animator = GetComponent<Animator>();
        Time.timeScale = 1;
        Buttons = GameObject.Find("Back").GetComponentsInChildren<Button>();
        foreach (var button in Buttons)
        {
            button.interactable = false;
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            PauseButton();
        }
        if ((Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0) && EventSystem.currentSelectedGameObject == null)
        {
            EventSystem.SetSelectedGameObject(ResumeButton);
        }

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
            Time.timeScale = 0;
            EventSystem.SetSelectedGameObject(ResumeButton);
            IsPaused = true;
            Animator.SetBool("Paused", true);

            foreach (var button in Buttons)
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
        Time.timeScale = 1;
        EventSystem.SetSelectedGameObject(null);
        IsPaused = false;
        Animator.SetBool("Paused", false);

        foreach (var button in Buttons)
        {
            button.interactable = false;
        }
    }

    public void Options()
    {
        print("Not Implemented");
    }

    public void Quit()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }
}
