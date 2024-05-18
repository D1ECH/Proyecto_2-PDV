using OpenAI_API;
using OpenAI_API.Chat;
using OpenAI_API.Models;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NPC1Conversacion : MonoBehaviour
{
    private OpenAIAPI api;
    private List<ChatMessage> messages;
    public GameObject jugador;
    // private bool dialogoSoloConHumano = true;
    public bool permiso = false;

    // Referencias a los campos de texto en la interfaz de usuario
    public TMP_InputField inputField;
    public TMP_Text chatText;

    private bool conversacion = true;

    void Start()
    {
        inputField.text = "Presiona K para escribir";
        // AQUI VA LA API
        StartConversation();
        jugador.GetComponent<playerController>().SetStop(true);
    }

    void Update()
    {
        if(conversacion)
        {
            if (Input.GetKeyDown(KeyCode.K))
            {
                inputField.interactable = true; // Activa la interacción con el campo de entrada
                inputField.Select(); // Selecciona el campo de texto para escribir directamente
            }
            if (Input.GetKeyDown(KeyCode.Return))
            {
                EnviarMensaje();
                // GetResponse();
            }
        }
    }

    private void StartConversation()
    {
        messages = new List<ChatMessage>
        {
            new ChatMessage(ChatMessageRole.System, 
            "Tu nombre es GUARDIAN. Eres un guardia de seguridad que protege el acceso a un helicóptero. " +
            "Debes proponer un acertijo" +
            "El jugador necesita acertar el acertijo para poder subir al helicóptero" + 
            "Tendrá 3 oportunidades" + 
            "Si finalmente le permites pasar, mandarás el siguiente mensaje 'Puedes montarte', sino le permites pasar, mandarás el siguiente mensaje 'No puedes montarte'")
        };
    }


    private void EnviarMensaje()
    {
        ChatMessage userMessage = new ChatMessage
        {
            Role = ChatMessageRole.User,
            TextContent = inputField.text
        };

        if (userMessage.TextContent.Length > 200)
        {
            userMessage.TextContent = userMessage.TextContent.Substring(0, 200);
        }
        messages.Add(userMessage);
        // UpdateChatUI(messages);
        UpdateChatUI(messages[messages.Count - 1]);
        RecibirMensaje();
        inputField.text = "";
    }

    private async void RecibirMensaje()
    {
        var chatResult = await api.Chat.CreateChatCompletionAsync(new ChatRequest()
        {
            Model = Model.ChatGPTTurbo_1106,
            Temperature = 0.0,
            MaxTokens = 3000,
            Messages = messages
        });

        ChatMessage responseMessage = new ChatMessage
        {
            Role = chatResult.Choices[0].Message.Role,
            TextContent = chatResult.Choices[0].Message.TextContent
        };
        messages.Add(responseMessage);
        // UpdateChatUI(messages);
        UpdateChatUI(messages[messages.Count - 1]);

        if (responseMessage.TextContent.ToLower().Contains("puedes montarte"))
        {
            permiso = true;
            inputField.gameObject.SetActive(false); // Oculta el campo de entrada
            chatText.gameObject.SetActive(false); // Oculta el texto del chat
            jugador.GetComponent<playerController>().SetStop(false);
            conversacion = false;
        }
        else if (responseMessage.TextContent.ToLower().Contains("no puedes montarte"))
        {
            permiso = false;
            inputField.gameObject.SetActive(false); // Oculta el campo de entrada
            chatText.gameObject.SetActive(false); // Oculta el texto del chat
            jugador.GetComponent<playerController>().SetStop(false);
            conversacion = false;
        }


    }
    /*
    public async void GetResponse()
    {
        if (dialogoSoloConHumano)
        {
            //inputField.interactable = false; // Desactiva la interacción con el campo de entrada
        }

        ChatMessage userMessage = new ChatMessage
        {
            Role = ChatMessageRole.User,
            TextContent = inputField.text
        };

        if (userMessage.TextContent.Length > 200)
        {
            userMessage.TextContent = userMessage.TextContent.Substring(0, 200);
        }
        messages.Add(userMessage);
        UpdateChatUI(messages);

        inputField.text = ""; // Limpia el campo de entrada

        var chatResult = await api.Chat.CreateChatCompletionAsync(new ChatRequest()
        {
            Model = Model.ChatGPTTurbo_1106,
            Temperature = 0.0,
            MaxTokens = 3000,
            Messages = messages
        });

        ChatMessage responseMessage = new ChatMessage
        {
            Role = chatResult.Choices[0].Message.Role,
            TextContent = chatResult.Choices[0].Message.TextContent
        };
        messages.Add(responseMessage);
        UpdateChatUI(messages);

        if (dialogoSoloConHumano && responseMessage.TextContent.Contains("Puedes montarte")) // Si el NPC dice "sí podemos"
        {
            permiso = true;
            // inputField.interactable = true; // Activa la interacción con el campo de entrada
            inputField.show = false;
            chatText.show = false;
        }
    }
    */
    /*
    void OnMouseDown()
    {
        if (!dialogoSoloConHumano)
        {
            // try { inputField.text = jugador.GetComponent<NPC1Conversacion>().Salida; } catch { inputField.text = "Repite la pregunta"; }
        }
        GetResponse();
    }
    */

    // Método para actualizar la interfaz de usuario con los mensajes
    private void UpdateChatUI(ChatMessage message)
    {
        chatText.text = ""; // Borra el contenido anterior del área de texto
        chatText.text += $"{message.Role}: {message.TextContent}\n";

        /*
        foreach (var message in messages)
        {
            // Agrega el mensaje al área de texto
            chatText.text += $"{message.Role}: {message.TextContent}\n";
        }
        */
    }
}
