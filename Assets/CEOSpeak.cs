using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CEOSpeak : MonoBehaviour
{
    TextMeshProUGUI TextBox;
    public AudioClip LetterSound;
    AudioSource Source;
    public string DialogueName;
    TextBlob Dialogue;

    // Start is called before the first frame update
    void Start()
    {
        TextBox = GameObject.Find("Text").GetComponent<TextMeshProUGUI>();
        Source = gameObject.AddComponent<AudioSource>();
        Dialogue = (TextBlob)Resources.Load("Dialogue/" + DialogueName);
        StartCoroutine(PlayDialogue());
    }

    // Update is called once per frame
    
    IEnumerator PlayDialogue()
    {
        yield return new WaitForSeconds(2f);
        for(int i = 0; i < Dialogue.Strings.Length; i++)
        {
            TextBox.maxVisibleCharacters = 0;
            TextBox.text = Dialogue.Strings[i];
            while (TextBox.maxVisibleCharacters < TextBox.textInfo.characterCount)
            {
                TextBox.maxVisibleCharacters++;
                Source.PlayOneShot(LetterSound);
                yield return new WaitForSeconds(.1f);
            }
            yield return new WaitForSeconds(1);
        }
        
    }
}
