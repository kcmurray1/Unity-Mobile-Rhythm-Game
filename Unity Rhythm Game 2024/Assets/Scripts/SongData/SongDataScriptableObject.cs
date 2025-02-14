using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "SongDataScriptableObject", menuName = "ScriptableObjects/Song", order = 0)]
public class SongDataScriptableObject : ScriptableObject
{
    [SerializeField] public SpawnMap EasyNoteMap;
    [SerializeField] private SpawnMap MediumNoteMap;
    [SerializeField] private SpawnMap HardNoteMap;


    [SerializeReference] public List<INote> test;


    public string Name; 
    public int Bpm; 
    public string MidiFile; 
    public AudioClip SongClip;

}


public class test : ISerializationCallbackReceiver {


    public void OnBeforeSerialize()
    {
        Debug.Log("serial");
    }

    public void OnAfterDeserialize()
    {
        Debug.Log("deserial");
    }
}



/// <summary>
/// Class <c>SpawnData</c> stores a list of MidiNotes to spawn at a specific timestamp.
/// </summary>
[Serializable]
public class SpawnData
{
    [SerializeField]
    private List<MidiNote> _notes;
    [SerializeField]
    private float _timestamp;

    public SpawnData(float timestamp, List<MidiNote> notes) 
    { 
        _timestamp = timestamp;
        _notes = notes;
    }

    public float GetTimestamp()
    {
        return _timestamp;
    }

    public List<MidiNote> GetNotes()
    {
        return _notes;
    }
}


public interface ISerializeNoteData 
{
    void Spawn();
}

[Serializable]
public class SerializableMultiNote : ISerializeNoteData
{
    [SerializeField]
    private int _numNotes;
    [SerializeField]
    private List<float> _noteSpawnLocations;
    public void Spawn()
    {

    }
}

public class SerializableSingleNote : ISerializeNoteData
{
    public bool isAwesome = false;
    public void Spawn()
    {

    }
}

/// <summary>
/// Class <c>SpawnMap</c> stores information required for gameplay such as the location, 
/// time, and number of notes to spawn.
/// </summary>
[Serializable]
public class SpawnMap
{
    public List<SpawnData> map;
    public int TopScore;

    [SerializeField]
    private int _noteCount;

    private void _Reset()
    {
        TopScore = 0;
        _noteCount = 0;
        map = new List<SpawnData>();
    }
    // Create an empty SpawnMap
    public SpawnMap()
    {
       _Reset();
    }
    // Create a SpawnMap from a dictionary
    public SpawnMap(Dictionary<float, List<MidiNote>> midiNoteMap)
    {
        _Reset();
        foreach(float timestamp in midiNoteMap.Keys)
        {
            this.AddNotes(timestamp, midiNoteMap[timestamp]);
            _noteCount += midiNoteMap[timestamp].Count;
        }
    }
    
    // Add notes to the SpawnMap 
    public void AddNotes(float timestamp, List<MidiNote> notes)
    {
        map.Add(new SpawnData(timestamp, notes));
    }
    // Converting two lists into a dict: https://stackoverflow.com/questions/4038978/map-two-lists-into-a-dictionary-in-c-sharp
    public Dictionary<float, List<MidiNote>> GetMap()
    {
        return map.ToDictionary(x=>x.GetTimestamp(), x => x.GetNotes());
    }

}

