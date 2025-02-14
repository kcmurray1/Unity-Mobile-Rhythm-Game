using System;
using UnityEngine;


public interface INote
{
    // Spawn note
    void Spawn(Transform parent);

    /// <summary>
    /// The time in (float)seconds that this note will be spawned
    /// </summary>
    float Timestamp {get; set;}
    int NumQuarterNotes {get; set;}

    
}

[Serializable]
public class SingleNote : INote
{
    public float Timestamp {get; set;}
    public int NumQuarterNotes {get; set;}
    private GameObject _notePrefab;
    [SerializeField]
    private float _lanePosition;
    public SingleNote(float lanePosition, float timeToSpawn, GameObject notePrefab=null)
    {
        Timestamp = timeToSpawn;
        _lanePosition = lanePosition;
        _notePrefab = notePrefab;
    }
    
    //Spawn note
    public void Spawn(Transform parent)
    {
        GameObject newNote = GameObject.Instantiate(_notePrefab, parent);
        newNote.transform.position = new Vector3(_lanePosition, newNote.transform.position.y, newNote.transform.position.z);
    }

}