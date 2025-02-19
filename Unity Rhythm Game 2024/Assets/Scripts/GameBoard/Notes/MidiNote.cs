using System;
using UnityEngine;
/// <summary>
/// Class <c>MidiNote</c> stores MidiNote informated required to spawn
/// </summary>
[Serializable]
public class MidiNote
{
    /// <summary>
    /// Determine whether this is a LongNote or Note
    /// </summary>
    public bool IsLongNote = false;
    /// <summary>
    /// The lane this note will be spawned
    /// </summary>
    public int  LaneIndex = 0;
    /// <summary>
    /// The time in (float)seconds that this note will be spawned
    /// </summary>
    public float Timestamp = 0;
    /// <summary>
    /// The number in the spawn sequence
    /// </summary>
    public int Id;
    public double NoteLength;
    public int NumQuarterNotes;
    /// <summary>
    /// Create a MidiNote
    /// </summary>
    /// <param name="noteLength">Length of note in seconds</param>
    /// <param name="laneIndex">The lane this note will be spawned</param>
    /// <param name="timestamp">The time in seconds that this note will be spawned</param>
    /// <param name="id">The number in the spawn sequence</param>
    public MidiNote(double noteLength, int laneIndex, float timestamp, int id)
    {
        NumQuarterNotes = (int)(noteLength / 0.414);
        IsLongNote = NumQuarterNotes > 1;
        LaneIndex = laneIndex;
        Timestamp = timestamp;
        NoteLength = noteLength;
        Id=id;
    }

    public override string ToString()
    {
        return $"Note: {Id} Timestamp: {Timestamp} Lane: {LaneIndex} IsLongNote: {IsLongNote}\n";
    }


}