using UnityEngine;

public class JudgementButton : MonoBehaviour {
    // Connected Managers
    [SerializeField] private TouchManager _touchManager;
    [SerializeField] private ScoreManager _scoreManager;

    [SerializeField] private SoundManager _soundManager;

    private GameObject _overlappedNote;

    // Event Handlers for SoundManager
    public delegate void SongStateHandler();
    public event SongStateHandler OnSongStateChange;

    // Judgement Button attributes
    private bool hasNote;
    [SerializeField]
    private int id;

    public void Initialize(TouchManager touchManager, ScoreManager scoreManager, SoundManager soundManager, Vector3 position, int id)
    {
        // Connect to TouchManager
        _touchManager = touchManager;
        _touchManager.OnTouch += TouchPressed;
        // Connect to ScoreManager
        _scoreManager = scoreManager;
        // Connect to SoundManager
        _soundManager = soundManager;
        OnSongStateChange += soundManager.OnSongStateChange; 

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
            _soundManager.PlayEffect();
            _scoreManager.OnNoteHit(_overlappedNote.gameObject.transform.position);
            _DestroyNote();
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
        if(other.gameObject.CompareTag("start"))
        {
            OnSongStateChange?.Invoke();
        }
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
            _scoreManager.OnNoteMiss();
        }
        
    }
    
}