using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Networking.Transport;

static class BattleFunctions
{
    public static void OnBattleStart(DataStreamReader stream, object sender, NetworkConnection connection)
    {
        var client = sender as ClientBehaviour;
        var message = MessageManager.ReadMessage<MessageBattleStart>(stream) as MessageBattleStart;
        BattleUIManager.Instance.CreateNewVisualsForCharacters(message.allMonsters);
        //GameManager.Instance.allMonsters = message.allMonsters;
        //var message = MessageManager.ReadMessage<V10MessageMazeCreate>(stream) as V10MessageMazeCreate;
        //string snd = MazeGenerator.Instance.currentMaze != null ? "Run" : null;
        //MazeGenerator.Instance.currentMaze = message.maze;
        ScreenTransitioner.Instance.ChangeScreen(ActiveScreen.GAME_BATTLE_SCREEN, 1.0f, 1.0f, "StartBattle");
        BattleUIManager.Instance.UpdateTargets();
    }

    public static void OnBattleWin(DataStreamReader stream, object sender, NetworkConnection connection)
    {
        ScreenTransitioner.Instance.BattleToMaze();
        //AudioManager.Instance.StopMusic();
        //AudioManager.Instance.PlaySound("Victory");
        //ScreenTransitioner.Instance.ChangeScreen(ActiveScreen.GAME_MAZE_SCREEN, 1.0f, 1.0f);
    }

    public static void OnBattleGiveTurn(DataStreamReader stream, object sender, NetworkConnection connection)
    {
        var message = MessageManager.ReadMessage<MessageGiveTurn>(stream) as MessageGiveTurn;
        Debug.Log(message.characterName + " is aan de beurt");
        if (message.characterName == ClientBehaviour.Instance.username) BattleUIManager.Instance.ShowBattleUI();
        else BattleUIManager.Instance.HideBattleUI();
    }
}
