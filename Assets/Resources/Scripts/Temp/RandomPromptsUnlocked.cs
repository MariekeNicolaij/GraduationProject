using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RandomPromptsUnlocked : MonoBehaviour
{
    public Text text;


    void Start()
    {
        TextAsset file = (TextAsset)Resources.Load("Prompts", typeof(TextAsset));

        string[] row = file.text.Trim().Split('\n');
        int prompts = row.Length - 1; // First row doesn't count as a prompt

        text.text = Random.Range(0, prompts + 1) + " / " + prompts;
    }
}
