using System;
using UnityEngine;

public class ShortNote : INote
{

     [SerializeField] private float _timestamp;

    public int NumQuarterNotes {get; set;}

    private int _numChildren;

    private float _lanePosition;

    [SerializeField] private GameObject _notePrefab;

    public ShortNote(float spawnPosition, int numNotes, float timeToSpawn, GameObject notePrefab)
    {
        _lanePosition = spawnPosition;
        _numChildren = numNotes;
        _timestamp = timeToSpawn;
        _notePrefab = notePrefab;

    }

    public float Timestamp()
    {
        return _timestamp;
    }

    public void Spawn(Transform parent)
    {
        //Spawn main note
        GameObject newParent = GameObject.Instantiate(_notePrefab, parent);
        newParent.transform.position = new Vector3(_lanePosition, newParent.transform.position.y, newParent.transform.position.z);
    }

    public void Spawn(Transform parent, float spawnPosition)
    {
        throw new NotImplementedException();
    }

    public void SetLane(float lane)
    {
       throw new NotImplementedException();
    }
}