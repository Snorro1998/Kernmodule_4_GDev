using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    private AudioSource musicIntroPlayer;
    private AudioSource musicLoopPlayer;
    private AudioSource soundAmbientPlayer;
    private AudioSource soundPlayer;

    public List<MusicObject> music = new List<MusicObject>();
    public List<AudioClip> ambientSounds = new List<AudioClip>();
    public List<AudioClip> sounds = new List<AudioClip>();

    private MusicObject currentMusic;

    protected override void Awake()
    {
        base.Awake();
        musicIntroPlayer = gameObject.AddComponent<AudioSource>();
        musicLoopPlayer = gameObject.AddComponent<AudioSource>();
        musicLoopPlayer.loop = true;
        soundAmbientPlayer = gameObject.AddComponent<AudioSource>();
        soundAmbientPlayer.loop = true;
        soundPlayer = gameObject.AddComponent<AudioSource>();
    }

    private AudioClip GetSound(ref List<AudioClip> list, string name)
    {
        foreach (var i in list)
        {
            if (i.name == name)
            {
                return i;
            }
        }
        return null;
    }

    public void PlaySound(string name)
    {
        var snd = GetSound(ref sounds, name);
        if (snd != null) soundPlayer.PlayOneShot(snd);
    }

    public void PlayAmbientSound(string name)
    {
        var snd = GetSound(ref ambientSounds, name);
        if (snd != null)
        {
            soundAmbientPlayer.Stop();
            soundAmbientPlayer.clip = snd;
            soundAmbientPlayer.Play();
        }
    }

    

    public MusicObject GetMusicByName(string name)
    {
        foreach (var i in music)
        {
            if (i.name == name) return i;
        }
        return null;
    }

    public bool SameMusic(string name)
    {
        return GetMusicByName(name) == currentMusic;
    }

    public bool SameMusic(ActiveScreen newScreen)
    {
        return GetMusicByName(GetMusicByScene(newScreen)) == currentMusic;
    }

    public string GetMusicByScene(ActiveScreen newScreen)
    {
        switch (newScreen)
        {
            case ActiveScreen.GAME_MAZE_SCREEN:
                return "Forest";
                //break;
            case ActiveScreen.GAME_BATTLE_SCREEN:
                return "Battle";
                //break;

        }
        return null;
    }
    /*
    public AudioClip GetMusicForScene(ActiveScreen newScreen)
    {
        AudioClip val = null;
        switch (newScreen)
        {
            case ActiveScreen.GAME_MAZE_SCREEN:
                music = "Forest";
                break;
            case ActiveScreen.GAME_BATTLE_SCREEN:
                music = "Battle";
                break;

        }
        return val;
    }
    */

    public void PlayMusic(string name)
    {
        if (name == currentMusic?.name) return;
        foreach (var i in music)
        {
            if (i.name == name)
            {
                StopMusic();
                currentMusic = i;
                musicIntroPlayer.clip = currentMusic.introClip;
                musicLoopPlayer.clip = currentMusic.loopClip;
                musicIntroPlayer.Play();
                musicLoopPlayer.PlayDelayed(musicIntroPlayer.clip.length);
                return;
            }
        }
    }

    public void PlayMusicOnSceneChange(ActiveScreen newScreen)
    {
        string music = "";
        switch (newScreen)
        {
            case ActiveScreen.GAME_MAZE_SCREEN:
                music = "Forest";
                break;
            case ActiveScreen.GAME_BATTLE_SCREEN:
                music = "Battle";
                break;

        }
        PlayMusic(music);
    }

    public void PlayAmbientSoundOnSceneChange(ActiveScreen newScreen)
    {
        string snd = "";
        switch (newScreen)
        {
            case ActiveScreen.GAME_MAZE_SCREEN:
                snd = "AmbForest";
                break;
        }
        PlayAmbientSound(snd);
    }

    public void StopMusic()
    {
        musicIntroPlayer.Stop();
        musicLoopPlayer.Stop();
        soundAmbientPlayer.Stop();
    }
}
