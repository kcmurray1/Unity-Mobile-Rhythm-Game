using System.Collections; // IEnumerator()
using System.Collections.Generic; // Dictionary
using UnityEngine;
using System.Linq; //ToList()
using System;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
/// <summary>
/// Class <c> NoteSpawner </c> reads a midi file to generate a noteMaps used <br/>
/// to spawn notes for the game.
/// </summary>
public class NoteSpawner : MonoBehaviour 
{

    // Object to spawn
    public GameObject notePrefab;
    public GameObject _longNotePrefab;

    // Dict containing where lane number is the key and lane position is the value
    private Dictionary<int, float> _laneHorizPositions;
    private int _centerLaneIndex;
    // Spawner Object(itself)
    [SerializeField] private GameObject _spawnerObject;

    /// <summary>
    ///  Set the spawn boundaries <br/>
    ///  Ex): Receiving three lane positions [-4,0,4] will update _laneHorizPositions to <br/>
    ///  {0: -4, 1: 0, 2: 4} <br/>
    ///  where lane 0 is located at x = -4 and lane 1 is located at x = 0
    /// </summary>
    /// <param name="lanePositions">The x-coordinate for each lane</param>   
    public void Initialize(List<float> lanePositions)
    {
        _InitializeLanePositions(lanePositions);
        Dictionary<float, List<MidiNote>> songMap = _GetSongData("Assets/Songs/Test_MIDI_Cascade_2.mid",115);
        StartCoroutine(_SpawnNote(songMap));
    }

    private void _InitializeLanePositions(List<float> lanePositions)
    {
        _laneHorizPositions = new Dictionary<int, float>();
        int laneIndex = 0;
        foreach(float laneHorizPosition in lanePositions)
        {
            _laneHorizPositions[laneIndex] = laneHorizPosition;
            laneIndex++; 
        }
        _centerLaneIndex = _laneHorizPositions.Count / 2;
    }
    
    // 
    private void _Spawn(int laneIndex, bool isLongNote=false, bool isStart=false, bool isEnd=false, int numChildren=0)
    {
        GameObject noteToSpawn = notePrefab;
        if (isLongNote)
        {
            noteToSpawn = _longNotePrefab;
        }
        GameObject newNote = Instantiate(noteToSpawn, _spawnerObject.transform);
        // Adjust horizontal position
        newNote.transform.position = new Vector3(_laneHorizPositions[laneIndex], 
                newNote.transform.position.y, newNote.transform.position.z);
        // Adjust look and tag for start and end notes
        if(isStart || isEnd)
        {
            newNote.gameObject.GetComponent<SpriteRenderer>().color = Color.yellow;
            newNote.tag = isStart ? "start" : "end";
        }
        if (isLongNote)
        {
            newNote.GetComponent<NoteLong>().Initialize(numChildren);
        }
    }

    private IEnumerator _SpawnNote(Dictionary<float, List<MidiNote>> noteMap)
    {   
        _Spawn(_centerLaneIndex, isStart: true);
        float prevTime = 0;
        foreach(float timestamp in noteMap.Keys)
        {
            foreach(MidiNote note in noteMap[timestamp])
            {
                if(timestamp - prevTime != 0)
                {   
                    yield return new WaitForSeconds(timestamp - prevTime);
                }     
                prevTime = timestamp;
                _Spawn(laneIndex: note.LaneIndex, isLongNote: note.IsLongNote, numChildren: note.NumQuarterNotes);
            }
        }
        _Spawn(_centerLaneIndex, isEnd: true);
    }

    private List<MidiNote> _RemoveExtraNotes(List<MidiNote> notes, int maxNotes=1)
    {
        while(notes.Count > _laneHorizPositions.Count || notes.Count > maxNotes)
        {
            notes.RemoveAt(0);
        }
        return notes;
    }

    private List<MidiNote> _RedistributeLanes(List<MidiNote> notes)
    {
        Stack<int> availableLanes = new Stack<int>(_laneHorizPositions.Keys);
        for(int i =0; i < notes.Count; i++)
        {
            notes[i].LaneIndex = availableLanes.Pop();
        }
        return notes;
    }
    // Adjust notes that are spawned at the same time in the same lane(i.e prevent notes from stacking)
    private List<MidiNote> _RemoveMidiNoteCollisions(List<MidiNote> notes)
    {
        // No collision
        if(notes.Count < 2)
        {
            return notes;
        }
        // Remove notes such that notes.Count <= numLanes
        notes = _RemoveExtraNotes(notes);
        // Redistribute notes across lanes
        notes = _RedistributeLanes(notes);
        return notes;
    }

    // Assign random lane to MidiNote
    private int _AssignRandomLane()
    {
        System.Random rnd = new System.Random();
        return rnd.Next(0, _laneHorizPositions.Count);
    }
    // Read midifile
    private Dictionary<float, List<MidiNote>> _GetSongData(string fileName, float bpm)
    {
        MidiFile midiFile = MidiFile.Read(fileName);

        float midiTempo = (float)midiFile.GetTempoMap().GetTempoAtTime((MidiTimeSpan)0).BeatsPerMinute;
        TempoMap newTempoMap;
        //Set tempo if they do not match
        if(midiTempo != bpm)
        {
            //Standard 96 ticks per quarter note
            var timeDivision = new TicksPerQuarterNoteTimeDivision((Int16)96);
            //Create tempo based on song tempo
            newTempoMap = TempoMap.Create(timeDivision, Tempo.FromBeatsPerMinute((double)bpm));
        }
        else
        {
            newTempoMap = midiFile.GetTempoMap();
        }
        //Generate new map
        //Use Dictionary to store timestamps as keys and lanes as values
        //The purpose is to check if there is a note in a lane at noteTimeDict[timestamp]
        //and avoid collisions by placing a new note in a different lane
        Dictionary<float, List<MidiNote>> midiNoteMap = new Dictionary<float, List<MidiNote>>();
        //Get Note timings from midiFile
        var notes = midiFile.GetNotes().ToList();
        int noteNum = 0;
        // Build initial map
        foreach(var note in notes)
        {
            //Get the timestamp of when a note is played
            float spawnTime = (float)note.TimeAs<MetricTimeSpan>(newTempoMap).TotalSeconds;
            double noteLength = note.LengthAs<MetricTimeSpan>(newTempoMap).TotalSeconds;
            MidiNote newNote = new MidiNote(noteLength, _AssignRandomLane(), spawnTime, noteNum);
            if(!midiNoteMap.ContainsKey(spawnTime))
            {
                midiNoteMap[spawnTime] = new List<MidiNote>();
            }
            midiNoteMap[spawnTime].Add(newNote);
            noteNum++;     
        }
        // Resolve any spawning collisions
        foreach(float timestamp in midiNoteMap.Keys)
        {
            _RemoveMidiNoteCollisions(midiNoteMap[timestamp]);
        }
        Debug.Log($"Finished Generation for {fileName}");

        return midiNoteMap;
    }

}

