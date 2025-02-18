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

    public GameObject multiNotePrefab;

    public GameObject multiNoteChildPrefab;

    public GameObject LongNotePrefab;
    public GameObject LongNoteChildPrefab;

    // Dict containing where lane number is the key and lane position is the value
    private Dictionary<int, float> _laneHorizPositions;
    [SerializeField] private int _centerLaneIndex;
    // Spawner Object(itself)
    [SerializeField] private GameObject _spawnerObject;

    // void Awake()
    // {
    //     Initialize(new List<float>{0});
    // }
    /// <summary>
    ///  Set the spawn boundaries <br/>
    ///  Ex): Receiving three lane positions [-4,0,4] will update _laneHorizPositions to <br/>
    ///  {0: -4, 1: 0, 2: 4} <br/>
    ///  where lane 0 is located at x = -4 and lane 1 is located at x = 0
    /// </summary>
    /// <param name="lanePositions">The x-coordinate for each lane</param>   
    public void Initialize(List<float> lanePositions, SongDataScriptableObject song=null)
    {
        print(lanePositions);
        _InitializeLanePositions(lanePositions);
        // Debug.Log(song.MidiFile);
      
        // Dictionary<float, List<MidiNote>> songMap = _GetSongData(song);
        // Debug.Log($"Spawned {songMap.Count} notes");
        List<INote> debugMap = new List<INote>{
            new LongNote(lanePositions[0], 5, 0f, LongNotePrefab, LongNoteChildPrefab),
        };
        List<INote> testMap = _GetSongData_V2(song);
        
        StartCoroutine(_SpawnNotes_v2(testMap));
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
            newNote.gameObject.GetComponent<SpriteRenderer>().enabled = false;
            newNote.tag = isStart ? "start" : "end";
        }
        
        if (isLongNote)
        {
            newNote.GetComponent<NoteLong>().Initialize(numChildren);
        }
    }

    private IEnumerator _SpawnNotes_v2(List<INote> noteMap)
    {   
        _Spawn(_centerLaneIndex, isStart: true);
        float prevTime = 0;
        foreach(INote noteToSpawn in noteMap)
        {
            if(noteToSpawn.Timestamp - prevTime != 0)
            {
                yield return new WaitForSeconds(noteToSpawn.Timestamp - prevTime);
            }
            prevTime = noteToSpawn.Timestamp;
            noteToSpawn.Spawn(gameObject.transform);
        }
        yield return new WaitForSeconds(3f);
        _Spawn(_centerLaneIndex, isEnd: true);
    }

    
    // Reduce number of notes to be <= number of lanes and to <= maxNotes
    // Ex: A Gameboard with 4 lanes can have at most 4 notes in each lane at the same timestamp.
    // maxNotes can be used to reduce this number further to support game difficulty.
    // An easy difficulty for this Gameboard my allow at most 1 note at each timestamp; whereas,
    // a hard difficulty may allow at most 4 notes to spawn at once.
    private List<MidiNote> _RemoveExtraNotes(List<MidiNote> notes, int maxNotes=2)
    {
        while(notes.Count > _laneHorizPositions.Count || notes.Count > maxNotes)
        {
            notes.RemoveAt(0);
        }
        return notes;
    }

    // Spread notes across the Gameboard such that there is at most
    // 1 note in each lane.
    private List<MidiNote> _RedistributeNotes(List<MidiNote> notes)
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
        // Handle Collision
        if(notes.Count > 1)
        {
            // Remove notes such that notes.Count <= numLanes
            notes = _RemoveExtraNotes(notes);
            // Redistribute notes across lanes
            notes = _RedistributeNotes(notes);
        }
        return notes;
    }

    // Assign random lane to MidiNote
    private int _AssignRandomLane()
    {
        System.Random rnd = new System.Random();
        return rnd.Next(0, _laneHorizPositions.Count);
    }
    
    private List<INote> _GetSongData_V2(SongDataScriptableObject song)
    {      
        // Notemap Already exists
        // if (song.EasyNoteMap.map.Count > 0)
        // {
        //     return song.EasyNoteMap.GetMap();
        // }
        // // Create a new note map
        MidiFile midiFile = MidiFile.Read(song.MidiFile);

        float midiTempo = (float)midiFile.GetTempoMap().GetTempoAtTime((MidiTimeSpan)0).BeatsPerMinute;
        TempoMap newTempoMap;
        //Set tempo if they do not match
        if(midiTempo != song.Bpm)
        {
            //Standard 96 ticks per quarter note
            var timeDivision = new TicksPerQuarterNoteTimeDivision(96);
            //Create tempo based on song tempo
            newTempoMap = TempoMap.Create(timeDivision, Tempo.FromBeatsPerMinute(song.Bpm));
        }
        else
        {
            newTempoMap = midiFile.GetTempoMap();
        }
        // Generate new map
        // Use Dictionary to store timestamps as keys and lanes as values
        // The purpose is to check if there is a note in a lane at noteTimeDict[timestamp]
        // and avoid collisions by placing a new note in a different lane
        Dictionary<float, INote> midiNoteMap = new Dictionary<float, INote>();
        // Get Note timings from midiFile
        var notes = midiFile.GetNotes().ToList();
        // Build initial map
        foreach(var note in notes)
        {
            // Get the timestamp of when a note is played
            float spawnTime = (float)note.TimeAs<MetricTimeSpan>(newTempoMap).TotalSeconds;
            double noteLength = note.LengthAs<MetricTimeSpan>(newTempoMap).TotalSeconds;
            SingleNote newNote = new SingleNote(_laneHorizPositions[_AssignRandomLane()], spawnTime, notePrefab);
            // Each timestamp holds a list of MidiNotes
            if(!midiNoteMap.ContainsKey(spawnTime))
            {
                midiNoteMap[spawnTime] = newNote;
            }
            else
            {
                if(midiNoteMap[spawnTime] is MultipleNote multiNote)
                {
                    multiNote.AddNote(newNote);
                }
                else
                {
                    MultipleNote newMultiNote = new MultipleNote(0f, spawnTime, _laneHorizPositions, multiNotePrefab, multiNoteChildPrefab);
                    newMultiNote.AddNote(midiNoteMap[spawnTime]);
                    newMultiNote.AddNote(newNote);
                    midiNoteMap[spawnTime] = newMultiNote;
                }
                
            }  
        }
        List<INote> testNotes = new List<INote>();
        foreach(float timestamp in midiNoteMap.Keys)
        {
            testNotes.Add(midiNoteMap[timestamp]);
        }
        song.test = testNotes;
        Debug.Log($"Finished Generation for {song.MidiFile}");

        return song.test;
    }

}

