using System;
using UnityEngine;
[Serializable]
public class Note : MonoBehaviour
{
    public float NoteSpeed;

    protected void Awake()
    {
        NoteSpeed = 10f;
    }

    protected void Update()
    {
      transform.position += Vector3.down * NoteSpeed * Time.deltaTime;
    }
    
    protected void OnDestroy() {
      Destroy(gameObject);  
    }
}
