using TMPro;
using UnityEngine;
using System;

public enum Difficulty
{
    Easy,
    Medium,
    Hard
}
public class GameManager: MonoBehaviour
{
    [SerializeField] private SoundManager _soundManager;

    // TODO: Replace this with a list of songs
    [SerializeField] private SongDataScriptableObject song;

    [SerializeField] private GameBoard _gameBoard;

    void Awake()
    {
        _PlayGame(song);
        Console.WriteLine("STARTING");
        Debug.Log("STARTING - LOG");
    }

    
    private void _PlayGame(SongDataScriptableObject song)
    {
        // Display song name
        TextMeshProUGUI _displaySongName = GameObject.Find("Text_Song_Name").GetComponent<TextMeshProUGUI>();
        _displaySongName.text = song.name;
        _BuildGameBoard(song);
        _soundManager.SetGameSong(song.SongClip);
    }

    // Create GameBoard for song
    private void _BuildGameBoard(SongDataScriptableObject song)
    {
        _gameBoard.Initialize(5, LaneSpacing.Small, song, _soundManager, true);
    }
}