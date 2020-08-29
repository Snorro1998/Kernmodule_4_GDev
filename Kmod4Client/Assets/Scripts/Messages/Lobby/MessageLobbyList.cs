using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;

//misschien deze class verwijderen en vervangen door messagelobbyinfo
public class MessageLobbyList : Message
{
    public override MessageType Type => MessageType.sendLobbyList;
    public List<Lobby> lobbies = new List<Lobby>();

    public MessageLobbyList()
    {

    }

    public MessageLobbyList(List<Lobby> _lobbies)
    {
        lobbies = _lobbies;
    }

    public override void Sending(ref DataStreamWriter writer)
    {
        base.Sending(ref writer);
        writer.WriteInt(lobbies.Count);
        foreach (Lobby lob in lobbies)
        {
            int varsInMessage = lob.allPlayers.Count;
            writer.WriteInt(varsInMessage);//1
            writer.WriteString(lob.lobbyName);//2
            writer.WriteInt(lob.gameID);//3
            writer.WriteUInt((uint)lob.state);//4
            foreach (Lobby.SinglePlayer player in lob.allPlayers)
            {
                writer.WriteString(player.name);
                writer.WriteInt(player.hp);
            }
        }
    }

    public override void Receiving(ref DataStreamReader reader)
    {
        base.Receiving(ref reader);
        int nLobbies = reader.ReadInt();
        for (int i = 0; i < nLobbies; i++)
        {
            int varsInMessage = reader.ReadInt();//1
            string lobName = reader.ReadString().ToString();//2
            int lobGameID = reader.ReadInt();//3
            Lobby.State lobState = (Lobby.State)reader.ReadUInt();//4
            List<Lobby.SinglePlayer> players = new List<Lobby.SinglePlayer>();
            //Debug.Log("lobbies[" + i + "], varsinmessage = " + varsInMessage);
            
            for (int j = 0; j < varsInMessage; j++)
            {             
                string name = reader.ReadString().ToString();
                int hp = reader.ReadInt();
                //default omdat t erbij moet
                Lobby.SinglePlayer player = new Lobby.SinglePlayer(name, default, hp);
                players.Add(player);
            }
            lobbies.Add(new Lobby(lobName, lobGameID, lobState, players));
        }
    }
}
