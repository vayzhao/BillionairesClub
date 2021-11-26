using System;
using UnityEngine;

public class CursorController : MonoBehaviour
{
    public static bool cursorDisplay;
    public static bool windowDisplay;
    public static bool isTypingText;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftAlt))
            ShowCursor(true);

        if (Input.GetKeyUp(KeyCode.LeftAlt))
            ShowCursor(false);
    }

    void ShowCursor(bool flag)
    {
        cursorDisplay = flag;

        if (!windowDisplay)
            LockCursor(!flag);
    }

    public static void LockCursor(bool flag)
    {
        Cursor.visible = !flag;
        Cursor.lockState = flag ? CursorLockMode.Locked : CursorLockMode.None;
    }

    public static void WindowFocusMode(bool flag)
    {
        LockCursor(!flag);
        cursorDisplay = flag;
        windowDisplay = flag;
    }

    public static bool IsCharacterLocked() => cursorDisplay || windowDisplay || isTypingText;
}