using UnityEngine;

[CreateAssetMenu(fileName = "CharacterProfile", menuName = "CharacterProfile")]
public class CharacterProfile : ScriptableObject
{
    public string text;

    public string npcName;
    public Sprite npcPortrait;
}
