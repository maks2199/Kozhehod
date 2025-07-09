// using System;
// using Microsoft.Extensions.AI;
// using OllamaSharp; 
// using TMPro;
// using UnityEngine;
// using UnityEngine.UI;
// // using Ollama;


// public class InputText : MonoBehaviour
// {
//     [SerializeField] TMP_InputField inputField;
//     [SerializeField] TMP_Text resultText;

//     public async void SendQuestion()
//     {
//         string input = inputField.text;
//         Debug.Log(input);



//         // set up the client
//         var uri = new Uri("http://localhost:11434");
//         var ollama = new OllamaApiClient(uri);


//         // using var ollama = new OllamaApiClient();
//         // var chat = ollama.Chat(
//         // model: "llama3.2",
//         // systemMessage: "You are a helpful weather assistant.",
//         // autoCallTools: true);

//         // select a model which should be used for further operations
//         ollama.SelectedModel = "qwen3:1.7b";


//         string text =
//             @"You are a crazy paranoidal scientist, annswer corrding it.
//             We are playing a visual novel game. 
//             You are a monster. You are on the sapce ship. 
//             Player asks you a question to understand if you are a human or not. 
//             Answer his question and try to hide that you are a monster. Player question: " + input;


//         // var request = new GenerateRequest
//         // {
//         // 	Model = client.SelectedModel,
//         // 	Stream = false,
//         // 	KeepAlive = "0s"
//         // };
//         // ollama.Config.Model.
//         var chat = new Chat(ollama);
//         chat.Model = "qwen3:1.7b";
//         chat.Think = false;


//         // await foreach (var stream in ollama.GenerateAsync(text))
//         // {
//         //     Debug.Log(stream.Response);
//         //     resultText.SetText(resultText.text + stream.Response);
//         // }

//         await foreach (var answerToken in chat.SendAsync(text))
//         {
//             Debug.Log(answerToken);
//             resultText.SetText(resultText.text + answerToken);
//         }
//     }
// }
