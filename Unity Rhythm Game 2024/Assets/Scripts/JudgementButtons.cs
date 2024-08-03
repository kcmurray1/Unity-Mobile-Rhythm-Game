using UnityEngine;


public class JudgementButtons : MonoBehaviour {


    [SerializeField] private TouchManager touchManager;
    [SerializeField] private ScoreManager scoreManager;

    private GameObject _overlappedNote;

    public delegate void NoteHitHandler();
    public delegate void NoteMissHandler();

    public event NoteHitHandler OnNoteHit;
    public event NoteMissHandler OnNoteMiss;


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
    }

    void TouchPressed()
    {
        if(_overlappedNote != null)
        {
            OnNoteHit?.Invoke();
            Destroy(_overlappedNote);
        }
        
    }
    
    void OnDestroy()
    {
        if (touchManager != null)
        {
            touchManager.OnTouch -= TouchPressed;
        }
    }


    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.CompareTag("Note"))
        {
            _overlappedNote = other.gameObject;
        }
    }

    // Destroy note after leaving judgement button   
    private void OnTriggerExit2D(Collider2D other) {
        if(other.gameObject.CompareTag("Note"))
        {
            Destroy(_overlappedNote);
            OnNoteMiss?.Invoke();
        }
        
    }
    
}