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
      int numChildren = 5;
      Initialize(numChildren);  
      NoteSpeed = 12f;
    }

    public void Initialize(int numChildren)
    {
      _childNotes = new List<GameObject>();
      for(int i = 1; i < numChildren; i++)
      {
        GameObject toSpawn = child;
        if (i == numChildren - 1)
        {
          toSpawn = head;
        }
        GameObject newChild = Instantiate(toSpawn, _tailTransform, worldPositionStays: true);
        newChild.transform.position = _tailTransform.position +
                new Vector3(newChild.transform.position.x, newChild.transform.position.y * ((2 * i) - 1), 
                            newChild.transform.position.z);
        _childNotes.Add(newChild);
        
      }
      _childNotes.Add(gameObject);
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
