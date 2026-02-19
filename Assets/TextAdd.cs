using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextAdd : MonoBehaviour
{
    TextMeshProUGUI TextBox;
    public AudioClip LetterSound;
    AudioSource Source;
    // Start is called before the first frame update
    void Start()
    {
        TextBox = GetComponent<TextMeshProUGUI>();
        Source = gameObject.AddComponent<AudioSource>();
        TextBox.maxVisibleCharacters = 0;
        StartCoroutine(FillText());
    }

    // Update is called once per frame
    
    IEnumerator FillText()
    {
        yield return new WaitForSeconds(2f);
        while(TextBox.maxVisibleCharacters < TextBox.textInfo.characterCount)
        {
            TextBox.maxVisibleCharacters++;
            Source.PlayOneShot(LetterSound);
            yield return new WaitForSeconds(.1f);
        }
    }
}
