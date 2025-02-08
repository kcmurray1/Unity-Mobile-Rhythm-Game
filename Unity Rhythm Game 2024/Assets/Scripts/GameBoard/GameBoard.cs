using System;
using System.Collections.Generic;
using UnityEngine;

 public enum LaneSpacing
 {
    Small,
    Medium,
    Large
 }
/// <summary>
/// Class <c>GameBoard</c> creates a 3-5 lane version of the game 
/// </summary>
public class GameBoard : MonoBehaviour
{
    // Lanes and Judgement Buttons
    [SerializeField] private GameObject LaneObject;

    [SerializeField] private GameObject JudgementButtonObject;

    [SerializeField] private Transform JudgementButtonTransform;

    // Managers
    private TouchManager _touchManager;
    public ScoreManager scoreManager;
    private SoundManager _soundManager;
    [SerializeField] private GameObject _touchManagerPrefab;
    [SerializeField] private GameObject _scoreManagerPrefab;
    [SerializeField] private GameObject _soundManagerObject;

    // NoteSpawner
    private NoteSpawner _noteSpawner;
    [SerializeField] private GameObject _noteSpawnerObject;
    
    // Center of board
    private Vector3 _center;

    // Spacing around the center
    private int _centerHorizOffset;

    private float _laneHorizSpacing;
    // Number of lanes
    [Range(3, 5)]
    [SerializeField] private int _numLanes;
    private bool _hasEvenLanes;
    private List<JudgementButton> _judgementButtons;

    // Board Design
    private const float LARGE_LANE_SPACING = 4f;
    private const float MEDIUM_LANE_SPACING = 3.75f;

    private const float SMALL_LANE_SPACING = 3.5f;

    private SongDataScriptableObject _song;

    // Autoplay feature
    bool _isAutoPlay;

    /// <summary>
    /// Set the number of lanes [3-5] and specify how wide the <c>GameBoard</c> should be
    /// </summary>
    /// <param name="numLanes"></param>
    /// <param name="mode">The width of the GameBoard [Small, Medium, Large]</param>
    public void Initialize(int numLanes, LaneSpacing mode, SongDataScriptableObject song, SoundManager soundManager, Action endGame, bool autoPlay=false)
    {
        _SetLaneSpacing(mode);
        _numLanes = numLanes;
        _isAutoPlay = autoPlay;
        _center = new Vector3(0,10,90);
        _hasEvenLanes = false;
        _centerHorizOffset = 0;

        // Adjust lane spacing if number of lanes is even
        if (_numLanes % 2 == 0)
        {
            _centerHorizOffset = 2;
            _hasEvenLanes = true;
        }
        _song = song;
        _soundManager = soundManager;
        _judgementButtons = new List<JudgementButton>();
        // Create Managers
        _CreateManagers(soundManager);
        _BuildBoard(song, endGame);
    }
   

    // Determine how close the lanes should be on the board
    private void _SetLaneSpacing(LaneSpacing mode)
    {
        switch(mode)
        {
            case LaneSpacing.Small:
                _laneHorizSpacing = SMALL_LANE_SPACING;
                break;
            case LaneSpacing.Medium:
                _laneHorizSpacing = MEDIUM_LANE_SPACING;  
                break;
            default:
                _laneHorizSpacing = LARGE_LANE_SPACING;
                break;
        }
    }

    // Instantiate Managers for scoring and interaction
    private void _CreateManagers(SoundManager soundManager)
    {
        // Create TouchManager
        if(_touchManager)
        {
            Destroy(_touchManager);
            _touchManager = null;
        }
        _touchManager = Instantiate(_touchManagerPrefab, gameObject.transform).GetComponent<TouchManager>();
        // Create ScoreManager
        if (scoreManager)
        {
            Destroy(scoreManager);
            scoreManager = null;
        }
        scoreManager = Instantiate(_scoreManagerPrefab, gameObject.transform).GetComponent<ScoreManager>();
        scoreManager.Initialize();
        // Connect to soundManager
        _soundManager = soundManager;
    }

    // Add lanes to the board to fit the layout: <Left lane(s)> <center> <Right lane(s)>
    private List<float> _PlaceLanes()
    {
        List<float> lanePositions = new List<float>();
        // Place evenly spaced lanes
        for(int i = 0; i < Math.Ceiling(_numLanes / 2.0); i++)
        {
            if (i == 0 && !_hasEvenLanes)
            {
                GameObject centerLane = Instantiate(LaneObject, _center, Quaternion.identity);
                JudgementButton center = Instantiate(JudgementButtonObject, JudgementButtonTransform).GetComponent<JudgementButton>();
                center.Initialize(_touchManager, scoreManager, _soundManager, new Vector3(0,0,0), i, _isAutoPlay);
                lanePositions.Add(centerLane.transform.position.x);
                _judgementButtons.Add(center);
                continue;
            }
            Vector3 lanePosition = new Vector3(i * _laneHorizSpacing + _centerHorizOffset, 0, 0);
            // Create Mirrored Lanes   
            GameObject rightLane = Instantiate(LaneObject, _center + lanePosition, Quaternion.identity);
            GameObject leftLane = Instantiate(LaneObject, _center - lanePosition, Quaternion.identity);
            // Create Mirrored Judgement Buttons
            JudgementButton rightButton = Instantiate(JudgementButtonObject, JudgementButtonTransform).GetComponent<JudgementButton>();
            JudgementButton leftButton = Instantiate(JudgementButtonObject, JudgementButtonTransform).GetComponent<JudgementButton>();
            rightButton.Initialize(_touchManager, scoreManager, _soundManager, lanePosition, i, _isAutoPlay);
            leftButton.Initialize(_touchManager, scoreManager, _soundManager, lanePosition * Vector2.left, -i, _isAutoPlay);
            // Record location of lanes
            lanePositions.Add(leftLane.transform.position.x);
            lanePositions.Add(rightLane.transform.position.x);
            _judgementButtons.Add(leftButton);
            _judgementButtons.Add(rightButton);
        }
        // Sort lanes from left to right
        // Ex: 3 lane layout sorted: -4 0 4 
        lanePositions.Sort();
        return lanePositions;
    }

    private void _CreateNoteSpawner(List<float> lanePositions)
    {
        _noteSpawner = Instantiate(_noteSpawnerObject, gameObject.transform).GetComponent<NoteSpawner>();
        _noteSpawner.Initialize(lanePositions, _song);
    }

    private void _AddJudgementListeners(Action gameEnd)
    {
        for (int i = 0; i < _judgementButtons.Count; i++)
        {
            _judgementButtons[i].OnSoundEffect += _soundManager.PlayEffect;
            _judgementButtons[i].OnToggleGameSong += _soundManager.ToggleGameSong;
            _judgementButtons[i].OnGameEnd += gameEnd;
        }
    }

    // Create all Necessary GameBoard Components
    private void _BuildBoard(SongDataScriptableObject song, Action gameManagerMethod)
    {
        // Place lanes & Buttons (each lane gets a button)
        List<float> laneHorizPositions = _PlaceLanes();

        // Subscribe SoundManager methods to Judgement buttons
        _AddJudgementListeners(gameManagerMethod);    

        // Create Spawner
        _CreateNoteSpawner(laneHorizPositions);

    }

}