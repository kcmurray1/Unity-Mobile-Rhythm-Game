using UnityEngine;

public class JudgementButtons : MonoBehaviour {


    [SerializeField] private TouchManager touchManager;
    [SerializeField] private ScoreManager scoreManager;

    private GameObject _overlappedNote;

    public delegate void NoteHitHandler();
    public delegate void NoteMissHandler();

    public event NoteHitHandler OnNoteHit;
    public event NoteMissHandler OnNoteMiss;

    private bool hasNote;


    // Start is called before the first frame update
    void Start()
    {
        if (touchManager != null )
        {
            touchManager.OnTouch += TouchPressed;
        }   
        if (scoreManager != null)
        {
            OnNoteHit += scoreManager.OnNoteHit;
            OnNoteMiss += scoreManager.OnNoteMiss;
        }
        hasNote = false;
    }

    // Unsubscribe from touchManager
    void OnDestroy()
    {
        if (touchManager != null)
        {
            touchManager.OnTouch -= TouchPressed;
        }
    }

    // Button was touched
    void TouchPressed()
    {
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

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.CompareTag("Note"))
        {
            _overlappedNote = other.gameObject;
            hasNote = true;
        }
    }

    // Destroy note after leaving judgement button   
    private void OnTriggerExit2D(Collider2D other) {
        if(_overlappedNote && hasNote)
        {
            _DestroyNote();
            OnNoteMiss?.Invoke();
        }
        
    }
    
}