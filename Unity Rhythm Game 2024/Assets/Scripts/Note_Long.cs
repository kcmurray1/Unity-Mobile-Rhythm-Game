using UnityEngine;
using System.Collections.Generic;
public class NoteLong : MonoBehaviour
{
    [SerializeField] private GameObject child;
    [SerializeField] private GameObject head;

    [SerializeField] private Transform _tailTransform;

    private List<GameObject> _childNotes;

    private int numChildren;
    public float NoteSpeed;
    public bool isLongNote;

    void Awake()
    {  
      _childNotes = new List<GameObject>();
      numChildren = 5;
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
      NoteSpeed = 8f;
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
      // GetComponent<SpriteRenderer>().color = Color.gray;

    }
}
