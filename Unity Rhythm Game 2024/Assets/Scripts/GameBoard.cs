using System;
using UnityEngine;
public class GameBoard : MonoBehaviour
{
    [SerializeField] private GameObject LaneObject;

    [SerializeField] private GameObject JudgementButtonObject;

    [SerializeField] private Transform JudgementButtonTransform;

    [SerializeField] private GameObject _TouchManagerObject;

    private TouchManager _touchManager;
    
    private Vector3 _center;
    private int _centerHorizOffset;
    private bool _hasEvenLanes;

    [Range(3, 5)]
    [SerializeField] private int _numLanes;
    private int _laneHorizSpacing;
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
    }
    private void _PlaceLanes()
    {
        // Place evenly spaced lanes
        for(int i = 0; i < Math.Ceiling(_numLanes / 2.0); i++)
        {
            Debug.Log(i);
            if (i == 0 && !_hasEvenLanes)
            {
                Instantiate(LaneObject, _center, Quaternion.identity);
            
                JudgementButtons center = Instantiate(JudgementButtonObject, JudgementButtonTransform).GetComponent<JudgementButtons>();
               
                // center.transform.position = new Vector3(0,0,0);
                center.Initialize(_touchManager, new Vector3(0,0,0), 0);
                continue;
            }
            Vector3 lanePosition = new Vector3(i * _laneHorizSpacing + _centerHorizOffset, 0, 0);    
            Instantiate(LaneObject, _center + lanePosition, Quaternion.identity);
            Instantiate(LaneObject, _center - lanePosition, Quaternion.identity);
            JudgementButtons rightButton = Instantiate(JudgementButtonObject, JudgementButtonTransform).GetComponent<JudgementButtons>();
            JudgementButtons leftButton = Instantiate(JudgementButtonObject, JudgementButtonTransform).GetComponent<JudgementButtons>();
            rightButton.Initialize(_touchManager, lanePosition, i);
            leftButton.Initialize(_touchManager, lanePosition * Vector2.left, -1);
            // rightButton.transform.position = lanePosition;
            
            // leftButton.transform.position = lanePosition * Vector2.left;
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