using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterProfile", menuName = "CharacterProfile")]
public class CharacterProfile : ScriptableObject
{
    public string text;

    public string npcName;

    public string npcProfession;
    public Sprite npcPortrait;

    public Sprite deadSprite;

    public List<String> startPhrazes;
}
