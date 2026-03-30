using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Transition : MonoBehaviour
{
    Animator animator;
    public static Transition Instance;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        if(Instance == null)
        {
            //animator.SetTrigger("Off");
        }
        Instance = this;
    }

    public void FadeToScene(int Scene)
    {
        animator.SetTrigger("Fade");
        StartCoroutine(Load(Scene));
    }

    public void FadeToScene(string Scene)
    {
        animator.SetTrigger("Fade");
        StartCoroutine(Load(Scene));
    }

    IEnumerator Load(int Scene)
    {
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(Scene);
    }

    IEnumerator Load(string Scene)
    {
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(Scene);
    }
}
