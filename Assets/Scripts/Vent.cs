using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Vent : MonoBehaviour
{
    public List<Vent> Vents;
    Transform ArrowParent;
    public float ZAngle;
    public Animator Animator;
    public SpriteRenderer[] Arrows;
    // Start is called before the first frame update
    void Start()
    {
        GameObject ArrowTemplate = (GameObject)Resources.Load("Arrow");
        ArrowParent = transform.GetChild(0);
        int count = 0;
        Arrows = new SpriteRenderer[Vents.Count];
        foreach (var vent in Vents)
        {
            GameObject Arrow = Instantiate(ArrowTemplate, ArrowParent);
            Arrow.transform.LookAt(vent.transform.position, Vector3.up);
            Arrow.transform.Translate(0, 0, 1);
            SpriteRenderer Indicator = Arrow.GetComponent<SpriteRenderer>();
            Arrows[count++] = Indicator;
            Vector3 Direction = vent.transform.position - transform.position;
            ZAngle = Mathf.Atan2(Direction.z, Direction.x) * Mathf.Rad2Deg;
            float rad = Mathf.Atan2(Direction.z, Direction.x);
            Arrow.transform.rotation = Quaternion.Euler(new Vector3(0, 0, ZAngle - 90));
            //Indicator.transform.eulerAngles = new Vector3(0, 0, -(ZAngle - 90));

        }
        ToggleArrows(false);
        Animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ToggleArrows(bool toggle)
    {
        ArrowParent.gameObject.SetActive(toggle);
        if (toggle)
        {
            Select(0);
        }
    }

    public void Select(int index)
    {
        foreach (SpriteRenderer spriteRenderer in Arrows)
        {
            spriteRenderer.color = Color.white;
        }
        Arrows[index].color = Color.yellow;
    }
}
