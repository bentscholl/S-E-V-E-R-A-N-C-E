using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Checkbox : MonoBehaviour
{
    Transform Parent;
    Image Image;
    private void Start()
    {
        Parent = transform.parent.parent;
        Image = GetComponent<Image>();
    }
    void Update()
    {
        if(EventSystem.current.currentSelectedGameObject == Parent.gameObject)
        {
            Image.color = Color.white;
        }
        else 
        {
            Image.color = new Color(.6f,.6f,.6f);
        }
            
    }
}
