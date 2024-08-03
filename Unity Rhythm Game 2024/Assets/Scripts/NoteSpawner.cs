using System.Collections;
using UnityEngine;

public class NoteSpawner : MonoBehaviour {

    public GameObject noteObject;
    [SerializeField] private GameObject SpawnerObject;

    public void Spawn()
    {
        GameObject newNote = Instantiate(noteObject, SpawnerObject.transform.position, Quaternion.identity);
    }

    void Start()
    {

        StartCoroutine(SpawnNote());

    }

    IEnumerator SpawnNote()
    {
        for (int i =0; i < 10; i++)
        {
            yield return new WaitForSeconds(3);
            Spawn();
        }
    }

}