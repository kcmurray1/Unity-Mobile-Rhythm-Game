using UnityEngine;
using System;
using System.Collections;
using NUnit.Framework.Internal;

public class SoundManager : MonoBehaviour
{
    // General sound(bgm + effects)
    [SerializeField] private AudioSource _generalSoundSource;


    // Song to be played in the game
    [SerializeField] private AudioSource _gameSong;


    [SerializeField] private AudioSource _TuneSound;

    [SerializeField] private BeatManager _beatManager;


    // Start is called before the first frame update
    void Start()
    {
        _beatManager = GameObject.FindAnyObjectByType<BeatManager>();

        _beatManager.OnQuarterBeat += test;
    }

    private void test()
    {
        if(!_gameSong.isPlaying)
            _gameSong.Play();
    }

    public void SetGameSong(AudioClip songClip)
    {
        if (songClip != null)
        {
             _gameSong.clip = songClip;
        }
        _gameSong.clip.LoadAudioData();
        _generalSoundSource.clip.LoadAudioData();
    }

    // Play the game song if it's not playing
    // Otherwise, stop playing
    public void ToggleGameSong(string state)
    {
        if (!_gameSong.isPlaying && state == "start")
        {
            // StartCoroutine(_Tune());
            _gameSong.Play();
        } 
        if (state == "end")
        {
            _gameSong.Stop();
        }  
    }

    public void PlayEffect()
    {
        if (!_generalSoundSource) {return;}
        _generalSoundSource.Play();
   
    }

    public event Action OnQuarterNote;

    private IEnumerator _Tune()
    {
        int bpm = 95;
        float qtrNote = 60f / bpm; 
        while(true)
        {
            yield return new WaitForSeconds(qtrNote);
            OnQuarterNote?.Invoke();
            _TuneSound.Play(delay: 50);
         
        }
    }
}
