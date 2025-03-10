using System;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using Unity.Mathematics;
using System.Collections.Generic;
using Unity.VisualScripting;


public class SimpleJudgementButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
  public bool isHolding = false;

  public bool isPressed = false;

  public bool IsAutoPlay;

  private float _timePressed;

  // Callbacks
  public event Action OnGameEnd;
  public event Action<string> OnToggleGameSong;
  public event Action OnSoundEffect;
  private Action<float> _effectCallback;

  private ScoreManager _scoreManager;

  [SerializeField] private BoxCollider2D _hitBox;

  public Vector2 offset;

  private HashSet<int> idk;

  // FIXME: used to debug touch status
  [SerializeField] private TextMeshProUGUI status_text;

  public void Initialize(Vector3 position, ScoreManager scoreManager, Action<float> effectCallback, bool isAutoPlay=false)
  {
    IsAutoPlay = isAutoPlay;
    offset = _hitBox.offset;
    idk =  new HashSet<int> {
    LayerMask.NameToLayer("Note"),
    LayerMask.NameToLayer("HoldableNote")
    };
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

  public void _ManageTouch()
  {
    if(isPressed)
    {
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

  private void _HandleAutoPlay(GameObject other, float yDifference)
  {
    if(other.layer == LayerMask.NameToLayer("HoldableNote"))
    {
      _HandleNoteHit(yDifference, other.gameObject, playSoundEffect: false);
    }
    else if(other.layer == LayerMask.NameToLayer("Note"))
    {
      _HandleNoteHit(yDifference, other.gameObject);
    }
  }

  private void _Foo(GameObject other, float yDifference)
  {
    // if(Math.Abs(yDifference) >= ScoreConstants.ACCURACY_PERFECT_THRESHHOLD)
    // {
    //   print(yDifference);
    //   Vector2 offset = gameObject.GetComponent<BoxCollider2D>().offset;
    //   gameObject.GetComponent<BoxCollider2D>().offset = new Vector2(offset.x, offset.x + yDifference);

    // }
    
    if((isHolding || isPressed) && other.layer == LayerMask.NameToLayer("HoldableNote") && yDifference <= ScoreConstants.ACCURACY_PERFECT_THRESHHOLD)
      {
        _HandleNoteHit(yDifference, other, playSoundEffect: false);
      }
      else if(!isHolding && isPressed && other.layer == LayerMask.NameToLayer("Note"))
      {        
        _HandleNoteHit(yDifference, other);
        isPressed = false;
      }
  }

  private void _HandleNoteHit(float hitDifference, GameObject objectToDestory, bool playSoundEffect=true)
  {
     if(playSoundEffect)
    {
      OnSoundEffect?.Invoke();
    }
    _scoreManager.OnNoteHit(hitDifference);
    _effectCallback(hitDifference);
    Destroy(objectToDestory);
  }



  private void OnTriggerStay2D(Collider2D other) {
      float yDifference = other.transform.position.y - transform.position.y;

      // Start/Stop playing music
      if(other.gameObject.layer == LayerMask.NameToLayer("SoundTrigger") && yDifference <= ScoreConstants.ACCURACY_PERFECT_THRESHHOLD)
      {
          _ToggleGameSong(other.tag);
          Destroy(other.gameObject);
          if(other.CompareTag("end"))
          {
            _EndGame();
          }
          return;
      }

      if(IsAutoPlay && (yDifference <= ScoreConstants.ACCURACY_PERFECT_THRESHHOLD))
      {
        _HandleAutoPlay(other.gameObject, yDifference);
        return;
      }

      _Foo(other.gameObject, yDifference);

  }

  private void OnTriggerExit2D(Collider2D other)
  {
    float yDifference = other.transform.position.y - transform.position.y;
    Destroy(other.gameObject);
  
    if(idk.Contains(other.gameObject.layer) && math.abs(yDifference) > 1f)
    {
      _scoreManager.OnNoteMiss();
    }
  }
  void Update()
  {
    _ManageTouch();
  }
}
