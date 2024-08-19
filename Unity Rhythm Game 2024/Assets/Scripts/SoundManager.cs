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

    // Play the game song if it's not playing
    // Otherwise, stop playing
    public void ToggleGameSong(string state)
    {
        if (!_gameSong.isPlaying && state == "start")
        {
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
}
