using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.UI;

public class TestBattleSystem : Singleton<TestBattleSystem>
{
    public int nPlayers = 2;
    public int nMonsters = 2;

    private List<Player> allPlayers = new List<Player>();
    private List<Monster> allMonsters = new List<Monster>();

    private List<Character> characterTurnOrder = new List<Character>();

    System.Random rand = new System.Random();
    private int turnNumber = 0;

    Character currentCharacter;

    public GameObject playerGroupObject;
    public GameObject monsterGroupObject;

    public GameObject monsterTargetPanel;
    public GameObject buttonPrefab;

    public GameObject characterPrefab;


    public enum DisplayButtonMode
    {
        all,
        allAlive,
        players,
        playersAlive,
        enemies,
        enemiesAlive,
    }

    private DisplayButtonMode buttonMode = DisplayButtonMode.all;

    public DisplayButtonMode ButtonMode
    {
        get { return buttonMode; }
        set
        {
            buttonMode = value;
            UpdateButtonVisibility();
        }
    }

    private void UpdateButtonVisibility()
    {
        foreach(Character c in characterTurnOrder)
        {
            var button = c.displayObject.button;
            button.interactable = true;
            switch (ButtonMode)
            {
                case DisplayButtonMode.all:
                    button.gameObject.SetActive(true);
                    break;
                case DisplayButtonMode.allAlive:
                    button.gameObject.SetActive(true);
                    button.interactable = c.IsAlive();
                    break;
                case DisplayButtonMode.players:
                    button.gameObject.SetActive(c.displayObject.character.GetType() == typeof(Player));
                    break;
                case DisplayButtonMode.playersAlive:
                    button.gameObject.SetActive(c.displayObject.character.GetType() == typeof(Player));
                    button.interactable = c.IsAlive();
                    break;
                case DisplayButtonMode.enemies:
                    button.gameObject.SetActive(c.displayObject.character.GetType() == typeof(Monster));
                    break;
                case DisplayButtonMode.enemiesAlive:
                    button.gameObject.SetActive(c.displayObject.character.GetType() == typeof(Monster));
                    button.interactable = c.IsAlive();
                    break;
            }
        }
    }

    public class Character : IDisposable
    {
        public Character(string _name, int _hp, CharacterDisplay _displayObject)
        {
            name = _name;
            hp = _hp;
            displayObject = _displayObject;
        }

        public string name;
        private int hp = 10;
        public int attack = 3;
        public int defense = 1;
        public CharacterDisplay displayObject;

        public void ReceiveDamage(int amount)
        { 
            Hp -= Math.Max(1, amount - defense);
        }

        public int Hp 
        {
            get { return hp; }
            set
            {
                var oldHP = hp;
                hp = value;
                displayObject.PerformAnimations(oldHP);
            }
        }

        public bool IsAlive()
        {
            return hp > 0;
        }
        

        //private bool isDisposed = false;

        ~Character()
        {
            Dispose(false);
        }

        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                Destroy(displayObject.gameObject);
                // Code to dispose the managed resources of the class
            }
            // Code to dispose the un-managed resources of the class
            //isDisposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }

    class Player : Character
    {
        public Player(string _name, int _hp, CharacterDisplay _displayObject) : base(_name, _hp, _displayObject)
        {

        }
    }

    public class Monster : Character
    {
        public Monster(string _name, int _hp, CharacterDisplay _displayObject) : base (_name, _hp, _displayObject)
        {

        }
    }

    public T ChooseRandomFromList<T>(List<T> list)
    {
        T elem = default;
        if (list.Count > 0)
        {
            int index = rand.Next(0, list.Count);
            elem = list[index];
        }
        return elem;
    }

    public void ShuffleList<T>(ref List<T> list)
    {
        int n = list.Count;

        for (int i = 0; i < n; i++)
        {
            var elem = ChooseRandomFromList(list);
            list.Remove(elem);
            list.Add(elem);
        }
    }

    private void AppendList<T, K>(ref List<K> orig, ref List<T> target)
    {
        for (int i = 0; i < orig.Count; i++)
        {
            var elem = orig[i];
            target.Add((T)(object)elem);
        }
    }

    private void InitPlayers()
    {
        for (int i = 0; i < nPlayers; i++)
        {
            var visualObject = Instantiate(characterPrefab, playerGroupObject.transform);
            var chardisp = visualObject.GetComponent<CharacterDisplay>();
            var player = new Player("player" + i, 10, chardisp);
            chardisp.character = player;
            chardisp.maxHP = player.Hp;
            allPlayers.Add(player);
            characterTurnOrder.Add(player);
        }
    }

    private void InitMonsters()
    {
        for (int i = 0; i < nMonsters; i++)
        {
            var visualObject = Instantiate(characterPrefab, monsterGroupObject.transform);
            var chardisp = visualObject.GetComponent<CharacterDisplay>();
            var monster = new Monster("monster" + i, 10, chardisp);
            chardisp.character = monster;
            chardisp.maxHP = monster.Hp;
            allMonsters.Add(monster);
            characterTurnOrder.Add(monster);
        }
    }

    private static bool AllDeadCharacterType<T>(List <T> list) where T: Character
    {
        foreach (Character c in list)
        {
            if (c.IsAlive())
            {
                return false;
            }
        }

        return true;
    }

    private bool AllPlayersDead()
    {
        return AllDeadCharacterType(allPlayers);// allPlayers.Cast<Character>().ToList());
    }

    private bool AllMonstersDead()
    {
        return AllDeadCharacterType(allMonsters);// allMonsters.Cast<Character>().ToList());
    }

    private void Start()
    {
        StartCoroutine(StartNewRound(false));
    }

    private void RemovePlayers()
    {
        for (int i = allPlayers.Count - 1; i >= 0; i--)
        {
            allPlayers[i].Dispose();
            allPlayers.RemoveAt(i);
        }
    }

    private void RemoveAllOfType<T>(List<T> list) where T : Character
    {
        for (int i = characterTurnOrder.Count -1; i >= 0; i--)
        {
            var c = characterTurnOrder[i];
            if (c.GetType() == typeof(T))
            {
                c.Dispose();
                characterTurnOrder.RemoveAt(i);
            }
        }
        list.Clear();
    }

    IEnumerator StartNewRound(bool keepPlayers)
    {
        if (!keepPlayers)
        {
            RemoveAllOfType(allPlayers);
            InitPlayers();
        }

        RemoveAllOfType(allMonsters);
        InitMonsters();

        ShuffleList(ref characterTurnOrder);
        yield return new WaitForSeconds(1);
        NextTurn();
        yield return 0;
    }

    private Character ChooseRandomCharacterType<T>(List<T> list) where T: Character
    {
        List<T> aliveCharacters = new List<T>();
        foreach (T p in list)
        {
            if (p.IsAlive())
            {
                aliveCharacters.Add(p);
            }
        }
        int i = rand.Next(0, aliveCharacters.Count);
        return aliveCharacters[i];
    }

    private Character ChooseRandomPlayer()
    {
        return ChooseRandomCharacterType(allPlayers);
    }

    private Character ChooseRandomMonster()
    {
        return ChooseRandomCharacterType(allMonsters);
    }

    public void Attack(Character target)
    {
        Debug.Log(currentCharacter.name + " valt " + target.name + " aan!");
        target.ReceiveDamage(currentCharacter.attack);
    }

    private void PlayerTurn()
    {
        ButtonMode = DisplayButtonMode.enemiesAlive;
        monsterTargetPanel.SetActive(true);
    }

    private void MonsterTurn()
    {
        Attack(ChooseRandomPlayer());
    }

    public void NextTurn()
    {
        if (AllMonstersDead())
        {
            Debug.Log("Alle monsters zijn dood");
            StartCoroutine(StartNewRound(true));
            return;
        }

        if (AllPlayersDead())
        {
            Debug.Log("Alle speler zijn dood");
            return;
        }

        turnNumber++;
        if (turnNumber >= characterTurnOrder.Count) turnNumber = 0;
        currentCharacter = characterTurnOrder[turnNumber];
         
        if (currentCharacter.IsAlive())
        {
            Debug.Log(currentCharacter.name + " is aan de beurt");
            if (currentCharacter.GetType() == typeof(Player))
            {
                PlayerTurn();
            }

            else if (currentCharacter.GetType() == typeof(Monster))
            {
                MonsterTurn();
            }
        }
        else
        {
            NextTurn();
        }              
    }
}
