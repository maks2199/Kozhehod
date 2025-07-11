using System.Collections;
using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using OllamaSharp;
using Unity.VisualScripting;
using System.Collections.Generic;
using OllamaSharp;

public class NPC : MonoBehaviour, IInteractable
{
    public CharacterProfile characterProfile;
    public bool isMonster = false;

    public Chat chat;

    public bool CanInteract()
    {
        // return !isDialogueActive;
        return true;
    }

    public void Interact()
    {
        Debug.Log("Interaction happened!");
        GameManager.Instance.Dialogue(this);
    }

    public void UpdateChat(OllamaApiClient ollama,
        string modelName, string promptPreamble, string promptConversationRules, string promptStatusHuman, string promptStatusMonster)
    {
        string statusText;
        if (isMonster)
        {
            statusText = promptStatusMonster;
        }
        else
        {
            statusText = promptStatusHuman;
        }

        string systemPrompt =
            promptPreamble + " "
            + characterProfile.text + " "
            + promptConversationRules + " "
            + statusText;
        Debug.Log(systemPrompt);
        chat = new Chat(ollama, systemPrompt);
        chat.Model = modelName;
        chat.Think = false;

        // return chat;
    }
}


