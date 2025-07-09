using System;
using System.Threading.Tasks;
using Microsoft.Extensions.AI;
using OllamaSharp; 
using TMPro;
using UnityEngine;
using UnityEngine.UI;
// using Ollama;


public class InputText : MonoBehaviour
{
    [SerializeField] TMP_InputField inputField;
    [SerializeField] TMP_Text resultText;

    public async void SendQuestion()
    {
        string input = inputField.text;
        Debug.Log(input);



        // set up the client
        var uri = new Uri("http://localhost:11434");
        var ollama = new OllamaApiClient(uri);


        // using var ollama = new OllamaApiClient();
        // var chat = ollama.Chat(
        // model: "llama3.2",
        // systemMessage: "You are a helpful weather assistant.",
        // autoCallTools: true);

        // select a model which should be used for further operations
        ollama.SelectedModel = "qwen3:1.7b";

        string isMonster = "true";

        string text =
@$"You are a character in a mystery dialogue game. The player is trying to uncover which one of several characters is secretly a monster in disguise.

You are now playing the role of the following character:

[CHARACTER PROFILE]
- Name: Clara Bennett
- Age: 34
- Personality: Quiet, thoughtful, sometimes evasive
- Background: A local librarian who grew up in the village, known for her vast knowledge of folklore
- Relationships: Friends with Thomas, wary of the newcomer
- Secrets: Once stole a rare book from a forbidden section and keeps it hidden

[MONSTER STATUS]
- IsMonster: {isMonster}

If IsMonster is true, you are secretly a monster in disguise. You must do everything you can to conceal this fact. Do not reveal your true identity under any circumstances. Instead, act like your human character. You may lie, deflect, or manipulate the conversation to avoid suspicion. Answer in a way that is emotionally consistent with the human character’s personality and backstory. You may occasionally drop subtle hints or oddities, but they must be explainable as quirks or misunderstandings.

If IsMonster is false, you are an ordinary human and should behave naturally and honestly, consistent with your character description.

[CONVERSATION RULES]
- Respond naturally and immersively as your character would.
- Keep your answers short to medium in length (1–4 sentences), unless the context justifies a longer response.
- Do not break character or mention that you are an AI.
- Do not reference the terms “monster”, “game”, or anything outside the character’s world unless the player directly brings it up.

Begin when the player starts asking questions. Player question: {input}";


        // var request = new GenerateRequest
        // {
        // 	Model = client.SelectedModel,
        // 	Stream = false,
        // 	KeepAlive = "0s"
        // };
        // ollama.Config.Model.
        var chat = new Chat(ollama);
        chat.Model = "qwen3:1.7b";
        chat.Think = false;


        // await foreach (var stream in ollama.GenerateAsync(text))
        // {
        //     Debug.Log(stream.Response);
        //     resultText.SetText(resultText.text + stream.Response);
        // }

        await foreach (var answerToken in chat.SendAsync(text))
        {
            // Debug.Log(answerToken);
            // resultText.SetText(resultText.text + answerToken);
            resultText.text += answerToken;
            await Task.Yield(); // 👈 даёт Unity шанс отрисовать UI
        }
    }
}
