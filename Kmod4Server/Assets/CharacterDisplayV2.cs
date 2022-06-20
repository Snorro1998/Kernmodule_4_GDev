using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CharacterDisplayV2 : MonoBehaviour
{
    public static float flashInterval = 0.1f;

    public Text nameTextObject;
    public Image img;
    public Slider hpBar;
    public Image hpColorObject;

    public Character character;

    private int oldHP;
    //public int maxHP;

    public Button button;

    public void PerformAnimations(int _oldHP)
    {
        Debug.Log(character.charName + " doet iets");
        oldHP = _oldHP;
        StartCoroutine(UpdateCharacter());   
    }

    private IEnumerator UpdateCharacter()
    {
        if (oldHP != character.currentHp)
        {
            if (oldHP > character.currentHp)
            {
                yield return StartCoroutine(HurtAnimation());
            }
            
            yield return StartCoroutine(UpdateHealthAnimation());
            yield return new WaitForSeconds(1);
          
            if (!character.IsAlive())
            {
                yield return StartCoroutine(DeathFadeAnimation());
                Debug.Log(character.charName + " stierf!");
                
                // upload score
                yield return StartCoroutine(DBManager.OpenURL("insert_score", "username=" + character.charName + "", "game_id=1", "score=" + character.score));
                if (DBManager.response.Contains("ERROR"))
                {
                    Debug.LogError("Couldnt upload highscore for player " + character.charName + "!");
                }
                else
                {
                    Debug.Log("Score uploaded!");
                }
                
                yield return new WaitForSeconds(.25f);

                //string HighScores = "";
                string[] scores = new string[1];
                // haal scores op
                yield return StartCoroutine(DBManager.OpenURL("get_scores", "game_id=1", "n_scores=10"));
                if (DBManager.response.Contains("ERROR"))
                {
                    Debug.LogError("Couldnt load highscores!");
                }
                else
                {
                    Debug.Log("Loaded highscores!");
                    Debug.Log(DBManager.response);
                    //HighScores = DBManager.response;
                    scores = DBManager.response.Split('.');
                }

                var message = new MessageDead(character.charName, GameManager.Instance.IsPlayer(character.charName), scores);
                ServerBehaviour.Instance.SendMessageToAllOnline(message);
            }
        }
        //TestBattleSystem.Instance.NextTurn();
        GameManager.Instance.NextTurn();
        yield return 0;
    }

    private IEnumerator HurtAnimation()
    {
        for (int i = 0; i < 10; i++)
        {
            img.enabled = !img.enabled;
            yield return new WaitForSeconds(flashInterval);
        }
        img.enabled = true;
        yield return 0;
    }

    private IEnumerator UpdateHealthAnimation()
    {      
        float oldValue = oldHP;
        float newValue = character.currentHp;

        if (oldValue > newValue)
        {
            while (oldValue > newValue)
            {
                oldValue = Mathf.Max(newValue, oldValue - 2 * Time.deltaTime);
                hpBar.value = Mathf.InverseLerp(0, character.statMaxHealth, oldValue);
                UpdateHPBarColor();
                yield return new WaitForEndOfFrame();
            }
        }
        else
        {
            while (oldValue < newValue)
            {
                oldValue = Mathf.Min(newValue, oldValue + 2 * Time.deltaTime);
                hpBar.value = Mathf.InverseLerp(0, character.statMaxHealth, oldValue);
                UpdateHPBarColor();
                yield return new WaitForEndOfFrame();
            }
        }
        yield return 0;
    }

    private IEnumerator DeathFadeAnimation()
    {
        var tmpColor = img.color;

        float i = 1;
        while (i > 0.5f)
        {
            i = Mathf.Max(0.5f, i - Time.deltaTime);
            tmpColor.a = i;
            img.color = tmpColor;
            yield return new WaitForEndOfFrame();
        }      
        yield return 0;
    }

    private void UpdateHPBarColor()
    {
        var val = hpBar.value;
        var col = Color.green;
        if (val < 0.3f)
        {
            col = Color.red;
        }
        else if (val < 0.6f)
        {
            col = Color.yellow;
        }
        hpColorObject.color = col;
    }

    private void CreateTargetButton()
    {
        img = GetComponent<Image>();
        /*
        var buttonObject = Instantiate(TestBattleSystem.Instance.buttonPrefab, TestBattleSystem.Instance.monsterTargetPanel.transform);
        buttonObject.transform.GetChild(0).GetComponent<Text>().text = character.name;
        button = buttonObject.GetComponent<Button>();
        button.onClick.AddListener(() => TestBattleSystem.Instance.Attack(character));
        button.onClick.AddListener(() => TestBattleSystem.Instance.monsterTargetPanel.SetActive(false));
        */
    }

    private void Start()
    {
        nameTextObject.text = character.charName;        
        hpBar.value = Mathf.InverseLerp(0, character.statMaxHealth, character.currentHp);
        UpdateHPBarColor();
        /*
        CreateTargetButton();
        */
    }

    private void OnDestroy()
    {   
        if (button != null)
        {
            Destroy(button.gameObject);
        }
    }
}
