using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dialogue")]
public class TextBlob : ScriptableObject
{
    public string[] Strings;
    public Sprite Profile;
    public string Name;
    public Color Color;
}
