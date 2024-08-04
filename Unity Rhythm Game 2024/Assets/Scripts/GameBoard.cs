using System;
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
    [SerializeField] private GameObject _TouchManagerObject;
    [SerializeField] private GameObject _ScoreManagerObject;
    
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
        _touchManager = Instantiate(_TouchManagerObject, gameObject.transform).GetComponent<TouchManager>();
       
        // Create ScoreManager
        if (_scoreManager)
        {
            Destroy(_scoreManager);
            _scoreManager = null;
        }

        _scoreManager = Instantiate(_ScoreManagerObject, gameObject.transform).GetComponent<ScoreManager>();
    }

    // Add lanes to the board
    private void _PlaceLanes()
    {
        // Place evenly spaced lanes
        for(int i = 0; i < Math.Ceiling(_numLanes / 2.0); i++)
        {
            Debug.Log(i);
            if (i == 0 && !_hasEvenLanes)
            {
                Instantiate(LaneObject, _center, Quaternion.identity);
            
                JudgementButton center = Instantiate(JudgementButtonObject, JudgementButtonTransform).GetComponent<JudgementButton>();
                center.Initialize(_touchManager, new Vector3(0,0,0), i);
                continue;
            }
            Vector3 lanePosition = new Vector3(i * _laneHorizSpacing + _centerHorizOffset, 0, 0);    
            Instantiate(LaneObject, _center + lanePosition, Quaternion.identity);
            Instantiate(LaneObject, _center - lanePosition, Quaternion.identity);
            JudgementButton rightButton = Instantiate(JudgementButtonObject, JudgementButtonTransform).GetComponent<JudgementButton>();
            JudgementButton leftButton = Instantiate(JudgementButtonObject, JudgementButtonTransform).GetComponent<JudgementButton>();
            rightButton.Initialize(_touchManager, lanePosition, i);
            leftButton.Initialize(_touchManager, lanePosition * Vector2.left, -1);
        }
    }

    // Spawn all Necessary GameBoard Components
    private void _BuildBoard()
    {
        // Create Managers
        _CreateManagers();
        // Place lanes & Buttons (each lane gets a button)
        _PlaceLanes();    


    }

}