using System;
using UnityEngine;

public class BeatManager : MonoBehaviour
{

    public int Bpm;

    public double QuarterNoteLength;
    public double BarNoteLength {
        get { return QuarterNoteLength * 4;}
        set { BarNoteLength = value;}
    }

    public double NextBeatTime;
    private double _currentTime;

    // Event for Quarter beat
    public event Action OnQuarterBeat;

    public void Initialize(int bpm)
    {
        Bpm = bpm;
        QuarterNoteLength = 60d / Bpm;
        NextBeatTime = AudioSettings.dspTime + QuarterNoteLength;


    }

    // Update is called once per frame
    void Update()
    {
        _currentTime = AudioSettings.dspTime;
        if(_currentTime > NextBeatTime)
        {
            NextBeatTime += QuarterNoteLength;
            OnQuarterBeat?.Invoke();

        }
    }


}
