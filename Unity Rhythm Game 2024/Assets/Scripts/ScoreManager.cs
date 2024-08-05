using UnityEngine;
using TMPro;
public class ScoreManager : MonoBehaviour
{
    private const string DEFAULT_SCORE_UI = "000000000";
    private const string DEFAULT_MULTIPLIER_UI = "x0";
    private const int BASE_SCORE_VALUE = 100;
    private TextMeshProUGUI _textScore;
    private TextMeshProUGUI _textMultiplier;
    private int _missCount;
    private int _hitCount;
    private int _multiplier;
    private int _totalScore;

    // Connect to UI elements and set initial values
    public void Initialize()
    {
        _textScore = GameObject.Find("Text_Score").GetComponent<TextMeshProUGUI>();
        _textMultiplier = GameObject.Find("Text_Multiplier").GetComponent<TextMeshProUGUI>();
        Reset();
    }

    // Reset score, multiplier, counts, etc..
    public void Reset()
    {
        _multiplier = 0;
        _missCount = 0;
        _hitCount = 0;
        _totalScore = 0;
        _textScore.text = DEFAULT_SCORE_UI;
        _textMultiplier.text = DEFAULT_MULTIPLIER_UI;
    }

    // Update score based on quality it
    public void OnNoteHit()
    {
        _hitCount++;
        _multiplier++;
        _totalScore += BASE_SCORE_VALUE;
        _textScore.text = _totalScore.ToString("D9");
        _textMultiplier.text = "x" + _multiplier.ToString();
    }

    // Update multiplier and count of misses
    public void OnNoteMiss()
    {
        _missCount++;
        _multiplier = 0;
        Debug.Log(_missCount + "Misses");
    }
}
