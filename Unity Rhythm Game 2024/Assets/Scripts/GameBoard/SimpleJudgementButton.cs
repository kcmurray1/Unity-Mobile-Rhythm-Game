using System;
using UnityEngine;
using TMPro;



using UnityEngine.EventSystems;
using Unity.VisualScripting; // Required for Event Triggers


public class SimpleJudgementButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
  public bool isHolding = false;

  public bool isPressed = false;

  // Callbacks
  public event Action OnGameEnd;

  public event Action<string> OnToggleGameSong;
  private Action<float> _noteHitCallback;
  private Action<float> _effectCallback;

  [SerializeField] private TextMeshProUGUI status_text;


  
  public void Initialize(Vector3 position, Action<float> noteHitCallback, Action<float> effectCallback)
  {
      gameObject.transform.position = position;
      _noteHitCallback = noteHitCallback;
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
    print("Button released!");
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

      // Start playing music
      if(other.CompareTag("start") && yDifference <= 0.3f)
      {
          _ToggleGameSong(other.tag);
      }

      if(isPressed)
      {
        _noteHitCallback(yDifference);
        _effectCallback(yDifference);
        Destroy(other.gameObject);
        isPressed = false;
      }
  }

  private void OnTriggerExit2D(Collider2D other)
  {

    Destroy(other.gameObject);
    if (other.CompareTag("end"))
    {
      _ToggleGameSong(other.tag);
      _EndGame();
    }
  }
  void Update()
  {
    _ManageTouch();
  }
}
