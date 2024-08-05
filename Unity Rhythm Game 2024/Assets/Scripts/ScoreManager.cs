using System;
using UnityEngine;
public class ScoreManager : MonoBehaviour
{
    private int missCount;
    private int hitCount;


    private void Awake()
    {
        missCount = 0;
        hitCount = 0;
    }

    public void OnNoteHit()
    {
        hitCount++;
        Debug.Log(hitCount + "Hits!");
    }

    public void OnNoteMiss()
    {
        missCount++;
        
        Debug.Log(missCount + "Misses");
    }
}
