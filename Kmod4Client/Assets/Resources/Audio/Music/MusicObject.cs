using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New MusicObject", menuName = "Music/Music Object")]
public class MusicObject : ScriptableObject
{
    public new string name;
    public AudioClip introClip;
    public AudioClip loopClip;
}
