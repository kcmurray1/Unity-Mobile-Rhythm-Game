using UnityEngine;

public class GameManager: MonoBehaviour
{
    [SerializeField] private SoundManager _soundManager;

    // TODO: Replace this with a list of songs
    [SerializeField] private SongDataScriptableObject song;

    [SerializeField] private GameBoard _gameBoard;

    void Awake()
    {
        _PlayGame(song);
    }

    
    private void _PlayGame(SongDataScriptableObject song)
    {
        _BuildGameBoard(song);
        _soundManager.SetGameSong(song.SongClip);
    }

    // Create GameBoard for song
    private void _BuildGameBoard(SongDataScriptableObject song)
    {
        _gameBoard.Initialize(5, LaneSpacing.Small, song, _soundManager, true);
    }
}