using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="CoinTapAudioData",menuName ="IngameAudios")]
public class AudioCollection : ScriptableObject
{
    public List<AudioData> audioData = new List<AudioData>(); 
}
[System.Serializable]
public class AudioData

{
    public AudioClip clip;
    public Audioenum audioName;
    [Range(0,1)]
    public float volume;
    [Range(0,3)]
    public float pitch;
    public bool Ismute;
}
public enum Audioenum
{
    GameBgm,
    ButtonTap,
    ToggleSound,
    CoinTap,

}
