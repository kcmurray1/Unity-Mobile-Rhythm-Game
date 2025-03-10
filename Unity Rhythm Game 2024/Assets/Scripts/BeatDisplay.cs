using UnityEngine;

public class BeatDisplay : MonoBehaviour
{
    public SoundManager SoundManager;
    public Transform localInfo;

    bool isExpaned;
    float time;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SoundManager = GameObject.FindFirstObjectByType<SoundManager>();
        time = 0f;
        isExpaned = false;
        SoundManager.OnQuarterNote += AdjustSize;
    }

    public void AdjustSize()
    {
        if(!isExpaned)
        {
            gameObject.transform.localScale += new Vector3(0.1f, 0.1f, 0f);
        }
        else
        {
            gameObject.transform.localScale -= new Vector3(0.1f, 0.1f, 0f);
        }
        isExpaned = !isExpaned;
        
    }
}
