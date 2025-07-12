using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NpcStatusItem : MonoBehaviour
{
    public Image portraitImage;
    public TMP_Text nameText;
    public TMP_Text professionText;
    public TMP_Text statusText;

    public void Setup(NPC npc)
    {
        nameText.SetText(npc.characterProfile.npcName);
        professionText.SetText(npc.characterProfile.npcProfession);
        portraitImage.sprite = npc.characterProfile.npcPortrait;
        // statusText.text = npc.GetStatus(); // например, "Жив" или "Подозрительный"
        
        // Enable components just in case
        if (!portraitImage.enabled) portraitImage.enabled = true;
        if (!nameText.enabled) nameText.enabled = true;
        if (!professionText.enabled) professionText.enabled = true;
        // if (!statusText.enabled) statusText.enabled = true;
        // statusText.gameObject.SetActive(true);
    }
}
