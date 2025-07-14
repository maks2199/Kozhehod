using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class NpcStatusItem : MonoBehaviour
{
    public Image portraitImage;
    public TMP_Text nameText;
    public TMP_Text professionText;
    public TMP_Text chatText;

    public NPC connectedNpc;

    public Image monsterIcon;

    // Selected button
    private Coroutine pulseCoroutine;
    private Vector3 originalScale;

    private void Awake()
    {
        originalScale = transform.localScale;
    }

    public void PlayPulse()
    {
        if (pulseCoroutine == null)
        {
            pulseCoroutine = StartCoroutine(PulseEffect());
        }
    }

    public void StopPulse()
    {
        if (pulseCoroutine != null)
        {
            StopCoroutine(pulseCoroutine);
            pulseCoroutine = null;
        }

        transform.localScale = originalScale; // сброс масштаба
    }

    private IEnumerator PulseEffect()
    {
        float pulseSpeed = .80f;       // скорость пульсации
        float pulseAmount = 0.05f;   // насколько масштаб увеличивается

        while (true)
        {
            float t = 0;
            while (t < 1f)
            {
                t += Time.deltaTime * pulseSpeed;
                float scale = 1f + Mathf.Sin(t * Mathf.PI) * pulseAmount;
                transform.localScale = originalScale * scale;
                yield return null;
            }
        }
    }

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
        // monsterIcon.gameObject.SetActive(false);

        chatText.SetText(npc.GetChatHistory());

        // Enable components just in case
        if (!portraitImage.enabled) portraitImage.enabled = true;
        if (!nameText.enabled) nameText.enabled = true;
        if (!professionText.enabled) professionText.enabled = true;
        if (!chatText.enabled) chatText.enabled = true;
        // if (!statusText.enabled) statusText.enabled = true;
        // statusText.gameObject.SetActive(true);
    }
    public void SetupEnd(NPC npc)
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

        monsterIcon.gameObject.SetActive(npc.wasMonster);

        chatText.SetText(npc.GetChatHistory());

        // Enable components just in case
        if (!portraitImage.enabled) portraitImage.enabled = true;
        if (!nameText.enabled) nameText.enabled = true;
        if (!professionText.enabled) professionText.enabled = true;
        if (!chatText.enabled) chatText.enabled = true;

    }
}
