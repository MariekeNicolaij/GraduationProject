using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RandomAchievements : MonoBehaviour
{
    public List<GameObject> achievements = new List<GameObject>();
    public string subject = "Achievements? Challenges?";


    void Start()
    {
        if (achievements.Count == 0)
            return;

        SetRandomAchievements();
    }

    void SetRandomAchievements()
    {
        // Each achievement object has 2 Text child objects so that is what the [0] and [1] means (first and second)
        for(int i = 0; i < achievements.Count; i++)
        {
            bool locked = System.Convert.ToBoolean(Random.Range(0, 2)); // ( 0 - 1 )

            GameObject go = achievements[i];
            go.GetComponentsInChildren<Text>()[0].text = subject + " " + (i + 1);
            go.GetComponentsInChildren<Text>()[1].text = (locked) ? "Locked" : "Unlocked";
            go.GetComponentsInChildren<Text>()[0].color = (locked) ? Color.red : Color.green;
            go.GetComponentsInChildren<Text>()[1].color = (locked) ? Color.red : Color.green;
        }
    }
}
