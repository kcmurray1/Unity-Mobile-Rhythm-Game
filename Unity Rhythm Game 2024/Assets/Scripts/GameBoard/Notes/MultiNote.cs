using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.UIElements;

[Serializable]
public class MultiNote : Note
{
    public GameObject notePrefab;

    // Create note(s)
    public void Initialize(List<float> positions, int numNotes)
    {
        //Create numNotes objects
        for(int i = 0; i < numNotes; i++)
        {
            // Create instance
            GameObject newNote = Instantiate(notePrefab, gameObject.transform);
            // Adjust horizontal position
            newNote.transform.position = new Vector3(positions[i], 
                    newNote.transform.position.y, newNote.transform.position.z);
            
        }
    }
}