using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoSingleton<SoundManager>
{
    private Dictionary<int, AudioClip> bgmDict;
    private Dictionary<int, AudioClip> sfxDict;

    [RuntimeInitializeOnLoadMethod]
    public static void OnLoadSoundData()
    {

    }

    public void PlayBGM()
    {

    }

    public void PlaySFX()
    {

    }


}
