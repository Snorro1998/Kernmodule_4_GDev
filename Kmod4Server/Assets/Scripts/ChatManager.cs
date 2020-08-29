using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatManager : V2Singleton<ChatManager>
{
    private KeyCode sendMessageKey = KeyCode.Return;
    private static int maxMessages = 30;

    public GameObject chatPanel, textObject;
    public InputField chatBox;
    private bool chatBoxActive = false;

    public bool server = false;

    [SerializeField]
    List<Message> messageList = new List<Message>();

    [System.Serializable]
    public class Message
    {
        public string text;
        public Text textObject;
    }

    public void SendMessageToChat(string text)
    {
        if (messageList.Count >= maxMessages)
        {
            Destroy(messageList[0].textObject.gameObject);
            messageList.Remove(messageList[0]);
        }

        Message newMessage = new Message();
        newMessage.text = text;
        GameObject newText = Instantiate(textObject, chatPanel.transform);
        newMessage.textObject = newText.GetComponent<Text>();
        newMessage.textObject.text = newMessage.text;

        messageList.Add(newMessage);
    }

    protected override void Awake()
    {
        base.Awake();
    }

    private void Update()
    {
        if (Input.GetKeyDown(sendMessageKey))
        {
            if (!chatBoxActive)
            {
                chatBox.ActivateInputField();
            }
            else
            {
                if (chatBox.text != "")
                {
                    if (server)
                    {
                        SendMessageToChat(chatBox.text);
                        TestServerBehaviour.Instance.SendMessageToClients(new MessageText(chatBox.text));
                        //TestServerBehaviour.Instance.SendTextToChat(chatBox.text);
                        
                        //SendMessageToChat(chatBox.text);
                        //TODO stuur ook naar clients
                    }
                    else
                    {
                        TestClientBehaviour.Instance.SendMessageToServer(chatBox.text);
                        //TestClientBehaviour.Instance.SendTextToServer(chatBox.text);

                        //client stuurt data naar server
                        //ClientBehaviour.Instance.SendText(chatBox.text);                     
                    }
                    
                    chatBox.text = "";
                    //fix die alleen nodig is als de toets ENTER is
                    chatBox.ActivateInputField();
                }
                else
                {
                    chatBox.DeactivateInputField();
                }
            }
        }
        //fix die alleen nodig is als de toets ENTER is
        chatBoxActive = chatBox.isFocused;
    }
}
