using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

public class CustomSelectable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    EventSystem system;
    UnityEngine.UI.Selectable Selectable;
    // Start is called before the first frame update
    void Awake()
    {
        system = FindAnyObjectByType<EventSystem>();
        Selectable = GetComponent<UnityEngine.UI.Selectable>();
        if(!Selectable.interactable)
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
        if (Selectable.interactable)
        {
            system.SetSelectedGameObject(this.gameObject);
            system.sendNavigationEvents = false;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (Selectable.interactable)
        {
            system.sendNavigationEvents = true;
            system.SetSelectedGameObject(null);
        }
    }
}
