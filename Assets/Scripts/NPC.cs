using System.Collections;
using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using OllamaSharp;
using Unity.VisualScripting;

public class NPC : MonoBehaviour, IInteractable
{
    public CharacterProfile characterProfile;
    public bool isMonster;


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

}


