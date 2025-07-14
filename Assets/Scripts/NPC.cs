using System.Collections;
using System;
using System.Threading.Tasks;
using System.Linq;
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

    public bool wasMonster = false;

    public bool isDead = false;

    public Chat chat;

    public Status status;

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
        string modelName, string promptPreamble, string promptConversationRules, string promptStatusHuman, string promptStatusMonster, string playerName)
    {
        string statusText;
        if (isMonster)
        {
            statusText = promptStatusMonster;
        }
        else
        {
            statusText = promptStatusHuman + $"Обращайся к игроку по имени {playerName}";
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
    public string GetChatHistory()
    {
        List<OllamaSharp.Models.Chat.Message> npcChatMessages = chat.Messages;
        string chatLog = string.Join("\n", npcChatMessages
        .Skip(1)
        .Select(m =>
        {
            string role = m.Role.ToString() switch
            {
                "assistant" => characterProfile.npcName,
                "user" => "Вы"
            };
            return $"{role}: {m.Content}";
        }));

        return chatLog;
    }

    public void SetAliveSprite()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null && characterProfile.aliveSprite != null)
        {
            sr.sprite = characterProfile.aliveSprite;
        }
    }

    // public void SetStatusMonstered()
    // {
        
    // }

    public enum Status
    {
        Alive,
        Killed,
        Monstered
    }
}


