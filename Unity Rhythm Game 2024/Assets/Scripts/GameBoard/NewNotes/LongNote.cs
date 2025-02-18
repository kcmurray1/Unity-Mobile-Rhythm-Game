using System;
using UnityEngine;

public class LongNote : INote
{
    public float Timestamp {get; set;}
    public int NumQuarterNotes {get; set;}

    private int _numChildren;

    private float _lanePosition;
    private GameObject _parentPrefab;
    private GameObject _childPrefab;

    public LongNote(float spawnPosition, int numChildren, float timeToSpawn, GameObject parentPrefab, GameObject childPrefab)
    {
        _lanePosition = spawnPosition;
        Timestamp = timeToSpawn;
        _numChildren = numChildren;
        _parentPrefab = parentPrefab;
        _childPrefab = childPrefab;

    }
    public void Spawn(Transform parent)
    {
        //Spawn parent
        GameObject newParent = GameObject.Instantiate(_parentPrefab, parent);
        Debug.Log(parent.transform.position);
        newParent.transform.position = new Vector3(_lanePosition, newParent.transform.position.y + _numChildren, newParent.transform.position.z);
        //Spawn children
        for(int i = 0; i < _numChildren; i++)
        {
            GameObject newChild = GameObject.Instantiate(_childPrefab, newParent.transform);
            newChild.transform.position = new Vector3(
                _lanePosition, 
                newParent.transform.position.y - i - 1, 
                newParent.transform.position.z
            );

        }
    }
}