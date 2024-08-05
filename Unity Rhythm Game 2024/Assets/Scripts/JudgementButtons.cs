using UnityEngine;

public class JudgementButton : MonoBehaviour {
    [SerializeField] private TouchManager _touchManager;
    [SerializeField] private ScoreManager _scoreManager;

    private GameObject _overlappedNote;

    public delegate void NoteHitHandler();
    public delegate void NoteMissHandler();

    public event NoteHitHandler OnNoteHit;
    public event NoteMissHandler OnNoteMiss;

    private bool hasNote;
    private int id;

    public void Initialize(TouchManager touchManager, ScoreManager scoreManager, Vector3 position, int id)
    {
        _touchManager = touchManager;
        _touchManager.OnTouch += TouchPressed;
        _scoreManager = scoreManager;
        OnNoteHit += scoreManager.OnNoteHit;
        OnNoteMiss += scoreManager.OnNoteMiss;
        hasNote = false;
        gameObject.transform.position = position;
        this.id = id;

    }

    // Unsubscribe from _touchManager
    void OnDestroy()
    {
        if (_touchManager != null)
        {
            _touchManager.OnTouch -= TouchPressed;
        }
    }

    // Button was touched
    void TouchPressed(JudgementButton pressedButton)
    {
        if (pressedButton.id != id)
        {
            return;
        }
        if(_overlappedNote != null)
        {
            // NOTE: setting hasNote to false here resolves bug where OnTriggerExit2D is called
            //  after _overlappedNote is destroyed
            hasNote = false;
            _DestroyNote();
            OnNoteHit?.Invoke();
        }
        
    }
    
    // Destory overlapping gameObject
    private void _DestroyNote()
    {
        Destroy(_overlappedNote);
        _overlappedNote = null;
    }


    // Triggered by Notes, mark them for destruction
    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.CompareTag("Note"))
        {
            _overlappedNote = other.gameObject;
            hasNote = true;
        }
    }

    // Triggered by Notes, Destroy note after leaving judgement button   
    private void OnTriggerExit2D(Collider2D other) {
        if(_overlappedNote && hasNote)
        {
            _DestroyNote();
            OnNoteMiss?.Invoke();
        }
        
    }
    
}