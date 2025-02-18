using System;
using UnityEngine;
using TMPro;



using UnityEngine.EventSystems;
using Unity.Mathematics;
using UnityEngine.SocialPlatforms.Impl;


public class SimpleJudgementButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
  public bool isHolding = false;

  public bool isPressed = false;

  private float _timePressed;

  // Callbacks
  public event Action OnGameEnd;
  public event Action<string> OnToggleGameSong;
  public event Action OnSoundEffect;
  private ScoreManager _scoreManager;
  private Action<float> _effectCallback;

  [SerializeField] private TextMeshProUGUI status_text;


  
  public void Initialize(Vector3 position, ScoreManager scoreManager, Action<float> effectCallback)
  {
    _timePressed = 0f;
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
    isPressed = true;
  }

  public void OnPointerUp(PointerEventData eventData)
  {
    status_text.text = "up";
    isPressed = false;  
  }

  public void onHold()
  {
    print("holding~!0");
  }

  public void _ManageTouch()
  {
    if(isPressed)
    {
      print("yay");
      _timePressed += Time.deltaTime;
    }
    else
    {
      _timePressed = 0f;
      isHolding = false;
    }
    if(_timePressed >= 0.25f)
    {
      status_text.text = "hold";
      isHolding = true;
    }
  }

  private bool _IsNote(Collider2D other)
  {
      return other.CompareTag("Note") || other.CompareTag("Note_Long_Start");
  }

  private void _HandleNoteHit(float hitDifference, GameObject objectToDestory, bool playSoundEffect=true)
  {
    _scoreManager.OnNoteHit(hitDifference);
    _effectCallback(hitDifference);
    if(playSoundEffect)
    {
      OnSoundEffect?.Invoke();
    }
    Destroy(objectToDestory);
  }

  private void OnTriggerStay2D(Collider2D other) {
      float yDifference = other.transform.position.y - transform.position.y;

      // Start playing music
      if(other.CompareTag("start") && yDifference <= ScoreConstants.ACCURACY_PERFECT_THRESHHOLD)
      {
          _ToggleGameSong(other.tag);
          Destroy(other.gameObject);
          return;
      }
      if(other.CompareTag("end") && yDifference <= ScoreConstants.ACCURACY_PERFECT_THRESHHOLD)
      {
        Destroy(other.gameObject);
        _ToggleGameSong(other.tag);
        _EndGame();
        return;
      }

      if(other.gameObject.layer == LayerMask.NameToLayer("Default") && yDifference <= ScoreConstants.ACCURACY_PERFECT_THRESHHOLD)
      {
        if(other.gameObject.layer == LayerMask.NameToLayer("HoldableNote"))
        {
          _HandleNoteHit(yDifference, other.gameObject, playSoundEffect: false);
        }
        else
        {
          _HandleNoteHit(yDifference, other.gameObject);
        }
        
        return;
      }

      if(isHolding && other.gameObject.layer == LayerMask.NameToLayer("HoldableNote") && yDifference <= ScoreConstants.ACCURACY_PERFECT_THRESHHOLD)
      {
        _scoreManager.OnNoteHit(yDifference);
        _effectCallback(yDifference);
        OnSoundEffect?.Invoke();
        Destroy(other.gameObject);
      }
      else if(!isHolding && isPressed)
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
  
    if(other.gameObject.layer == LayerMask.NameToLayer("Default") && math.abs(yDifference) > 1f)
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
