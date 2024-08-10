using System.Collections; // IEnumerator()
using System.Collections.Generic; // Dictionary
using UnityEngine;
using System.Linq; //ToList()
using System;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
public class NoteSpawner : MonoBehaviour 
{

    // Object to spawn
    public GameObject notePrefab;
    public GameObject _longNotePrefab;

    // Dict containing where lane number is the key and lane position is the value
    private Dictionary<int, float> _laneHorizPositions;
    // Spawner Object(itself)
    [SerializeField] private GameObject _spawnerObject;

    private Dictionary<float, int> testSong = new Dictionary<float, int>()
    // {
    //     {0,1},
    //     {1,1},
    //     {2,1},
    //     {3,1},
    // };
      {
        {0,0}, 
        {0.413793f, 0},
        { 0.827586f, 0},
        {1.241379f,0},
        { 1.448276f, 0},
        {1.655172f, 1},
        {1.75862f, 2},
        {1.862069f, 0},
    };
    
    // void Start()
    // {
    //     List<float> pos = new List<float>{0f};
    //     Initialize(pos);
    // }
    // Initializing the lanepositions sets the spawn boundaries
    // Ex): Receiving three lan positions [-4,0,4] will update _laneHorizPositions to
    // {0: -4, 1: 0, 2: 4} where lane 0 is located at x = -4 and lane 1 is located at x = 0
    public void Initialize(List<float> lanePositions)
    {
        _InitializeLanePositions(lanePositions);
        // List<float> timstamps = GetSongData("Assets/Songs/Test_C_Scale.mid", 145);
        // Dictionary<float, int> testMap = new Dictionary<float, int>();
        // foreach (var timstamp in timstamps)
        // {
        //     testMap[timstamp] = 0;
        // }
        List<float> timings = GetSongData("Assets/Songs/Test_Twelve.mid", 95);
        Dictionary<float, int> songMap = new Dictionary<float, int>();
        System.Random idk = new System.Random();
        foreach(float time in timings)
        {
            // Assign lane for each timestamp
            songMap[time] = idk.Next(0,_laneHorizPositions.Count);
        }
        StartCoroutine(SpawnNote(songMap));
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
    }
    
   
    // Spawn an object
    public void Spawn(int laneIndex)
    {
        if (laneIndex == 20)
        {
             GameObject newNote = Instantiate(_longNotePrefab, _spawnerObject.transform);
             newNote.transform.position = new Vector3(_laneHorizPositions[laneIndex], 
                    newNote.transform.position.y, newNote.transform.position.z);
        }
        else
        {
            GameObject newNote = Instantiate(notePrefab, _spawnerObject.transform);
            // Adjust horizontal position
            newNote.transform.position = new Vector3(_laneHorizPositions[laneIndex], 
                    newNote.transform.position.y, newNote.transform.position.z);
        }
    }

    IEnumerator SpawnNote(Dictionary<float, int> noteMap)
    {   
        GameObject newNote = Instantiate(notePrefab, _spawnerObject.transform);
        // Adjust horizontal position
        newNote.transform.position = new Vector3(_laneHorizPositions[0], 
                newNote.transform.position.y, newNote.transform.position.z);
        newNote.tag = "start";
        newNote.gameObject.GetComponent<SpriteRenderer>().color = Color.yellow;

        float prevTime = 0;
        foreach(float timestamp in noteMap.Keys)
        {
            if(timestamp - prevTime != 0)
            {   
                yield return new WaitForSeconds(timestamp - prevTime);
            }
           
            prevTime = timestamp;
            Spawn(noteMap[timestamp]);
        }

        newNote = Instantiate(notePrefab, _spawnerObject.transform);
        // Adjust horizontal position
        newNote.transform.position = new Vector3(_laneHorizPositions[0], 
                newNote.transform.position.y, newNote.transform.position.z);
        newNote.tag = "end";
        newNote.gameObject.GetComponent<SpriteRenderer>().color = Color.yellow;
    }

    // Read midifile
    public List<float> GetSongData(string fileName, float bpm)
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
        Dictionary<float, int> noteTimeDict = new Dictionary<float, int>();
        List<float> newMap = new List<float>(); 
        //Get Note timings from midiFile
        var notes = midiFile.GetNotes().ToList();

        int noteNum = 0;

        //Iterate each note
        foreach(var note in notes)
        {
            //Get the timestamp of when a note is played
            float spawnTime = (float)note.TimeAs<MetricTimeSpan>(newTempoMap).TotalSeconds;
            // Debug.Log($"Note {noteNum} at {spawnTime}");
            noteTimeDict[spawnTime] = noteNum;
            newMap.Add(spawnTime);
            noteNum++;     
        }
        Debug.Log($"Finished Generation for {fileName}");
        return newMap;
    }

    private void NoteInfo(Melanchall.DryWetMidi.Interaction.Note note)
    {
        Debug.Log($"Length: {note.Length}");
    }

}