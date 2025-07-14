using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Config;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using OllamaSharp;
using Unity.VisualScripting;
using System.Linq;
using System.IO;
using NUnit.Framework.Constraints;
using Spine.Unity;



public class GameManager : Singleton<GameManager>
{

    public static GameManager Instance;

    public string llmModelName = "qwen2.5:7b";
    Uri uri;
    OllamaApiClient ollama;
    Chat chat;


    public GameObject dialoguePanel;
    public TMP_Text dialogueText, nameText, professionText;
    private bool isTyping, isDialogueActive;
    public TMP_InputField inputField;
    public Image portraitImage;

    // public GameObject[] npcs;
    public List<GameObject> npcs;
    private List<GameObject> allNpcs; // копируем изначально всех NPC


    public GameObject monster;

    public GameObject activeNpc;



    private GameState currentState;


    // UI ----------

    public GameObject menuSceen;
    public SkeletonGraphic skeletonAnimation;

    public TMP_InputField playerNameField;
    public GameObject tutorialSceen;
    public TypingText tutorialTypingText;
    public GameObject lowNpcSceen;
    public TMP_Text AnalyzeScreenChatHistory;
    public TMP_Text AnalyzeScreenNPCName;
    public GameObject endGameScreen;

    public GameObject playerAnalyzeScreen;
    public TMP_Text endGameScreenResult;
    public TMP_Text endGameScreenAlive;
    public TMP_Text endGameScreenQuestions;
    public TMP_Text endGameScreenQuestionsCount;
    public TMP_Text endGameScreenScore;
    public TMP_Text endGameScreenLeaderboard;
    public TMP_Text uiMonsterNames;
    public TMP_Text uiRemainedQuestionCount;
    public TMP_Text uiCurrentScore;

    public Image fadeImg;

    public TMP_Text enemyTurnText;

    // UI ----------
    public GameObject UI;
    public PromptPreamble promptPreamble;
    public PromptConversationRules promptConversationRules;
    public PromptMonsterStatus promptMonsterStatus;
    public PromptMonsterStatus promptHumanStatus;
    public GameObject thinkingAnimation;

    private string enemyScreenText = "Твой ход, Кожеход...";

    // Player Analyze screen
    public GameObject npcStatusScreen;
    public Transform npcStatusListContainer; // content для списка
    public Transform npcStatusListContainerEnd; // content для списка
    public GameObject npcStatusItemPrefab;
    public GameObject npcStatusItemPrefabEnd;
    private NpcStatusItem lastClickedNpcStatusItem;


    // Game loop
    public int maxQuestions = 10;
    private int intRemainedQuestionCount;
    private int countAskedQuestions = 0;
    public QuestionCounterUI questionCounterUI;
    public float chanceToChangeBody;

    public int npcCountToKillPlayer;
    public float chanceToKillPlayer;
    // public int currentScore;
    private List<String> monsterNames = new List<string>();


    void Awake()
    {
        Instance = this;
        Screen.SetResolution(1920, 1080, true);
    }

    void Start()
    {
        skeletonAnimation.AnimationState.SetAnimation(0, "animation", true);

        // set up the client
        uri = new Uri("http://localhost:11434");
        ollama = new OllamaApiClient(uri);

        UpdateGameState(GameState.MainMenu); // comment for debug
        // UpdateGameState(GameState.Playing);
        EnemyMoveToAnotherNpc();

        allNpcs = new List<GameObject>(npcs);

        questionCounterUI.Setup(maxQuestions);

    }


    void Update()
    {
        if (thinkingAnimation.activeSelf)
        {
            thinkingAnimation.transform.Rotate(0f, 0f, 100f * Time.deltaTime);
        }
    }

    public void RestartGame()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }

    void StartPlayerTurn()
    {
        

        RefreshQuestionCounter();

        npcs = new List<GameObject>(npcs); // преобразуем из массива в список
        foreach (GameObject npcGO in npcs) // Инициализируем чат для  всех NPC
        {
            NPC npc = npcGO.GetComponent<NPC>();

            // Set alive sprite here
            npc.SetAliveSprite();

            npc.UpdateChat(
                ollama,
                llmModelName,
                promptPreamble.text,
                promptConversationRules.text,
                promptHumanStatus.text,
                promptMonsterStatus.text,
                playerNameField.text);
        }

        // EnemyMoveToAnotherNpc();
    }

    public void ContinuePlaying()
    {
        UpdateGameState(GameState.Playing);
        RefreshQuestionCounter();
        UpdateCurrentScore();
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
        professionText.SetText(npc.characterProfile.npcProfession);
        portraitImage.sprite = npc.characterProfile.npcPortrait;
        portraitImage.preserveAspect = true;


        // Get random phraze from list
        string startPhraze = npc.characterProfile.startPhrazes[UnityEngine.Random.Range(0, npc.characterProfile.startPhrazes.Count)];
        dialogueText.SetText(startPhraze);
        dialoguePanel.SetActive(true);

        // Display chat history
        string chatHistory = npc.GetComponent<NPC>().GetChatHistory();
        // dialogueText.SetText(chatHistory);
        dialogueText.SetText(startPhraze);

        bool isMonster = ReferenceEquals(npc.gameObject, Instance.monster);
        Debug.Log($"isMonster: {isMonster}");

        // Create LLM chat for each npc!
        // chat = new Chat(ollama);
        // chat.Model = llmModelName;
        // chat.Think = false;
    }
    private async void AskQuestion(NPC npc)
    {
        if (intRemainedQuestionCount > 0)
        {
            countAskedQuestions++;
            DecreeseQuestionCounter();

            dialogueText.text = "";

            bool isMonster = ReferenceEquals(npc.gameObject, Instance.monster);
            // string statusText;
            // if (isMonster)
            // {
            //     statusText = promptMonsterStatus.text;
            // }
            // else
            // {
            //     statusText = promptHumanStatus.text;
            // }

            Debug.Log($"isMonster: {isMonster}");

            string playerQuestion = inputField.text;

            endGameScreenQuestions.text += $"\n {playerQuestion}";




            // string prompt = promptPreamble.text + " "
            // + npc.characterProfile.text + " "
            // + statusText + " "
            // + promptConversationRules.text + " "
            // + playerQuestion;

            chat = npc.chat;

            // Debug.Log($"Prompt: {prompt}");
            Debug.Log($"Model: {llmModelName}");

            

            thinkingAnimation.SetActive(true);
            await foreach (var answerToken in chat.SendAsync(playerQuestion))
            {
                dialogueText.text += answerToken;
                await Task.Yield(); // 👈 даёт Unity шанс отрисовать UI
            }
            Debug.Log("Messages:");
            chat.Messages.ForEach(m => Debug.Log(m.ToString()));
            thinkingAnimation.SetActive(false);

            UpdateCurrentScore();
        }
        else
        {
            Debug.Log("Out of questions!");
        }

    }


    // Game loop -------------------------------

    public void CloseDialoguePanel()
    {
        dialoguePanel.SetActive(false);
    }
    public void KillNpcPlayer()
    {
        Debug.Log("KillNpc()");
        // bool isMonster = ReferenceEquals(activeNpc.gameObject, Instance.monster);
        if (activeNpc != null)
        {
            // if (isMonster)
            // {

            // }

            KillNpc(activeNpc);

            dialoguePanel.SetActive(false);
            activeNpc = null;


            // Calculate score points
            EndPlayerTurn();
        }
    }



    private void KillNpc(GameObject npc)
    {
        npcs.Remove(npc); // удаляем из списка
        // Destroy(activeNpc);     // уничтожаем объект // TODO replace with dead image instead of remove
        SpriteRenderer spriteRenderer = npc.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = npc.GetComponent<NPC>().characterProfile.deadSprite;
        npc.GetComponent<CircleCollider2D>().enabled = false;
        npc.GetComponent<NPC>().isDead = true;
        npc.GetComponent<NPC>().status = NPC.Status.Killed;

        // spriteRenderer.sprite.
    }
    private void KillNpcByMonster(GameObject npc)
    {
        npcs.Remove(npc); // удаляем из списка
        // Destroy(activeNpc);     // уничтожаем объект // TODO replace with dead image instead of remove
        SpriteRenderer spriteRenderer = npc.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = npc.GetComponent<NPC>().characterProfile.monsteredSprite;
        npc.GetComponent<CircleCollider2D>().enabled = false;
        npc.GetComponent<NPC>().isDead = true;
        npc.GetComponent<NPC>().status = NPC.Status.Monstered;

        // spriteRenderer.sprite.
    }

    public void EndPlayerTurn()
    {
        UpdateGameState(GameState.EnemyTurn);
    }

    private void RefreshQuestionCounter()
    {
        intRemainedQuestionCount = maxQuestions;
        uiRemainedQuestionCount.SetText(intRemainedQuestionCount.ToString());
        questionCounterUI.Setup(maxQuestions);
    }
    private void DecreeseQuestionCounter()
    {
        intRemainedQuestionCount--;
        uiRemainedQuestionCount.SetText(intRemainedQuestionCount.ToString());
        questionCounterUI.UseQuestion();
    }

    public void MakeEnemyTurn()
    {
        Debug.Log("MakeEnemyTurn()");

        // Получаем список всех НЕ-монстров
        List<GameObject> aliveNonMonsterNpcs;

        bool monsterAlive = npcs.Contains(monster);
        bool shouldMoveToAnotherNpc = UnityEngine.Random.value <= chanceToChangeBody / 100f;
        Debug.Log($"shouldMoveToAnotherNpc: {shouldMoveToAnotherNpc}");
        bool shouldMoveToPlayer = false;
        if (npcs.Count <= npcCountToKillPlayer)
        {
            shouldMoveToPlayer = UnityEngine.Random.value <= chanceToKillPlayer / 100f;
        }


        // If monster is still alive
        if (monsterAlive)
        {

            if (shouldMoveToPlayer)
            {
                Debug.Log("Monster moved to Player!");
                EndGame();
                return;
            }
            else if (shouldMoveToAnotherNpc)
            {
                aliveNonMonsterNpcs = npcs.FindAll(npc => npc != monster);
                if (aliveNonMonsterNpcs.Count != 0)
                {
                    // Выбираем случайного НЕ-монстра
                    GameObject randomNpc = aliveNonMonsterNpcs[UnityEngine.Random.Range(0, aliveNonMonsterNpcs.Count)];

                    Debug.Log($"EnemyTurn kills: {randomNpc.name}");

                    // npcs.Remove(randomNpc); // удаляем из общего списка
                    // Destroy(activeNpc);     // уничтожаем объект // TODO replace with dead image instead of remove
                    // KillNpc(randomNpc);
                    KillNpcByMonster(randomNpc);

                    EnemyMoveToAnotherNpc();
                }
            }
            aliveNonMonsterNpcs = npcs.FindAll(npc => npc != monster);
            if (aliveNonMonsterNpcs.Count == 0)
            {
                Debug.Log("No more non-monster NPCs left!");
                EndGame();
                return;
            }

        }
        else
        {

        }

        UpdateNpcStatusScreen();
        UpdateGameState(GameState.PlayerAnalyze);

        // UpdateGameState(GameState.Playing);
        // RefreshQuestionCounter();
        // UpdateCurrentScore();

        // Status of npcs
        
        
    }

    private void UpdateNpcStatusScreen()
    {
        // bool first = true;

        // Очистим список
        foreach (Transform child in npcStatusListContainer)
        {
            Destroy(child.gameObject);
        }

        NpcStatusItem firstItem = null;

        // Добавим все живые NPC -- надо и мертвых тоже
        foreach (GameObject npcGO in allNpcs)
        {
            if (npcGO == null) continue;
            NPC npc = npcGO.GetComponent<NPC>();
            GameObject itemGO = Instantiate(npcStatusItemPrefab, npcStatusListContainer);
            NpcStatusItem item = itemGO.GetComponent<NpcStatusItem>();
            item.Setup(npc);


            if (firstItem == null)
            {
                firstItem = item;
            }
        }

        // Вызываем PlayPulse и OpenChatHistory для первого элемента
        if (firstItem != null)
        {
            // firstItem.PlayPulse();
            // OpenChatHistory(firstItem.gameObject);
            StartCoroutine(ActivateFirstItemPulse(firstItem));
        }

    }
    private IEnumerator ActivateFirstItemPulse(NpcStatusItem item)
    {
        yield return null; // дождаться активации объекта в Unity
        OpenChatHistory(item.gameObject);
    }
    private void UpdateNpcEndScreen()
    {
        // Очистим список
        foreach (Transform child in npcStatusListContainerEnd)
        {
            Destroy(child.gameObject);
        }

        // Добавим все живые NPC -- надо и мертвых тоже
        foreach (GameObject npcGO in allNpcs)
        {
            if (npcGO == null) continue;
            NPC npc = npcGO.GetComponent<NPC>();
            GameObject itemGO = Instantiate(npcStatusItemPrefabEnd, npcStatusListContainerEnd);
            NpcStatusItem item = itemGO.GetComponent<NpcStatusItem>();
            item.SetupEnd(npc);
        }
    }

    private void EnemyMoveToAnotherNpc()
    {
        // Pick random from player
        monster = npcs[UnityEngine.Random.Range(0, npcs.Count)];


        NPC monsterNpc = monster.GetComponent<NPC>();
        string monsterName = monsterNpc.characterProfile.npcName;
        monsterNpc.isMonster = true;
        monsterNpc.wasMonster = true;
        monsterNpc.UpdateChat(
            ollama,
            llmModelName,
            promptPreamble.text,
            promptConversationRules.text,
            promptHumanStatus.text,
            promptMonsterStatus.text,
            playerNameField.text);


        // Log choice
        monsterNames.Add(monsterName);
    }


    public void CloseDialog()
    {
        dialoguePanel.SetActive(false);
        inputField.text = ""; // очищаем поле ввода
        isDialogueActive = false;
    }

    public void OnCloseDialog(InputAction.CallbackContext context)
    {
        CloseDialog();
    }
    public bool IsDialogueActive()
    {
        return isDialogueActive;
    }

    public void OpenChatHistory(GameObject npcStatusItem)
    {
        
        if (lastClickedNpcStatusItem != null)
        {
            lastClickedNpcStatusItem.StopPulse();
        }


        NpcStatusItem npcStatusItemComponent = npcStatusItem.GetComponent<NpcStatusItem>();
        NPC npc = npcStatusItemComponent.connectedNpc;
        string chatHistoryText = npc.GetChatHistory();

        if (chatHistoryText == "")
        {
            chatHistoryText = "...";
        }

        AnalyzeScreenChatHistory.SetText(chatHistoryText);
        AnalyzeScreenNPCName.SetText(npc.characterProfile.npcName);

        lastClickedNpcStatusItem = npcStatusItemComponent;
        lastClickedNpcStatusItem.PlayPulse();
    }
    public void OpenFirstNpcChatHistory()
    {
        // if (lastClickedNpcStatusItem != null)
        // {
        //     lastClickedNpcStatusItem.StopPulse();
        // }

        NPC npc = npcs[0].gameObject.GetComponent<NPC>();
        string chatHistoryText = npc.GetChatHistory();

        if (chatHistoryText == "")
        {
            chatHistoryText = "...";
        }

        AnalyzeScreenChatHistory.SetText(chatHistoryText);
        AnalyzeScreenNPCName.SetText(npc.characterProfile.npcName);

        // lastClickedNpcStatusItem.PlayPulse();
    }


    public void UpdateGameState(GameState newState)
    {
        currentState = newState;

        switch (newState)
        {
            case GameState.MainMenu:
                Time.timeScale = 1f;
                Debug.Log($"GameState: {currentState}");
                menuSceen.SetActive(true);
                tutorialSceen.SetActive(false);
                // lowNpcSceen.SetActive(false);
                endGameScreen.SetActive(false);
                UI.SetActive(false);
                playerAnalyzeScreen.SetActive(false);
                break;
            case GameState.Tutorial:
                Time.timeScale = 0f;
                Debug.Log($"GameState: {currentState}");
                // menuSceen.SetActive(false);
                tutorialSceen.SetActive(true);
                endGameScreen.SetActive(false);
                playerAnalyzeScreen.SetActive(false);
                UI.SetActive(false);

                if (tutorialTypingText != null)
                {
                    tutorialTypingText.StartTyping(); // ✅ правильно
                }
                break;
            case GameState.Playing:
                Time.timeScale = 1f;
                Debug.Log($"GameState: {currentState}");
                UI.SetActive(true);
                menuSceen.SetActive(false);
                tutorialSceen.SetActive(false);
                playerAnalyzeScreen.SetActive(false);
                StartPlayerTurn();
                break;
            case GameState.EnemyTurn:
                Debug.Log($"GameState: {currentState}");
                Debug.Log("Start of enemy turn");
                UI.SetActive(false);
                StartCoroutine(RunEnemyTurnWithFade());
                break;
            case GameState.PlayerAnalyze:
                Debug.Log($"GameState: {currentState}");
                // UI.SetActive(false);
                playerAnalyzeScreen.SetActive(true);
                // OpenFirstNpcChatHistory();
                break;
            case GameState.GameOver:
                Debug.Log($"GameState: {currentState}");
                UI.SetActive(false);
                playerAnalyzeScreen.SetActive(false);
                endGameScreen.SetActive(true);
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
        Tutorial,
        Playing,
        EnemyTurn,
        PlayerAnalyze,
        Paused,
        GameOver,
        Victory,
        Lose
    }

    public GameState CurrentState
    {
        get { return currentState; }
    }

    public void StartPlaying()
    {
        UpdateGameState(GameState.Playing);
        UpdateCurrentScore();
    }
    public void StartGame()
    {
        UpdateGameState(GameState.Tutorial);
    }
    public void CloseLowNpcScren()
    {
        lowNpcSceen.SetActive(false);
    }
    public void EndGame()
    {
        UpdateGameState(GameState.GameOver);

        endGameScreen.SetActive(true);

        // Count alive npcs
        List<GameObject> aliveNonMonsterNpcs = npcs.FindAll(npc => npc != monster);
        // int countAliveNpcs = aliveNonMonsterNpcs.Count;
        int countAliveNpcs = npcs.Count;
        int score;
        endGameScreenAlive.SetText($"Выжившие: {countAliveNpcs}");

        // Check Vin or loose
        bool monsterAlive = npcs.Contains(monster);
        if (monsterAlive)
        {
            score = 0;
            endGameScreenResult.SetText($"Поражение. Кожеход добрался до города");
            endGameScreenResult.color = new Color(49, 49, 49);
            endGameScreenScore.SetText("Счёт: 0");
        }
        else
        {
            score = countAliveNpcs * 10 + countAskedQuestions * -1 + 40;
            endGameScreenResult.SetText($"Победа");
            endGameScreenResult.color = new Color(49, 49, 49);
            endGameScreenScore.SetText("Счёт: " + score.ToString());
        }

        uiMonsterNames.SetText("Имена монстров: " + string.Join(", ", monsterNames));
        UpdateNpcEndScreen();


        // Count asked questions
        endGameScreenQuestionsCount.SetText($"Вопросы: {countAskedQuestions}");

        // Leaderboard
        AddNewEntry(playerNameField.text, score);
        endGameScreenLeaderboard.SetText(LoadLeaderboard().EntriesToString());
    }

    public void UpdateCurrentScore()
    {
        int countAliveNpcs = npcs.Count;
        int score = countAliveNpcs * 10 + countAskedQuestions * -1 + 40;
        uiCurrentScore.SetText(score.ToString());

    }

    // Leaderboard
    private string filePath => Path.Combine(Application.persistentDataPath, "leaderboard.json");

    public void AddNewEntry(string playerName, int score)
    {
        LeaderboardData data = LoadLeaderboard();
        data.entries.Add(new LeaderboardEntry(playerName, score));
        SaveLeaderboard(data);
    }

    private LeaderboardData LoadLeaderboard()
    {
        if (!File.Exists(filePath))
            return new LeaderboardData();

        string json = File.ReadAllText(filePath);
        return JsonUtility.FromJson<LeaderboardData>(json);
    }

    private void SaveLeaderboard(LeaderboardData data)
    {
        string json = JsonUtility.ToJson(data, true); // Pretty print
        File.WriteAllText(filePath, json);
    }



    IEnumerator FadeImage(bool fadeIn, float duration = 1f)
    {
        float start = fadeIn ? 0f : 1f;
        float end = fadeIn ? 1f : 0f;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            float alpha = Mathf.Lerp(start, end, elapsed / duration);
            fadeImg.color = new Color(0, 0, 0, alpha); // черный экран
            elapsed += Time.deltaTime;
            yield return null;
        }

        fadeImg.color = new Color(0, 0, 0, end);
    }

    IEnumerator FadeTextAlpha(TMP_Text text, float from, float to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float alpha = Mathf.Lerp(from, to, elapsed / duration);
            text.color = new Color(text.color.r, text.color.g, text.color.b, alpha);
            elapsed += Time.deltaTime;
            yield return null;
        }
        text.color = new Color(text.color.r, text.color.g, text.color.b, to);
    }


    IEnumerator RunEnemyTurnWithFade()
    {
        yield return StartCoroutine(FadeImage(true)); // черный экран

        // TODO:: place it in right place. Or change enemy turn text?
        if (npcs.Count <= npcCountToKillPlayer + 1)
        {
            // lowNpcSceen.SetActive(true);
            enemyScreenText = "Твой ход, Кожеход... \n Теперь Кожеход может переселиться в тебя";
        }
        else
        {
            enemyScreenText = "Твой ход, Кожеход...";
        }

        // Показать текст
        if (enemyTurnText != null)
        {
            enemyTurnText.text = enemyScreenText;
            yield return StartCoroutine(FadeTextAlpha(enemyTurnText, 0f, 1f, 1f));
        }

        // Задержка перед звуком
        yield return new WaitForSeconds(0.5f);

        // ▶️ Звук
        // if (monsterAttackClip != null && audioSource != null)
        // {
        //     audioSource.PlayOneShot(monsterAttackClip);
        // }

        yield return new WaitForSeconds(1.5f);

        // Удаляем жертву
        MakeEnemyTurn();

        // Скрыть текст
        if (enemyTurnText != null)
        {
            enemyTurnText.text = "";
            enemyTurnText.color = new Color(enemyTurnText.color.r,
            enemyTurnText.color.g,
            enemyTurnText.color.b,
            0); // прозрачный
        }

        // рассвет
        yield return StartCoroutine(FadeImage(false));

        
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