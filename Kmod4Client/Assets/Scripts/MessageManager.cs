using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

public static class MessageManager
{
    public static void SendMessage(NetworkDriver driver, Message message, NetworkConnection connection)
    {
        var writer = driver.BeginSend(connection);
        message.Sending(ref writer);
        driver.EndSend(writer);
    }

    public static Message ReadMessage<T>(DataStreamReader reader) where T : Message, new()
    {
        T message = new T();
        message.Receiving(ref reader);
        return message;
    }
}
