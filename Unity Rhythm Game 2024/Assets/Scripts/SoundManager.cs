using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    // General sound(bgm + effects)
    [SerializeField] private AudioSource _generalSoundSource;


    // Song to be played in the game
    [SerializeField] private AudioSource _gameSong;
    // Start is called before the first frame update
    void Start()
    {
        // play any default audio
    }

    public void SetGameSong()
    {
        _gameSong.clip.LoadAudioData();
        _generalSoundSource.clip.LoadAudioData();
    }

    public void OnSongStateChange()
    {
        if(_gameSong.isPlaying)
        {
            _gameSong.Stop();
        }
        else
        {
            _gameSong.Play();
        }   
    }

    public void PlayEffect()
    {
        if (!_generalSoundSource) {return;}
        _generalSoundSource.Play();
    }
}
