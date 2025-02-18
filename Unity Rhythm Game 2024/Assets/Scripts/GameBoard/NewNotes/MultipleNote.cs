using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MultipleNote : INote
{
    public float Timestamp {get; set;}
    public int NumQuarterNotes {get; set;}
    private float _lanePosition;
    [SerializeField]
    private int _numNotes;
    [SerializeField]
    private List<float> _noteSpawnLocations;

    private Dictionary<int, float> noteLocations;
    [SerializeReference]
    private List<INote> _notes;

    private GameObject _childNotePrefab;
    
    private GameObject _parentNotePrefab;

    // public void Initialize(float spawnPosition, int numNotes, List<float> noteLocations, GameObject parentNotePrefab, GameObject childNotePrefab)
    
    public MultipleNote(float spawnPosition, float timeToSpawn, int numNotes, List<float> noteLocations, GameObject parentNotePrefab, GameObject childNotePrefab)
    {
        Timestamp = timeToSpawn;
        _childNotePrefab = childNotePrefab;
        _parentNotePrefab = parentNotePrefab;
        _noteSpawnLocations = noteLocations;
        _lanePosition = spawnPosition;
        _numNotes = numNotes;
    }

    public MultipleNote(float spawnPosition, float timeToSpawn, Dictionary<int, float> noteSpawnLocations, GameObject parentNotePrefab, GameObject childNotePrefab)
    {
        _lanePosition = spawnPosition;
        Timestamp = timeToSpawn;
        _parentNotePrefab = parentNotePrefab;
        _childNotePrefab = childNotePrefab;
        _notes = new List<INote>();
        noteLocations = noteSpawnLocations;
    }

    public void AddNote(INote noteToAdd)
    {
        //FIXME: debug limit
        int limit = 2;
        if(_notes.Count >= limit) return;

        _notes.Add(noteToAdd);

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

    public void Spawn(Transform parent, float speed)
    {

    }
}