using System.Collections; // IEnumerator()
using System.Collections.Generic; // Dictionary
using UnityEngine;
using System.Linq; //ToList()
using System;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using Melanchall.DryWetMidi.MusicTheory;
public class NoteSpawner : MonoBehaviour 
{

    // Object to spawn
    public GameObject noteObject;

    // Spawner Object(itself)
    [SerializeField] private GameObject SpawnerObject;
    
    // Spawn an object
    public void Spawn()
    {
        GameObject newNote = Instantiate(noteObject, SpawnerObject.transform.position, Quaternion.identity);
    }

    void Start()
    {
        List<float> notes = GetSongData("Assets/Songs/Test_C_Scale.mid", 145);
        StartCoroutine(SpawnNote(notes));
    }

    IEnumerator SpawnNote(List<float> notes)
    {
        foreach(float timestamp in notes)
        {
            yield return new WaitForSeconds(timestamp);
            Spawn();
        }
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

            Debug.Log($"Note {noteNum} at {spawnTime}");
            noteTimeDict[spawnTime] = noteNum;
            newMap.Add(spawnTime);
            noteNum++;
            
        }
        Debug.Log($"Finished Generation for {fileName}");
        return newMap;
    }

}