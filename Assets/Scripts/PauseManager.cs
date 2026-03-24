using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance;
    public static bool IsPaused = false;
    EventSystem EventSystem;
    GameObject PauseMenu;
    GameObject ResumeButton;
    Animator Animator;
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        EventSystem = FindObjectOfType<EventSystem>();
        PauseMenu = GameObject.Find("PauseHandbook");
        ResumeButton = GameObject.Find("Resume");
        IsPaused = false;
        Animator = GetComponent<Animator>();
        Time.timeScale = 1;
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

    public void PauseButton()
    {
        if(!IsPaused)
        {
            Pause();
        }
    }


    public void Pause()
    {
        if(Meathead.Instance != null)
            Meathead.Instance.enabled = false;
        if(Combover.Instance != null)
            Combover.Instance.enabled = false;
        Time.timeScale = 0;
        EventSystem.SetSelectedGameObject(ResumeButton);
        IsPaused = true;
        Animator.SetBool("Paused", true);
    }

    public void Resume()
    {
        if (Meathead.Instance != null)
            Meathead.Instance.enabled = true;
        if (Combover.Instance != null)
            Combover.Instance.enabled = true;
        Time.timeScale = 1;
        EventSystem.SetSelectedGameObject(null);
        IsPaused = false;
        Animator.SetBool("Paused", false);
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
