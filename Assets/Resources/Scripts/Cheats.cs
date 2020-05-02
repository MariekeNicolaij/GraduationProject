using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cheats : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
            GameManager.instance.GameOver();
        if (Input.GetKeyDown(KeyCode.D))
            GameManager.instance.UpdateDate();
        if (Input.GetKeyDown(KeyCode.E))
            PlayerPrefs.DeleteAll();
    }
}
