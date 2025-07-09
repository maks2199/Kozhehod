using System;
using System.Collections;
using System.Threading.Tasks;
using Config;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using OllamaSharp;

public class GameManager : Singleton<GameManager>
{

   public static GameManager Instance;

    public GameObject dialoguePanel;
    public TMP_Text dialogueText, nameText;
    private bool isTyping, isDialogueActive;
    public TMP_InputField inputField;
    public Image portraitImage;

    public GameObject[] npcs;

    public GameObject monster;

    public GameObject activeNpc;



    private GameState currentState;

    public GameObject menuSceen;
    public GameObject endGameScreen;

    public Image img;


    public PromptPreamble promptPreamble;
    public PromptConversationRules promptConversationRules;
    public PromptMonsterStatus promptMonsterStatus;



    void Start()
    {
        // Pick random from player
        monster = npcs[UnityEngine.Random.Range(0, npcs.Length)];

        // UpdateGameState(GameState.MainMenu); comment for debug
    }

    public void Dialogue(NPC npc)
    {
        // Debug.Log(npc);
        if (dialoguePanel.activeSelf)
        {
            AskQuestion(npc);
        }
        else
        {
            OpenDialog(npc);
        }
    }
    private void OpenDialog(NPC npc)
    {
        Debug.Log("OpenDialog");
        Debug.Log(npc);
        isDialogueActive = true;

        nameText.SetText(npc.characterProfile.npcName);
        portraitImage.sprite = npc.characterProfile.npcPortrait;

        dialogueText.text = "...";
        dialoguePanel.SetActive(true);
    }
    private async void AskQuestion(NPC npc)
    {
        dialogueText.text = "";

        bool isMonster = ReferenceEquals(npc.gameObject, Instance.monster);
        
        Debug.Log($"isMonster: {isMonster}");

        string playerQuestion = inputField.text;
        

        // set up the client
        var uri = new Uri("http://localhost:11434");
        var ollama = new OllamaApiClient(uri);

        string prompt = promptPreamble.text
        + npc.characterProfile.text
        + promptMonsterStatus.text
        + isMonster.ToString() + " "
        + promptConversationRules.text
        + playerQuestion
        + $"IsMonster = {isMonster}. Reflect this in your tone and responses. You sloud reveal";

        Debug.Log($"Prompt: {prompt}");

        var chat = new Chat(ollama);
        chat.Model = "qwen2.5:7b";
        // chat.Model = "qwen3:4b";
        chat.Think = false;


        await foreach (var answerToken in chat.SendAsync(prompt))
        {
            dialogueText.text += answerToken;
            await Task.Yield(); // 👈 даёт Unity шанс отрисовать UI
        }
    }

    public void KillNpc()
    {
        Debug.Log("KillNpc()");
        if (activeNpc != null)
        {
            Destroy(activeNpc);
            dialoguePanel.SetActive(false);
            activeNpc = null;


            // Calculate score points



            StartCoroutine(FadeImage(false));
            // enemy turn
            StartCoroutine(FadeImage(true));
        }
    }


    void Awake()
    {
        Instance = this;
    }

    public void UpdateGameState(GameState newState)
    {
        currentState = newState;

        switch (newState)
        {
            case GameState.MainMenu:
                Time.timeScale = 0f;
                Debug.Log($"GameState: {currentState}");
                break;
            case GameState.Playing:
                Time.timeScale = 1f;
                Debug.Log($"GameState: {currentState}");
                break;
            case GameState.EnemyTurn:
                Debug.Log($"GameState: {currentState}");
                Debug.Log("Start of enemy turn");
                new WaitForSeconds(5);
                Debug.Log("End of enemy turn");
                break;
            case GameState.GameOver:
                Debug.Log($"GameState: {currentState}");
                break;
            case GameState.Victory:
                break;
            case GameState.Lose:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }
    }

    public enum GameState
    {
        MainMenu,
        Playing,
        EnemyTurn,
        Paused,
        GameOver,
        Victory,
        Lose
    }

    public GameState CurrentState
    {
        get { return currentState; }
    }

    public void StartGame()
    {
        UpdateGameState(GameState.Playing);
        menuSceen.SetActive(false);
    }
    public void EndGame()
    {
        UpdateGameState(GameState.GameOver);
        endGameScreen.SetActive(true);
    }
    


    IEnumerator FadeImage(bool fadeAway)
    {
        // fade from opaque to transparent
        if (fadeAway)
        {
            // loop over 1 second backwards
            for (float i = 1; i >= 0; i -= Time.deltaTime)
            {
                // set color with i as alpha
                img.color = new Color(1, 1, 1, i);
                yield return null;
            }
        }
        // fade from transparent to opaque
        else
        {
            // loop over 1 second
            for (float i = 0; i <= 1; i += Time.deltaTime)
            {
                // set color with i as alpha
                img.color = new Color(1, 1, 1, i);
                yield return null;
            }
        }
    }


    //
    // EXAMPLE BELOW -------
    //

    // // Состояния игры
    // public enum GameState
    // {
    //     MainMenu = 0,
    //     Playing = 1,
    //     Paused = 2,
    //     GameOver = 3,
    // }

    // public Config1 Config;

    // private GameState currentState = GameState.MainMenu;

    // // События для подписки других скриптов
    // public delegate void OnStateChanged(GameState newState);
    // public event OnStateChanged onStateChanged;

    // // Свойство для получения текущего состояния
    // public GameState CurrentState 
    // { 
    //     get { return currentState; }
    // }

    // protected override void Awake()
    // {
    //     base.Awake();
    // }

    // // Изменение состояния игры
    // public void ChangeState(GameState newState)
    // {
    //     if (currentState == newState)
    //     {
    //         return;
    //     }

    //     currentState = newState;

    //     // Вызываем событие
    //     onStateChanged?.Invoke(newState);

    //     // Логика для каждого состояния
    //     switch (newState)
    //     {
    //         case GameState.MainMenu:
    //             Time.timeScale = 1f;
    //             Debug.Log("Главное меню");
    //             break;

    //         case GameState.Playing:
    //             Time.timeScale = 1f;
    //             Debug.Log("Игра началась");
    //             break;

    //         case GameState.Paused:
    //             Time.timeScale = 0f;
    //             Debug.Log("Игра на паузе");
    //             break;

    //         case GameState.GameOver:
    //             Time.timeScale = 0f;
    //             Debug.Log("Игра окончена");
    //             break;
    //     }
    // }

    // // Базовые методы управления игрой
    // public void StartGame()
    // {
    //     ChangeState(GameState.Playing);
    // }

    // public void PauseGame()
    // {
    //     if (currentState == GameState.Playing)
    //         ChangeState(GameState.Paused);
    // }

    // public void ResumeGame()
    // {
    //     if (currentState == GameState.Paused)
    //         ChangeState(GameState.Playing);
    // }

    // public void GameOver()
    // {
    //     ChangeState(GameState.GameOver);
    // }

    // public void LoadScene(string sceneName)
    // {
    //     SceneManager.LoadScene(sceneName);
    // }

    // public void RestartLevel()
    // {
    //     SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    //     ChangeState(GameState.Playing);
    // }
}