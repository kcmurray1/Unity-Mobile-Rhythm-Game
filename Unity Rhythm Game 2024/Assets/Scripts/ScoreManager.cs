using UnityEngine;
using TMPro;
using System;
using System.Collections.Generic;
public class ScoreManager : MonoBehaviour
{
    // Default score information
    private const string DEFAULT_SCORE_UI = "000000000";
    private const string DEFAULT_MULTIPLIER_UI = "x0";
    private const int BASE_SCORE_VALUE = 100;
    // Multiplier score information
    private const float ACCURACY_PERFECT_THRESHHOLD = 0.4f;
    private const float ACCURACY_GREAT_THRESHHOLD = 0.7f;
    private const float ACCURACY_GOOD_THRESHHOLD = 1f;
    private const double PERFECT_SCORE_MULTIPLIER = 2;
    private const double GREAT_SCORE_MULTIPLIER = 1.5;
    private const double GOOD_SCORE_MULTIPLIER = 1;
    // Types of hits
    private string[] HIT_TYPES = new string[]{"Perfect", "Great", "Good", "Miss"};
    private TextMeshProUGUI _textScore;
    private TextMeshProUGUI _textMultiplier;
    // Current score, counts, and multipler
    private Dictionary<string, int> _counts;
   
    private int _multiplier;
    private int _totalScore;

    // Connect to UI elements and set initial values
    public void Initialize()
    {
        _textScore = GameObject.Find("Text_Score").GetComponent<TextMeshProUGUI>();
        _textMultiplier = GameObject.Find("Text_Multiplier").GetComponent<TextMeshProUGUI>();
        _counts = new Dictionary<string, int>();
        foreach(string hitType in HIT_TYPES)
        {
            _counts[hitType] = 0;
        }
        Reset();
    }

    // Reset score, multiplier, counts, etc..
    public void Reset()
    {
        _multiplier = 0;
        foreach(string key in HIT_TYPES)
        {
            _counts[key] = 0;
        }
        _totalScore = 0;
        _textScore.text = DEFAULT_SCORE_UI;
        _textMultiplier.text = DEFAULT_MULTIPLIER_UI;
    }

    private double _CalcScore(float accuracy)
    {
        accuracy = Math.Abs(accuracy);
        if(accuracy <= ACCURACY_PERFECT_THRESHHOLD)
        {
            _counts["Perfect"]++;
            return PERFECT_SCORE_MULTIPLIER;
        }
        else if(accuracy <= ACCURACY_GREAT_THRESHHOLD)
        {
            _counts["Great"]++;
            return GREAT_SCORE_MULTIPLIER;
        }
        else
        {
            _counts["Good"]++;
            return GOOD_SCORE_MULTIPLIER;
        }
    }
    // Update score based on quality it
    public void OnNoteHit(float accuracy)
    {
        _multiplier++;
        _totalScore += (int)(BASE_SCORE_VALUE * _CalcScore(accuracy));
        _textScore.text = _totalScore.ToString("D9");
        _textMultiplier.text = "x" + _multiplier.ToString();
    }

    // Update multiplier and count of misses
    public void OnNoteMiss()
    {
        _counts["Miss"]++;
        _multiplier = 0;
        Debug.Log(_counts["Miss"] + "Misses");
    }

    public override string ToString()
    {
        string ret = "";
        foreach(string key in _counts.Keys)
        {
            ret += $"{key}: {_counts[key]}\n";
        }

        return ret;
    }
}
