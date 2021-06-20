using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Networking.Transport;

static class LoginLogoutFunctions
{
    /// <summary>
    /// Wordt aangeroepen wanneer een client probeert in te loggen.
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="sender"></param>
    /// <param name="connection"></param>
    public static void OnLoginRequest(DataStreamReader stream, object sender, NetworkConnection connection)
    {
        ServerBehaviour server = sender as ServerBehaviour;
        var message = MessageManager.ReadMessage<MessageLoginRequest>(stream) as MessageLoginRequest;
        server.OnloginRequestFunction(message.name, message.password, connection);
        return;

        DebugMessages.PrintDebugMessage(DebugMessages.MessageTypes.CLIENT_LOGIN_REQUEST, message.name, message.password);
        //TODO gegevenscheck
        var result = PlayerManager.Instance.PlayerIsLoggedIn(message.name, connection) == true ? MessageLoginResponse.LoginResult.ALREADY_LOGGED_IN : MessageLoginResponse.LoginResult.SUCCES;
        //TODO niet weigeren als het spel al is gestart
        if (GameManager.Instance.gameStarted) result = MessageLoginResponse.LoginResult.GAME_STARTED;

        if (result == MessageLoginResponse.LoginResult.SUCCES)
        {
            PlayerManager.Instance.LoginPlayer(message.name, connection);
        }

        var response = new MessageLoginResponse(result);
        MessageManager.SendMessage(server.networkDriver, response, connection);
    }

    /// <summary>
    /// Wordt aangeroepen wanneer een client probeert uit te loggen.
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="sender"></param>
    /// <param name="connection"></param>
    public static void OnLogoutRequest(DataStreamReader stream, object sender, NetworkConnection connection)
    {
        ServerBehaviour server = sender as ServerBehaviour;
        PlayerManager.Instance.LogoutPlayer(connection);
        var response = new MessageLogoutResponse();
        MessageManager.SendMessage(server.networkDriver, response, connection);
    }
}
