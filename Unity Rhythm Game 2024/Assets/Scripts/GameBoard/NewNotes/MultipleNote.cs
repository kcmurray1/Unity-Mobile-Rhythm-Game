using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

[Serializable]
public class MultipleNote : INote
{
    private float _lanePosition;
    [SerializeField]
    private int _numNotes;
    [SerializeField]
    private List<float> _noteSpawnLocations;

    private GameObject _childNotePrefab;
    
    private GameObject _parentNotePrefab;

    // public void Initialize(float spawnPosition, int numNotes, List<float> noteLocations, GameObject parentNotePrefab, GameObject childNotePrefab)
    
    public MultipleNote(float spawnPosition, int numNotes, List<float> noteLocations, GameObject parentNotePrefab, GameObject childNotePrefab)
    {
        _childNotePrefab = childNotePrefab;
        _parentNotePrefab = parentNotePrefab;
        _noteSpawnLocations = noteLocations;
        _lanePosition = spawnPosition;
        _numNotes = numNotes;
    }



    public void Spawn(Transform parent)
    {
        // Create Parent gameobject
        GameObject newNote = GameObject.Instantiate(_parentNotePrefab, parent);
        newNote.transform.position = new Vector3(_lanePosition, newNote.transform.position.y, newNote.transform.position.z);

        // Create children
        for(int i = 0; i < _numNotes; i++)
        {
            GameObject newChildNote = GameObject.Instantiate(_childNotePrefab, newNote.transform);
            newChildNote.transform.position = new Vector3(
                _noteSpawnLocations[i], 
                newNote.transform.position.y, 
                newNote.transform.position.z
                );
        }
    }
}