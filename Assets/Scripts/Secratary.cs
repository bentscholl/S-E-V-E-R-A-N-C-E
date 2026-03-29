using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Secratary : NPC
{
    SpriteRenderer SpriteRenderer;
    // Start is called before the first frame update
    void Start()
    {
        if(GameManager.Instance != null)
            GameManager.Instance.StartingTotal--;
        SpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        Sprites = Resources.LoadAll<Sprite>("NPCs/NPC" + Random.Range(1, 16));
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        SpriteRenderer.sprite = Sprites[SpriteIndex];
    }
}
