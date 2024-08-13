using UnityEngine;
using System.Collections.Generic;
using Unity.Mathematics;
using System;
public class NoteLong : MonoBehaviour
{
    [SerializeField] private GameObject child;
    [SerializeField] private GameObject head;

    [SerializeField] private Transform _tailTransform;

    private List<GameObject> _childNotes;

    public float NoteSpeed;
    public bool isLongNote;

    void Awake()
    {  
      // int NumChildren = 5;
      // Initialize(NumChildren);  
      NoteSpeed = 12f;
    }

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
        float yOffset;//newChild.transform.position.y - (1 * (i - 1));
        if (i == 1)
        {
          yOffset = -0.8f;
        }
        else
        {
          yOffset = -0.8f - 0.95f * (i - 1);
        }
        
        // float yOffset = newChild.transform.position.y *((2 * i) - 1);
        Debug.Log(yOffset);
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
    void Update()
    {
      transform.position += Vector3.down * NoteSpeed * Time.deltaTime;
    }
    
    void OnDestroy() {
      Destroy(gameObject);  
    }

    public void Clear()
    {
      foreach(GameObject child in _childNotes)
      {
        child.GetComponent<SpriteRenderer>().color = Color.gray;
        child.tag = "Inactive";
      }

    }
}
