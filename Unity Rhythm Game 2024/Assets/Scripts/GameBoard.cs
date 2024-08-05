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
    [SerializeField] private GameObject _touchManagerObject;
    [SerializeField] private GameObject _scoreManagerObject;

    // NoteSpawner

    private NoteSpawner _noteSpawner;
    [SerializeField] private GameObject _noteSpawnerObject;
    
    // Center of board
    private Vector3 _center;

    // Spacing around the center
    private int _centerHorizOffset;

    private int _laneHorizSpacing;
    // Number of lanes
    [Range(3, 5)]
    [SerializeField] private int _numLanes;
    private bool _hasEvenLanes;
    
    void Start()
    {
        _laneHorizSpacing = 4;
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

    // Instantiate Managers for scoring and interaction
    private void _CreateManagers()
    {
        // Create TouchManager
        if(_touchManager)
        {
            Destroy(_touchManager);
            _touchManager = null;
        }
        _touchManager = Instantiate(_touchManagerObject, gameObject.transform).GetComponent<TouchManager>();
       
        // Create ScoreManager
        if (_scoreManager)
        {
            Destroy(_scoreManager);
            _scoreManager = null;
        }

        _scoreManager = Instantiate(_scoreManagerObject, gameObject.transform).GetComponent<ScoreManager>();
    }

    // Add lanes to the board
    private List<int> _PlaceLanes()
    {
        List<int> lanePositions = new List<int>();
        // Place evenly spaced lanes
        for(int i = 0; i < Math.Ceiling(_numLanes / 2.0); i++)
        {
            if (i == 0 && !_hasEvenLanes)
            {
                GameObject centerLane = Instantiate(LaneObject, _center, Quaternion.identity);
                JudgementButton center = Instantiate(JudgementButtonObject, JudgementButtonTransform).GetComponent<JudgementButton>();
                center.Initialize(_touchManager, _scoreManager, new Vector3(0,0,0), i);
                lanePositions.Add((int)centerLane.transform.position.x);
                continue;
            }
            Vector3 lanePosition = new Vector3(i * _laneHorizSpacing + _centerHorizOffset, 0, 0);    
            GameObject rightLane = Instantiate(LaneObject, _center + lanePosition, Quaternion.identity);
            GameObject leftLane = Instantiate(LaneObject, _center - lanePosition, Quaternion.identity);
            JudgementButton rightButton = Instantiate(JudgementButtonObject, JudgementButtonTransform).GetComponent<JudgementButton>();
            JudgementButton leftButton = Instantiate(JudgementButtonObject, JudgementButtonTransform).GetComponent<JudgementButton>();
            rightButton.Initialize(_touchManager, _scoreManager, lanePosition, i);
            leftButton.Initialize(_touchManager, _scoreManager, lanePosition * Vector2.left, -i);

            lanePositions.Add((int)leftLane.transform.position.x);
            lanePositions.Add((int)rightLane.transform.position.x);
        }
        lanePositions.Sort();
        return lanePositions;
    }

    private void _CreateNoteSpawner(List<int> lanePositions)
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
        List<int> laneHorizPositions = _PlaceLanes();    

        // Create Spawner
        _CreateNoteSpawner(laneHorizPositions);

    }

}