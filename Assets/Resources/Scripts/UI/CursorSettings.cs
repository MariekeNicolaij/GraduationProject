using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorSettings : MonoBehaviour
{
    public Texture2D cursorNormal, cursorClick;


    void Start()
    {
        SetCursorTexture(cursorNormal);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            SetCursorTexture(cursorClick);
        if (Input.GetMouseButtonUp(0))
            SetCursorTexture(cursorNormal);
    }

    void SetCursorTexture(Texture2D texture)
    {
        Cursor.SetCursor(texture, Vector2.zero, CursorMode.Auto);
    }
}
