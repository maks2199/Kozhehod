using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestionCounterUI : MonoBehaviour
{
    public List<Image> questionIcons; // Set in Inspector or dynamically
    public Sprite fullSprite;         // Visible icon
    public Sprite emptySprite;        // Disappeared/used icon (or set to null)

    private int currentCount;

    void Start()
    {
        // StartTyping();
    }

    public void Setup(int totalQuestions)
    {
        currentCount = totalQuestions;

        for (int i = 0; i < questionIcons.Count; i++)
        {
            questionIcons[i].sprite = (i < currentCount) ? fullSprite : emptySprite;
            questionIcons[i].enabled = true;
        }
    }

    public void UseQuestion()
    {
        if (currentCount <= 0) return;

        currentCount--;
        Image icon = questionIcons[currentCount];

        // Either disable, change sprite, or fade
        // Option A: Disable
        // icon.enabled = false;

        // Option B: Change sprite
        icon.sprite = emptySprite;

        // Option C: Fade out
        StartCoroutine(FadeOut(icon));
    }

    private IEnumerator FadeOut(Image image)
    {
        float duration = 0.5f;
        float elapsed = 0f;
        Color color = image.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            image.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }

        // image.enabled = false;
    }
}
