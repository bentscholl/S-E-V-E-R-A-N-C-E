using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CEOSpeak : MonoBehaviour
{
    TextMeshProUGUI TextBox;
    public AudioClip LetterSound;
    AudioSource Source;
    public string DialogueName;
    TextBlob Dialogue;
    MaskableGraphic[] TextboxElements;

    // Start is called before the first frame update
    void Start()
    {
        TextBox = GameObject.Find("Text").GetComponent<TextMeshProUGUI>();
        Source = gameObject.AddComponent<AudioSource>();
        Dialogue = (TextBlob)Resources.Load("Dialogue/" + DialogueName);
        StartCoroutine(PlayDialogue());
        TextboxElements = GetComponentsInChildren<MaskableGraphic>();
        foreach (MaskableGraphic maskableGraphic in TextboxElements)
        {
            maskableGraphic.enabled = false;
        }
    }

    // Update is called once per frame
    
    IEnumerator PlayDialogue()
    {
        yield return new WaitForSeconds(1.5f);
        foreach (MaskableGraphic maskableGraphic in TextboxElements)
        {
            maskableGraphic.enabled = true;
        }
        yield return new WaitForSeconds(.5f);
        for (int i = 0; i < Dialogue.Strings.Length; i++)
        {
            TextBox.maxVisibleCharacters = 0;
            TextBox.text = Dialogue.Strings[i];
            do
            {
                TextBox.maxVisibleCharacters++;
                Source.PlayOneShot(LetterSound);
                yield return new WaitForSeconds(.1f);
            } while (TextBox.maxVisibleCharacters < TextBox.textInfo.characterCount);
            yield return new WaitForSeconds(1);
        }
        yield return new WaitForSeconds(1);
        foreach(MaskableGraphic maskableGraphic in TextboxElements)
        {
            maskableGraphic.enabled = false;
        }

        yield return new WaitForSeconds(1.5f);

        foreach (MaskableGraphic maskableGraphic in TextboxElements)
        {
            maskableGraphic.enabled = true;
        }

        TextBox.maxVisibleCharacters = 0;
        TextBox.text = "Oh, but don't kill my secretary. I need them.";
        while (TextBox.maxVisibleCharacters < TextBox.textInfo.characterCount)
        {
            TextBox.maxVisibleCharacters++;
            Source.PlayOneShot(LetterSound);
            yield return new WaitForSeconds(.1f);
        }

        yield return new WaitForSeconds(1.5f);

        foreach (MaskableGraphic maskableGraphic in TextboxElements)
        {
            maskableGraphic.enabled = false;
        }

        yield return new WaitForSeconds(1.3f);
        Transition.Instance.FadeToScene(1);
    }
}
