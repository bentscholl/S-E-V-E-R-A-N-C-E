using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class ButtonTest : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    Button Button;
    EventSystem system;
    // Start is called before the first frame update
    void Start()
    {
        system = FindAnyObjectByType<EventSystem>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        system.SetSelectedGameObject(this.gameObject);
        system.sendNavigationEvents = false;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        system.sendNavigationEvents = true;
        system.SetSelectedGameObject(null);
    }
}
