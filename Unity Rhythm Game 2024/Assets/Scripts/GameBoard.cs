using System;
using System.Collections.Generic;
using UnityEngine;
public class GameBoard : MonoBehaviour
{
    // Lanes and Judgement Buttons
    [SerializeField] private GameObject LaneObject;

    [SerializeField] private GameObject JudgementButtonObject;

    [SerializeField] private Transform JudgementButtonTransform;

    // Managers
    private TouchManager _touchManager;
    private ScoreManager _scoreManager;
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

    // Board Design
    private const float LARGE_LANE_SPACING = 4f;
    private const float MEDIUM_LANE_SPACING = 3.75f;

    private const float SMALL_LANE_SPACING = 3.5f;

    enum LaneSpacing
    {
        Small,
        Medium,
        Large
    }

    
    void Start()
    {
        Initialize(5, LaneSpacing.Small);    
    }    

    // TODO: make LaneSpacing Enum accessible outside of this class to change this into a public method
    private void Initialize(int numLanes, LaneSpacing mode)
    {
        _SetLaneSpacing(mode);
        _numLanes = numLanes;

        _center = new Vector3(0,10,90);
        _hasEvenLanes = false;
        _centerHorizOffset = 0;

        if (_numLanes % 2 == 0)
        {
            _centerHorizOffset = 2;
            _hasEvenLanes = true;
        }
        _BuildBoard();
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
    private void _CreateManagers()
    {
        // Create TouchManager
        if(_touchManager)
        {
            Destroy(_touchManager);
            _touchManager = null;
        }
        _touchManager = Instantiate(_touchManagerPrefab, gameObject.transform).GetComponent<TouchManager>();
        // Create ScoreManager
        if (_scoreManager)
        {
            Destroy(_scoreManager);
            _scoreManager = null;
        }
        _scoreManager = Instantiate(_scoreManagerPrefab, gameObject.transform).GetComponent<ScoreManager>();
        _scoreManager.Initialize();

        // Find SoundManager
        _soundManagerObject = GameObject.Find("SoundManager");
        _soundManager = _soundManagerObject.GetComponent<SoundManager>();
    }

    // Add lanes to the board
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
                center.Initialize(_touchManager, _scoreManager, _soundManager, new Vector3(0,0,0), i);
                lanePositions.Add(centerLane.transform.position.x);
                continue;
            }
            Vector3 lanePosition = new Vector3(i * _laneHorizSpacing + _centerHorizOffset, 0, 0);
            // Create Mirrored Lanes    
            GameObject rightLane = Instantiate(LaneObject, _center + lanePosition, Quaternion.identity);
            GameObject leftLane = Instantiate(LaneObject, _center - lanePosition, Quaternion.identity);
            // Create Mirrored Judgement Buttons
            JudgementButton rightButton = Instantiate(JudgementButtonObject, JudgementButtonTransform).GetComponent<JudgementButton>();
            JudgementButton leftButton = Instantiate(JudgementButtonObject, JudgementButtonTransform).GetComponent<JudgementButton>();
            rightButton.Initialize(_touchManager, _scoreManager, _soundManager, lanePosition, i);
            leftButton.Initialize(_touchManager, _scoreManager, _soundManager, lanePosition * Vector2.left, -i);
            // Record location of lanes
            lanePositions.Add(leftLane.transform.position.x);
            lanePositions.Add(rightLane.transform.position.x);
        }
        lanePositions.Sort();
        return lanePositions;
    }

    private void _CreateNoteSpawner(List<float> lanePositions)
    {
        _noteSpawner = Instantiate(_noteSpawnerObject, gameObject.transform).GetComponent<NoteSpawner>();
        _noteSpawner.Initialize(lanePositions);
    }

    // Create all Necessary GameBoard Components
    private void _BuildBoard()
    {
        // Create Managers
        _CreateManagers();
        // Place lanes & Buttons (each lane gets a button)
        List<float> laneHorizPositions = _PlaceLanes();    

        // Create Spawner
        _CreateNoteSpawner(laneHorizPositions);

    }

}