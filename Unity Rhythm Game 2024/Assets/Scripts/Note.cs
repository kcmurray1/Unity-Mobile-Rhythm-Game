using UnityEngine;

public class Note : MonoBehaviour
{
    public float NoteSpeed;

    void Awake()
    {
        NoteSpeed = 12f;
    }

    void Update()
    {
      transform.position += Vector3.down * NoteSpeed * Time.deltaTime;
    }
    
    void OnDestroy() {
      Destroy(gameObject);  
    }
}
