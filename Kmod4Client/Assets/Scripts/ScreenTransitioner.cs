using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ActiveScreen
{
    LOGIN_SCREEN,
    GAME_WAIT_SCREEN,
    GAME_MAZE_SCREEN,
    GAME_BATTLE_SCREEN,
    GAME_OVER_SCREEN,
}
/*
public class AnimType
{
    public IEnumerator startEnum;
    public IEnumerator endEnum;
}

public class AnimFade : AnimType
{
    public new IEnumerator startEnum;
}

*/
public class ScreenTransitioner : Singleton<ScreenTransitioner>
{
    public Image fadeScreen;
    public Text fadeScreenText;

    public GameObject screenLogin;
    public GameObject screenGameWait;
    public GameObject screenGameMaze;
    public GameObject screenGameBattle;
    public GameObject screenGameOver;

    public Text scoreText;

    static Dictionary<ActiveScreen, GameObject> screens = new Dictionary<ActiveScreen, GameObject>() { };

    private void InitScreens()
    {
        screens.Add(ActiveScreen.LOGIN_SCREEN, screenLogin);
        screens.Add(ActiveScreen.GAME_WAIT_SCREEN, screenGameWait);
        screens.Add(ActiveScreen.GAME_MAZE_SCREEN, screenGameMaze);
        screens.Add(ActiveScreen.GAME_BATTLE_SCREEN, screenGameBattle);
        screens.Add(ActiveScreen.GAME_OVER_SCREEN, screenGameOver);
    }
    /*
    public IEnumerator DoTransition(AnimType anim)
    {
        yield return StartCoroutine(anim.startEnum);
        yield return StartCoroutine(anim.endEnum);
        yield return 0;
    }
    */

    public void ShowScores(string[] scores)
    {
        string scoreString = "";
        foreach (var score in scores)
        {
            Debug.Log(score);
            
            if (score != "")
            {
                var ding = score.Split(',');
                int scorevalue = int.Parse(ding[1]);
                string playername = ding[0];

                scoreString += scorevalue;
                scoreString += ": ";
                scoreString += playername;
                scoreString += "\n";
                //int scorevalue = 0;
                //int.TryParse(ding[0], out scorevalue);
                /*
                string playerName = ding[1];
                scoreString += ding[0];// += ": " += score[1];
                scoreString += ": ";
                scoreString += score[1];
                scoreString += "/n";
                */
            }
        }

        scoreText.text = scoreString;
    }

    public void UpdateVisibleScreen(ActiveScreen screen)
    {
        
        //Debug.Log("changescreen");
        foreach (var s in screens)
        {
            s.Value.SetActive(false);
        }

        if (screens.ContainsKey(screen))
        {
            screens[screen].SetActive(true);
        }
        /*
        if (screens.ContainsKey(screen))
        {
            screens[screen].transform.SetAsLastSibling();
        }*/
    }

    private IEnumerator Fade(bool fadeOut, float fadeTime)
    {
        if (fadeOut) fadeScreen.gameObject.SetActive(true);
        float i = 0;
        while (i < 1.0f)
        {
            i = Mathf.Min(i + Time.deltaTime, 1.0f);
            var col = fadeScreen.color;
            col.a = fadeOut ? i : 1.0f - i;
            fadeScreen.color = col;
            i += Time.deltaTime / fadeTime;
            yield return new WaitForEndOfFrame();
        }
        if (!fadeOut) fadeScreen.gameObject.SetActive(false);
        yield return 0;
    }

    public IEnumerator Transition(ActiveScreen newScreen, float fadeTime, float waitTime)
    {
        bool sameMusic = AudioManager.Instance.SameMusic(newScreen);
        //bool sameAmbient = AudioManager.Instance.SameMusic(newScreen);
        if (!sameMusic) AudioManager.Instance.StopMusic();
        fadeScreenText.gameObject.SetActive(false);
        if (fadeTime > 0) yield return StartCoroutine(Fade(true, fadeTime));
        if (newScreen == ActiveScreen.GAME_MAZE_SCREEN)
        {
            fadeScreenText.text = MazeGenerator.Instance.currentMaze.name;
            MazeGenerator.Instance.CreateMazeVisuals();
        }
        else
        {
            fadeScreenText.text = "";
        }
        fadeScreenText.gameObject.SetActive(true);
        yield return new WaitForSeconds(waitTime);
        UpdateVisibleScreen(newScreen);
        if (fadeTime > 0)  yield return StartCoroutine(Fade(false, fadeTime));
        AudioManager.Instance.PlayMusicOnSceneChange(newScreen);
        AudioManager.Instance.PlayAmbientSoundOnSceneChange(newScreen);
        yield return 0;
    }

    public void ChangeScreen(ActiveScreen newScreen, float fadeTime, float waitTime)
    {
        ChangeScreen(newScreen, fadeTime, waitTime, null);
        //StartCoroutine(Transition(newScreen, fadeTime, waitTime));
        //AudioManager.Instance.PlayMusicOnSceneChange(newScreen);
        //AudioManager.Instance.PlayAmbientSoundOnSceneChange(newScreen);
    }

    public void ChangeScreen(ActiveScreen newScreen, float fadeTime, float waitTime, string snd)
    {
        StartCoroutine(Transition(newScreen, fadeTime, waitTime));
        if (snd != null) AudioManager.Instance.PlaySound(snd);
        //AudioManager.Instance.PlayMusicOnSceneChange(newScreen);
        //AudioManager.Instance.PlayAmbientSoundOnSceneChange(newScreen);
    }

    IEnumerator BattleToMazeRoutine()
    {
        AudioManager.Instance.StopMusic();
        AudioManager.Instance.PlaySound("Victory");
        yield return new WaitForSeconds(5.0f);
        ScreenTransitioner.Instance.ChangeScreen(ActiveScreen.GAME_MAZE_SCREEN, 1.0f, 1.0f);
        yield return 0;
    }

    public void BattleToMaze()
    {
        StartCoroutine(BattleToMazeRoutine());
    }

    protected override void Awake()
    {
        base.Awake();
        InitScreens();       
    }

    private void Start()
    {
        ChangeScreen(ActiveScreen.LOGIN_SCREEN, 0, 0);
    }
}
