using System;

[Serializable]
public class MidiNote
{

    public bool IsLongNote{get;}
    public int  LaneIndex{get; set;}

    public float Timestamp{get;}

    public int Id {get;}

    public MidiNote(bool isLongNote, int laneIndex, float timestamp, int id)
    {
        IsLongNote = isLongNote;
        LaneIndex = laneIndex;
        Timestamp = timestamp;
        Id=id;
    }

    public override string ToString()
    {
        return $"Note: {Id} Timestamp: {Timestamp} Lane: {LaneIndex} IsLongNote: {IsLongNote}\n";
    }

}