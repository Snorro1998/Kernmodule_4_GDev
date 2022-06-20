using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

public class MessageDead : Message
{
    public override GameEvent Type => GameEvent.PLAYER_DIED;
    public string characterName;
    public bool isPlayer;
    public string[] scores;

    public MessageDead()
    {

    }

    public MessageDead(string _characterName, bool _isPlayer, string[] _scores)
    {
        characterName = _characterName;
        isPlayer = _isPlayer;
        scores = _scores;
    }

    public override void Receiving(ref DataStreamReader reader)
    {
        base.Receiving(ref reader);
        characterName = reader.ReadString().ToString();
        isPlayer = reader.ReadByte() == 0 ? false : true;
        int length = reader.ReadInt();
        scores = new string[length];
        for (int i = 0; i < length; i++)
        {
            scores[i] = reader.ReadString().ToString();
        }
        //highScores = reader.ReadString().ToString();
    }

    public override void Sending(ref DataStreamWriter writer)
    {
        base.Sending(ref writer);
        writer.WriteString(characterName);
        writer.WriteByte(isPlayer ? (byte)1 : (byte)0);
        writer.WriteInt(scores.Length);
        foreach (var score in scores)
        {
            writer.WriteString(score);
        }
        //writer.WriteString(highScores);
    }
}
