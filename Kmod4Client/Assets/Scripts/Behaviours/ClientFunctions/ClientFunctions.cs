using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Networking.Transport;

public static class ClientFunctions
{
    public static void OnPing(DataStreamReader stream, object sender, NetworkConnection connection)
    {
        var client = sender as ClientBehaviour;
        var message = new MessagePing();
        MessageManager.SendMessage(client.networkDriver, message, connection);
    }

    public static void OnServerQuit(DataStreamReader stream, object sender, NetworkConnection connection)
    {
        var client = sender as ClientBehaviour;
        Debug.Log("Server is gestopt");
        client.DisconnectFromServer();
        //client.networkConnection = default(NetworkConnection);
    }
}
