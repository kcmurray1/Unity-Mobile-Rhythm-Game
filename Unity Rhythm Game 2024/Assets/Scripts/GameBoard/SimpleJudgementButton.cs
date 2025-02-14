using System;
using UnityEngine;
using TMPro;



using UnityEngine.EventSystems;
using Unity.Mathematics;


public class SimpleJudgementButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
  public bool isHolding = false;

  public bool isPressed = false;

  // Callbacks
  public event Action OnGameEnd;
  public event Action<string> OnToggleGameSong;
  public event Action OnSoundEffect;
  private ScoreManager _scoreManager;
  private Action<float> _effectCallback;

  [SerializeField] private TextMeshProUGUI status_text;


  
  public void Initialize(Vector3 position, ScoreManager scoreManager, Action<float> effectCallback)
  {
      gameObject.transform.position = position;
      _scoreManager = scoreManager;
      _effectCallback = effectCallback;
  }
  private void _EndGame()
  {
      OnGameEnd?.Invoke();
  }

  private void _ToggleGameSong(string state)
  {
      OnToggleGameSong?.Invoke(state);
  }
  
  public void OnPointerDown(PointerEventData eventData)
  {
  
   status_text.text = "down";
    // isHolding = true;
    isPressed = true;
  }

  public void OnPointerUp(PointerEventData eventData)
  {
    status_text.text = "up";
  // isHolding = false;
    isPressed = false;  
  }

  public void onHold()
  {
    
  }

  public void _ManageTouch()
  {
    if(isHolding)
    {
      onHold();
    }
  }

  private bool _IsNote(Collider2D other)
  {
      return other.CompareTag("Note") || other.CompareTag("Note_Long_Start");
  }

  private void OnTriggerStay2D(Collider2D other) {
      float yDifference = other.transform.position.y - transform.position.y;

      // print($"distance: {Math.Abs(yDifference)}");
      // Start playing music
      if(other.CompareTag("start") && yDifference <= ScoreConstants.ACCURACY_PERFECT_THRESHHOLD)
      {
          _ToggleGameSong(other.tag);
          Destroy(other.gameObject);
          return;
      }

      // if(!other.CompareTag("end") && yDifference <= ScoreConstants.ACCURACY_PERFECT_THRESHHOLD)
      // {
      //   _scoreManager.OnNoteHit(yDifference);
      //   _effectCallback(yDifference);
      //   OnSoundEffect?.Invoke();
      //   Destroy(other.gameObject);
      //   isPressed = false;
      // }

      if(isPressed && !other.CompareTag("end"))
      {
        _scoreManager.OnNoteHit(yDifference);
        _effectCallback(yDifference);
        OnSoundEffect?.Invoke();
        Destroy(other.gameObject);
        isPressed = false;
      }
  }

  private void OnTriggerExit2D(Collider2D other)
  {
    float yDifference = other.transform.position.y - transform.position.y;
    Destroy(other.gameObject);
    if(other.CompareTag("end"))
    {
      _ToggleGameSong(other.tag);
      _EndGame();
      return;
    }
  
    if(math.abs(yDifference) > 1f)
    {
      print($"missed {other.tag}");
      _scoreManager.OnNoteMiss();
    }
  }
  void Update()
  {
    _ManageTouch();
  }
}
