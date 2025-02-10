using UnityEngine;
using TMPro;
using System;

public class EffectsManager : MonoBehaviour {

    private const float ACCURACY_PERFECT_THRESHHOLD = 0.4f;
    private const float ACCURACY_GREAT_THRESHHOLD = 0.7f;
    private const float ACCURACY_GOOD_THRESHHOLD = 1f;
   [SerializeField] private TMP_Text hit_type_text;


    void Awake()
    {
        hit_type_text.enabled = false;
    }

    public void UpdateDisplay(float accuracy)
    {

        accuracy = Math.Abs(accuracy);
        String text_to_display = "";
        if(accuracy <= ACCURACY_PERFECT_THRESHHOLD)
        {
            text_to_display = "Perfect";
        }
        else if(accuracy <= ACCURACY_GREAT_THRESHHOLD)
        {

            text_to_display = "Great";
        }
        else
        {
            text_to_display = "Meh";
        }
        hit_type_text.text = text_to_display;
        hit_type_text.enabled = true;
    }

}