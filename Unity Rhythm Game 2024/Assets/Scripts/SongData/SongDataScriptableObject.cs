using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;


[CreateAssetMenu(fileName = "SongDataScriptableObject", menuName = "ScriptableObjects/Song", order = 0)]
public class SongDataScriptableObject : ScriptableObject
{
    public Dictionary<float, List<MidiNote>> EasyNoteMap; 
    public Dictionary<float, List<MidiNote>> MediumNoteMap; 
    public Dictionary<float, List<MidiNote>> HardNoteMap;
    public string Name; 
    public int Bpm; 
    public int TopScore; 
    public string MidiFile; 
    public AudioClip SongClip; 

}
