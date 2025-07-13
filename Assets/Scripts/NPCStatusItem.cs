using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NpcStatusItem : MonoBehaviour
{
    public Image portraitImage;
    public TMP_Text nameText;
    public TMP_Text professionText;
    public TMP_Text chatText;

    public NPC connectedNpc;

    public void Setup(NPC npc)
    {
        connectedNpc = npc;

        nameText.SetText(npc.characterProfile.npcName);
        professionText.SetText(npc.characterProfile.npcProfession);

        if (npc.status == NPC.Status.Killed)
        {
            portraitImage.sprite = npc.characterProfile.deadSprite;
        }
        else if (npc.status == NPC.Status.Monstered)
        {
            portraitImage.sprite = npc.characterProfile.monsteredSprite;
        }
        else
        {
            portraitImage.sprite = npc.characterProfile.aliveSprite;
        }
        // statusText.text = npc.GetStatus(); // например, "Жив" или "Подозрительный"

        chatText.SetText(npc.GetChatHistory());

        // Enable components just in case
        if (!portraitImage.enabled) portraitImage.enabled = true;
        if (!nameText.enabled) nameText.enabled = true;
        if (!professionText.enabled) professionText.enabled = true;
        if (!chatText.enabled) chatText.enabled = true;
        // if (!statusText.enabled) statusText.enabled = true;
        // statusText.gameObject.SetActive(true);
    }
}
