using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MultipleNote : INote
{
    // [SerializeField] public float Timestamp {get; set;}
    public int NumQuarterNotes {get; set;}
    
    [SerializeField] private float _lanePosition;
    [SerializeField]
    private int _numNotes;
    [SerializeField]
    private List<float> _noteSpawnLocations;



    int count;

    [SerializeReference]
    private List<INote> _notes;

    [SerializeField] public float timestamp;

    private GameObject _childNotePrefab;
    [SerializeField]
    private GameObject _parentNotePrefab;

    // public void Initialize(float spawnPosition, int numNotes, List<float> noteLocations, GameObject parentNotePrefab, GameObject childNotePrefab)
    
    public MultipleNote(float spawnPosition, float timeToSpawn, int numNotes, List<float> noteLocations, GameObject parentNotePrefab, GameObject childNotePrefab)
    {
        timestamp = timeToSpawn;
        _childNotePrefab = childNotePrefab;
        _parentNotePrefab = parentNotePrefab;
        _noteSpawnLocations = noteLocations;
        _lanePosition = spawnPosition;
        _numNotes = numNotes;
    }

    public MultipleNote(float spawnPosition, float timeToSpawn, List<float> noteSpawnLocations, GameObject parentNotePrefab, GameObject childNotePrefab)
    {
        _lanePosition = spawnPosition;
        timestamp = timeToSpawn;
        _parentNotePrefab = parentNotePrefab;
        _childNotePrefab = childNotePrefab;
        _notes = new List<INote>();
        _noteSpawnLocations = noteSpawnLocations;
        count = 0;
    }

    public void AddNote(INote noteToAdd)
    {
        //FIXME: debug limit
        int limit = 2;
        if(_notes.Count >= limit) return;
        noteToAdd.SetLane(_noteSpawnLocations[count]);
        _notes.Add(noteToAdd);
        count++;

    }

    public float Timestamp()
    {
        return timestamp;
    }
    public void Spawn(Transform parent)
    {
        // Create Parent gameobject
        GameObject newNote = GameObject.Instantiate(_parentNotePrefab, parent);
        newNote.transform.position = new Vector3(_lanePosition, newNote.transform.position.y, newNote.transform.position.z);

        // Create children
        for(int i = 0; i < _notes.Count; i++)
        {
            _notes[i].Spawn(newNote.transform, 0f);
            // GameObject newChildNote = GameObject.Instantiate(_childNotePrefab, newNote.transform);
            // newChildNote.transform.position = new Vector3(
            //     noteLocations[i], 
            //     newNote.transform.position.y, 
            //     newNote.transform.position.z
            //     );
        }
    }

    public void SetLane(float lane)
    {
        _lanePosition = lane;
    }

    public void Spawn(Transform parent, float speed)
    {

    }

   
}