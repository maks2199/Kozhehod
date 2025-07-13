using System.Collections;
using TMPro;
using UnityEngine;

public class TypingText : MonoBehaviour
{
    public TMP_Text textComponent;

    [TextArea]
    public string fullText;
    public float typingSpeed = 0.05f;

    private Coroutine typingCoroutine;

    void Start()
    {
        // StartTyping();
    }

    public void StartTyping()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        typingCoroutine = StartCoroutine(TypeText());
    }

    IEnumerator TypeText()
    {
        string currentText = "";
        foreach (char c in fullText)
        {
            currentText += c;
            textComponent.text = currentText;
            yield return new WaitForSecondsRealtime(typingSpeed);
        }
    }
}
