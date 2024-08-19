using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "SongDataScriptableObject", menuName = "ScriptableObjects/Song", order = 0)]
public class SongDataScriptableObject : ScriptableObject
{
    public Dictionary<float, List<MidiNote>> EasyNoteMap {get; set;}
    public Dictionary<float, List<MidiNote>> MediumNoteMap {get; set;}
    public Dictionary<float, List<MidiNote>> HardNoteMap {get; set;}
    public string Name {get; set;}
    public int Bpm {get; set;}
    public int TopScore {get; set;}
    public string MidiFile {get; set;}
    public AudioClip SongFile {get; set;}

}
