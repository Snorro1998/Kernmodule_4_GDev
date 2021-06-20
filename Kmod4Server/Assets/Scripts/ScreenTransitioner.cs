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
}

public class ScreenTransitioner : Singleton<ScreenTransitioner>
{
    public Image fadeScreen;
    public Text fadeScreenText;

    public GameObject screenLogin;
    public GameObject screenGameWait;
    public GameObject screenGameMaze;
    public GameObject screenGameBattle;

    static Dictionary<ActiveScreen, GameObject> screens = new Dictionary<ActiveScreen, GameObject>() { };

    private void InitScreens()
    {
        screens.Add(ActiveScreen.LOGIN_SCREEN, screenLogin);
        screens.Add(ActiveScreen.GAME_WAIT_SCREEN, screenGameWait);
        screens.Add(ActiveScreen.GAME_MAZE_SCREEN, screenGameMaze);
        screens.Add(ActiveScreen.GAME_BATTLE_SCREEN, screenGameBattle);
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
        fadeScreenText.gameObject.SetActive(false);
        if (fadeTime > 0) yield return StartCoroutine(Fade(true, fadeTime));
        if (newScreen == ActiveScreen.GAME_MAZE_SCREEN)
        {
            fadeScreenText.text = MazeGenerator.Instance.currentMaze.name;  
            //MazeGenerator.Instance.CreateMazeVisuals();
        }
        fadeScreenText.gameObject.SetActive(true);
        yield return new WaitForSeconds(waitTime);
        UpdateVisibleScreen(newScreen);
        if (fadeTime > 0)  yield return StartCoroutine(Fade(false, fadeTime));
        //AudioManager.Instance.PlayMusicOnSceneChange(newScreen);
        //AudioManager.Instance.PlayAmbientSoundOnSceneChange(newScreen);
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
        //if (snd != null) AudioManager.Instance.PlaySound(snd);
        //AudioManager.Instance.PlayMusicOnSceneChange(newScreen);
        //AudioManager.Instance.PlayAmbientSoundOnSceneChange(newScreen);
    }

    protected override void Awake()
    {
        base.Awake();
        InitScreens();
        ChangeScreen(ActiveScreen.LOGIN_SCREEN, 0, 0);
    }
}
