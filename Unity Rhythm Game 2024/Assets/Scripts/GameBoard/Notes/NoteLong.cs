using UnityEngine;
using System.Collections.Generic;
using System;
[Serializable]
public class NoteLong : Note
{
    [SerializeField] private GameObject child;
    [SerializeField] private GameObject head;

    [SerializeField] private Transform _tailTransform;

    [SerializeField] private List<GameObject> _childNotes = new List<GameObject>();

    public bool isLongNote;


    public void Initialize(int NumChildren)
    {
      _childNotes = new List<GameObject>();
      for(int i = 1; i < NumChildren; i++)
      {
        GameObject toSpawn = child;
        
        if (i == NumChildren - 1)
        {
          toSpawn = head;
        }
        GameObject newChild = Instantiate(toSpawn, _tailTransform, worldPositionStays: true);
        float yOffset;
        if (i == 1)
        {
          yOffset = -0.8f;
        }
        else
        {
          yOffset = -0.8f - 0.95f * (i - 1);
        }
        newChild.transform.position = _tailTransform.position +
                new Vector3(newChild.transform.position.x, yOffset, 
                            newChild.transform.position.z);
        _childNotes.Add(newChild);
        
      }
      _childNotes.Add(gameObject);
      RelocateTail();
    }
    
    public void RelocateTail()
    {
      // Start of Long note
      GameObject head = _childNotes[_childNotes.Count - 2];
      float adjustedYOffset = Math.Abs(head.transform.position.y - transform.position.y);
      transform.position = new Vector3(transform.position.x, 
                           transform.position.y +  adjustedYOffset,
                           transform.position.z);
    }

    public void Clear()
    {
      foreach(GameObject child in _childNotes)
      {
        if(child != null)
        {
          child.GetComponent<SpriteRenderer>().color = Color.gray;
          child.tag = "Inactive";
        }
       
      }

    }
}
