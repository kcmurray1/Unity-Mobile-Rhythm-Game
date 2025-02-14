using System;
using UnityEngine;

[Serializable]
public abstract class MyNote
{
    GameObject _notePrefab;
    public MyNote(float lanePosition, GameObject notePrefab)
    {
        _notePrefab = notePrefab;
    }
    public abstract void Spawn(Transform parent);
}

public interface INote
{
    // Spawn note
    void Spawn(Transform parent);

}

[CreateAssetMenu(fileName = "SingleNote", menuName = "Notes/SingleNote")]
public class SingleNote : INote
{
    private GameObject _notePrefab;
    [SerializeField]
    private float _lanePosition;
    public SingleNote(float lanePosition, GameObject notePrefab=null)
    {
        _lanePosition = lanePosition;
        _notePrefab = notePrefab;
    }

    public void Initialize(float lanePosition, GameObject notePrefab)
    {
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