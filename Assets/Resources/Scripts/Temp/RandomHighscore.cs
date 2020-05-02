using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RandomHighscore : MonoBehaviour
{
    public Text text;


    void Start()
    {
        text.text = "Year " + Random.Range(1, 11) + " month " + Random.Range(1, 13);
    }
}
