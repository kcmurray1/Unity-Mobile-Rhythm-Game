using System;
using UnityEngine;


public interface INote
{
    // Spawn note
    void Spawn(Transform parent);

    void Spawn(Transform parent, float speed);

    void SetLane(float lane);

    /// <summary>
    /// The time in (float)seconds that this note will be spawned
    /// </summary>
    // [SerializeField] float Timestamp {get; set;}
    float Timestamp();
    int NumQuarterNotes {get; set;}

    
}

[Serializable]
public class SingleNote : INote
{
    // [SerializeField] public float Timestamp {get; set;}

    [SerializeField] public float timestamp;
    public int NumQuarterNotes {get; set;}
    [SerializeField] private GameObject _notePrefab;
    [SerializeField] private float _lanePosition;
    public SingleNote(float lanePosition, float timeToSpawn, GameObject notePrefab=null)
    {
        timestamp = timeToSpawn;
        _lanePosition = lanePosition;
        _notePrefab = notePrefab;
    }

    public void SetLane(float lane)
    {
        _lanePosition = lane;
    }
    
    //Spawn note
    public void Spawn(Transform parent)
    {
        GameObject newNote = GameObject.Instantiate(_notePrefab, parent);
        newNote.transform.position = new Vector3(_lanePosition, newNote.transform.position.y, newNote.transform.position.z);
    }

    public void Spawn(Transform parent, float speed)
    {
        GameObject newNote = GameObject.Instantiate(_notePrefab, parent);
        Note n = newNote.GetComponent<Note>();
        n.NoteSpeed = speed;
        newNote.transform.position = new Vector3(_lanePosition, parent.position.y, parent.position.z);
    }

    public float Timestamp()
    {
        return timestamp;
    }

}