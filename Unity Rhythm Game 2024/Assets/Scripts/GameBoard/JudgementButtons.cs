using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class <c>JudgementButton</c> destroys notes, starts/stops musics, and rates hit accuracy
/// </summary>
public class JudgementButton : MonoBehaviour {
    // Connected Managers
    [SerializeField] private TouchManager _touchManager;
    [SerializeField] private ScoreManager _scoreManager;

    [SerializeField] private SoundManager _soundManager;


    // Song Events
    public event Action OnSoundEffect;
    public event Action<string> OnToggleGameSong;
    public event Action OnGameEnd;

    // Judgement Button attributes
    private bool hasNote;
    private Queue<GameObject> _activeNotes;
    [SerializeField]
    public int Id;
    private bool isHolding;
    private bool _autoplay;
    private static string[] IGNORED_NOTE_TAGS = {"start", "end", "Inactive"};
    private static string[] LONG_NOTE_TAGS = {"Note_Long_Start", "Note_Long"};
    public void Initialize(TouchManager touchManager, ScoreManager scoreManager, SoundManager soundManager, Vector3 position, int id, bool autoplay=false)
    {
        // Connect to TouchManager
        _touchManager = touchManager;
        _touchManager.OnTouch += TouchPressed;
        _touchManager.OnHold += TouchHold;
        _touchManager.OnHoldRelease += TouchHoldRelease;
        // Connect to ScoreManager
        _scoreManager = scoreManager;
        // Connect to SoundManager
        _soundManager = soundManager;

        hasNote = false;
        isHolding = false;
        gameObject.transform.position = position;
        this.Id = id;
        _activeNotes = new Queue<GameObject>();

        _autoplay = autoplay;
    }

    // Unsubscribe from _touchManager
    void OnDestroy()
    {
        if (_touchManager != null)
        {
            _touchManager.OnTouch -= TouchPressed;
        }
    }

    private void _ToggleGameSong(string state)
    {
        OnToggleGameSong?.Invoke(state);
    }
    
    private void _PlayEffect()
    {
        OnSoundEffect?.Invoke();
    }
    
    void TouchHoldRelease(int releasedButtonId)
    {
        if (releasedButtonId != Id){return;}
        isHolding = false;
    }

    void TouchHold(int pressedButtonId)
    {
        if (pressedButtonId != Id){return;}
        isHolding = true;
    }
    // Button was tapped
    void TouchPressed(int pressedButtonId)
    {
        if (pressedButtonId != Id || _activeNotes.Count == 0){return;}
        GameObject temp = _activeNotes.Dequeue();
        if (temp == null){return;}
        // NOTE: setting hasNote to false here resolves bug where OnTriggerExit2D is called
        // after a note is destroyed on top of the JudgementButton
        hasNote = false;
        _scoreManager.OnNoteHit(temp.gameObject.transform.position.y - transform.position.y);
        Destroy(temp);
        // _DestroyNote();
        _soundManager.PlayEffect();
        
    }
    
    private bool _IsNote(Collider2D other)
    {
        return other.CompareTag("Note") || other.CompareTag("Note_Long_Start");
    }
    //Game is over
    private void _EndGame()
    {
        OnGameEnd?.Invoke();
    }
    // Triggered by Notes, mark them for destruction
    private void OnTriggerEnter2D(Collider2D other) {
        // Ignore deactivated notes    
        if (IGNORED_NOTE_TAGS.Contains(other.tag)) {return;}
        _activeNotes.Enqueue(other.gameObject);
        hasNote = true; 
    }

    private void OnTriggerStay2D(Collider2D other) {
        float yDifference = other.transform.position.y - transform.position.y;
        // Start playing music
        if(other.CompareTag("start") && yDifference <= 0.3f)
        {
            _ToggleGameSong(other.tag);
        }
        // Autoplay hits perfect notes
        if(_autoplay && (_IsNote(other) || other.CompareTag("Note_Long")) && yDifference <= 0.3f)
        {
            TouchPressed(Id);
            return;
        }
        if (isHolding && other.CompareTag("Note_Long"))
        {
            TouchPressed(Id);
        }
    }
    // Triggered by Notes, Destroy note after leaving judgement button   
    private void OnTriggerExit2D(Collider2D other) {
        if(IGNORED_NOTE_TAGS.Contains(other.tag))
        {
            Destroy(other.gameObject);
            if (other.CompareTag("end"))
            {
                _ToggleGameSong(other.tag);
                _EndGame();
                // Debug.Log(_scoreManager.ToString());
            }
            return;
        }
        // Missed start of long Note or Stopped holding the long note
        if (LONG_NOTE_TAGS.Contains(other.tag) && hasNote)
        {
            NoteLong longNote = other.GetComponent<NoteLong>();
            // Mark all segments in the long note as 'Inactive'
            if (!longNote)
            {
                other.gameObject.transform.parent.GetComponent<NoteLong>().Clear();
            }
            // Count long note miss as 1 miss
            _scoreManager.OnNoteMiss();
        }
        // Missed Note
        if(_IsNote(other) && hasNote)
        {
            _scoreManager.OnNoteMiss();
            _activeNotes.Dequeue();
        }
        Destroy(other.gameObject);
      
    }
    
}