using System;
using UnityEngine;
/// <summary>
/// Class <c>JudgementButton</c> destroys notes, starts/stops musics, and rates hit accuracy
/// </summary>
public class JudgementButton : MonoBehaviour {
    // Connected Managers
    [SerializeField] private TouchManager _touchManager;
    [SerializeField] private ScoreManager _scoreManager;

    [SerializeField] private SoundManager _soundManager;

    private GameObject _overlappedNoteObject;

    // Judgement Button attributes
    private bool hasNote;
    [SerializeField]
    public int Id;
    private bool isHolding;
    private bool _autoplay;
    public void Initialize(TouchManager touchManager, ScoreManager scoreManager, SoundManager soundManager, Vector3 position, int id)
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

        // Debug
        _autoplay = false;
    }

    // Unsubscribe from _touchManager
    void OnDestroy()
    {
        if (_touchManager != null)
        {
            _touchManager.OnTouch -= TouchPressed;
        }
    }

    void TouchHoldRelease()
    {
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
        if (pressedButtonId != Id){return;}
        if(!_overlappedNoteObject){return;}
        // NOTE: setting hasNote to false here resolves bug where OnTriggerExit2D is called
        //  after _overlappedNoteObject is destroyed
        hasNote = false;
        _scoreManager.OnNoteHit(_overlappedNoteObject.gameObject.transform.position.y - transform.position.y);
        _DestroyNote();
        _soundManager.PlayEffect();
        
    }
    
    // Destroy overlapping gameObject
    private void _DestroyNote()
    {
        Destroy(_overlappedNoteObject);
        _overlappedNoteObject = null;
    }

    private bool _IsDeactivatedLongNote(Collider2D other)
    {
        return other.CompareTag("Inactive") || (!isHolding && other.CompareTag("Note_Long"));
    }

    private bool _IsMarkerNote(Collider2D other)
    {
        return other.CompareTag("start") || other.CompareTag("end");
    }

    // Triggered by Notes, mark them for destruction
    private void OnTriggerEnter2D(Collider2D other) {
        // Ignore deactivated notes    
        if (_IsDeactivatedLongNote(other)) {return;}
        if(_IsMarkerNote(other)){return;}
        _overlappedNoteObject = other.gameObject;
        hasNote = true; 
        if (isHolding && other.CompareTag("Note_Long"))
        {
            TouchPressed(Id);
        }
    
    }

    private void OnTriggerStay2D(Collider2D other) {
        if(other.CompareTag("start") && Math.Abs(other.transform.position.y - transform.position.y) <= 0.3f)
        {
            _soundManager.OnSongStateChange();
        }

        if(_autoplay && other.CompareTag("Note") && Math.Abs(other.transform.position.y - transform.position.y) <= 0.3f)
        {
            TouchPressed(Id);
        }
    }
    // Triggered by Notes, Destroy note after leaving judgement button   
    private void OnTriggerExit2D(Collider2D other) {
        string otherTag = other.tag;
       
        if(!other.CompareTag("end") && !other.CompareTag("start"))
        {
            // Missed long Note
            if (otherTag == "Note_Long_Start" && hasNote)
            {
                other.gameObject.transform.parent.GetComponent<NoteLong>().Clear();
            }
            if(otherTag != "Inactive" && _overlappedNoteObject && hasNote)
            {
                _scoreManager.OnNoteMiss();
                _DestroyNote();
            }
        }
        else
        {
            //FIXME: used for debugging scoreManager
            Debug.Log(_scoreManager.ToString());
        }
        Destroy(other.gameObject);
        
        
 
    }
    
}