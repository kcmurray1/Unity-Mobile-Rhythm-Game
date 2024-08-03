using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note : MonoBehaviour
{
    public float NoteSpeed;

    void Awake()
    {
        NoteSpeed = 10f;
    }

    void Update()
    {
      transform.position += Vector3.down * NoteSpeed * Time.deltaTime;
    }
    
    void OnDestroy() {
      Destroy(gameObject);  
    }
}
