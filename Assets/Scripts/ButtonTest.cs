using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

public class ButtonTest : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    UnityEngine.UI.Button Button;
    EventSystem system;
    // Start is called before the first frame update
    void Awake()
    {
        system = FindAnyObjectByType<EventSystem>();
        Button = GetComponent<UnityEngine.UI.Button>();
        if(!Button.interactable)
        {
            GetComponent<Image>().color = Color.gray;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (Button.interactable)
        {
            system.SetSelectedGameObject(this.gameObject);
            system.sendNavigationEvents = false;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (Button.interactable)
        {
            system.sendNavigationEvents = true;
            system.SetSelectedGameObject(null);
        }
    }
}
