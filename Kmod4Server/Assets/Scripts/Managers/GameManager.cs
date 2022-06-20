using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleStatus
{
    NOT_STARTED,
    STARTED,
    LOSE,
    WIN,
}

public enum TurnStatus
{
    WAITING,
    PERFORMING,
    DONE,
}

public class GameManager : Singleton<GameManager>
{
    public bool gameStarted = false;
    public int floor = 0;

    public List<Player> allPlayers = new List<Player>();
    public List<Monster> allMonsters = new List<Monster>();

    public List<Character> turnOrderList = new List<Character>();
    public Character currentCharacter;

    public int turnNumber = -1;
    public BattleStatus battleStatus = BattleStatus.NOT_STARTED;
    public TurnStatus turnStatus = TurnStatus.WAITING;

    //public GameObject enemyTargetPanel;
    public string selectedActionName;

    public void DamageCharacter(Character target, uint amount)
    {
        var oldHp = target.currentHp;
        var disp = BattleUIManager.Instance.GetCharacterDisplayObject(target);
        target.Damage(amount);
        disp.PerformAnimations(oldHp);
    }

    public void SetCurrentAction(string newAction)
    {
        selectedActionName = newAction;
    }

    public void PerformAction(string target)
    {
        //var message = new MessageUseItem(selectedActionName, 1, target, ClientBehaviour.Instance.username);
        //ClientBehaviour.Instance.SendMessageToServer(message);
        //Debug.Log(ClientBehaviour.Instance.username + " gebruikt " + selectedActionName + " op " + target);
    }

    public void UpdatePlayers()
    {
        foreach (var p in PlayerManager.Instance.players)
        {
            if (!allPlayers.Contains(p))
            {
                allPlayers.Add(p);
                turnOrderList.Add(p);
            }
        }

        for (int i = 0; i < allPlayers.Count; i++)
        {
            if (!PlayerManager.Instance.players.Contains(allPlayers[i]))
            {
                //speler die aan de beurt is ging weg
                bool currentPlayerLeft = currentCharacter == allPlayers[i];
                if (currentPlayerLeft)
                {
                    turnOrderList.Remove(allPlayers[i]);
                    turnNumber--;
                    if (turnNumber < 0) turnNumber = turnOrderList.Count - 1;          
                }
                allPlayers.RemoveAt(i);
                if (currentPlayerLeft) NextTurn();
                i--;
            }
        }
    }

        public Character GetCharacterByName(string name)
        {
            foreach (var m in allMonsters)
            {
                if (m.charName == name) return m; 
            }
            foreach (var p in allPlayers)
            {
                if (p.charName == name) return p;
            }

            return null;
        }

    public bool IsPlayer(string name)
    {
        foreach(var p in allPlayers)
        {
            if (p.charName == name) return true;
        }
        return false;
    }

    private bool AllPlayersDead()
    {
        if (allPlayers.Count == 0) return true;
        foreach (var p in allPlayers)
        {
            if (p.IsAlive()) return false;
        }
        return true;
    }

    private bool AllMonstersDead()
    {
        if (allMonsters.Count == 0) return true;
        foreach (var m in allMonsters)
        {
            if (m.IsAlive()) return false;
        }
        return true;
    }

    public void NextTurn()
    {
        turnStatus = TurnStatus.DONE;
        if (AllPlayersDead()) battleStatus = BattleStatus.LOSE;
        else if (AllMonstersDead()) battleStatus = BattleStatus.WIN;
        else
        {
            turnNumber++;
            if (turnNumber == turnOrderList.Count) turnNumber = 0;
            GiveTurn();
        }       
    }

    public void GiveTurn()
    {
        turnStatus = TurnStatus.WAITING;
        currentCharacter = turnOrderList[turnNumber];
        Debug.Log(currentCharacter.charName + " is aan de beurt");
        var message = new MessageGiveTurn(currentCharacter.charName, IsPlayer(currentCharacter.charName));
        ServerBehaviour.Instance.SendMessageToAllOnline(message);
        if (!IsPlayer(currentCharacter.charName)) MonsterTurn();
    }

    public void MonsterTurn()
    {
        Character c = allPlayers[Random.Range(0, allPlayers.Count)];
        ItemManager.Instance.UseItem("Slash", 1, c.charName, currentCharacter.charName);
    }

    public IEnumerator BattleSym()
    {
        DebugMessages.PrintDebugMessage(DebugMessages.MessageTypes.GAME_BATTLE_START);
        InitMonsters();
        InitTurnOrderList();
        var message = new MessageBattleStart(allMonsters);
        ServerBehaviour.Instance.SendMessageToAllOnline(message);
        
        ScreenTransitioner.Instance.ChangeScreen(ActiveScreen.GAME_BATTLE_SCREEN, 1, 1);
        yield return new WaitForSeconds(2);

        while (battleStatus != BattleStatus.LOSE && battleStatus != BattleStatus.WIN)
        {
            NextTurn();
            while (turnStatus != TurnStatus.DONE)
            {
                yield return null;
            }
            yield return null;
        }
        Debug.Log("Spel geeindigd. Resultaat " + battleStatus);
        if (battleStatus == BattleStatus.WIN)
        {
            var messageWin = new MessageBattleWin();
            ServerBehaviour.Instance.SendMessageToAllOnline(messageWin);
            ScreenTransitioner.Instance.ChangeScreen(ActiveScreen.GAME_MAZE_SCREEN, 1, 1);
        }
        battleStatus = BattleStatus.NOT_STARTED;
        yield return null;
    }

    private void InitTurnOrderList()
    {
        turnOrderList.Clear();
        foreach (var p in allPlayers)
        {
            turnOrderList.Add(p);
        }

        foreach (var m in allMonsters)
        {
            turnOrderList.Add(m);
        }

        Utils.Shuffle(turnOrderList);
    }

    private void InitMonsters()
    {
        //UtilsMono.Instance.DestroyAllChildObjects(ref enemyTargetPanel);
        allMonsters.Clear();
        int nPlayers = allPlayers.Count;
        int nMonsters = 1;//Mathf.Max(1, nPlayers - 1);
        for (int i = 0; i < nMonsters; i++)
        {
            allMonsters.Add(new Monster("Griezel" + i, 10));
        }
        BattleUIManager.Instance.CreateNewVisualsForCharacters(allMonsters);
        BattleUIManager.Instance.UpdateTargets();
    }

    public void StartGame()
    {
        if (!gameStarted)
        {
            gameStarted = true;
            ItemManager.Instance.GiveItem("Grenade", 5);
            ItemManager.Instance.GiveItem("Bomb", 2);
            ScreenTransitioner.Instance.ChangeScreen(ActiveScreen.GAME_MAZE_SCREEN, 1, 1);
            GotoNextMazeFloor();
        }
    }

    public void GotoNextMazeFloor()
    {
        floor++;
        DebugMessages.PrintDebugMessage(DebugMessages.MessageTypes.GAME_MAZE_NEXT_FLOOR);
        MazeGenerator.Instance.InitMaze("Beeldig Bos " + floor + "F");
        var message = new MessageMazeCreate(MazeGenerator.Instance.currentMaze);
        ServerBehaviour.Instance.SendMessageToAllOnline(message);
    }

    public void StartBattle()
    {      
        if (battleStatus == BattleStatus.NOT_STARTED)
        {
            battleStatus = BattleStatus.STARTED;
            StartCoroutine(BattleSym());
        }
    }

    public void WinBattle()
    {
        foreach (var m in allMonsters)
        {
            m.currentHp = 0;
        }
        NextTurn();
    }
}
