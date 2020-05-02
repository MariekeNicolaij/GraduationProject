using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FindAndApplyActiveCamera : MonoBehaviour
{
    void Update()
    {
        if (!GetComponent<Canvas>().worldCamera)
            GetComponent<Canvas>().worldCamera = Camera.main;
        else
            Destroy(GetComponent<FindAndApplyActiveCamera>());
    }
}
