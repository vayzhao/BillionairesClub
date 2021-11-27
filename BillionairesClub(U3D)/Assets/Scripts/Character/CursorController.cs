using System;
using UnityEngine;

public class CursorController : MonoBehaviour
{
    public static bool cursorDisplay; // switch on when the user is pressing ALT key
    public static bool windowDisplay; // switch on when there is a window overlay the screen
    public static bool isTypingText;  // switch on when the user is typing 

    private UIManager uiManager; // a component that handles functionalities of UI components

    void Start()
    {
        // find the ui manager
        uiManager = FindObjectOfType<UIManager>();
    }

    void Update()
    {
        DetectAltKey();
        DetectWindowFocus();
    }

    /// <summary>
    /// Method to detect and switch cursor display state when the user 
    /// is pressing or releasing the alt key
    /// </summary>
    void DetectAltKey()
    {
        // switch cursor display state to true when alt key is pressed
        // switch to false when alt key is released
        // otherwise remain its current value
        cursorDisplay = Input.GetKeyDown(KeyCode.LeftAlt) ? true : Input.GetKeyUp(KeyCode.LeftAlt) ? false : cursorDisplay;

        // return when there is a window overlay the screen or the cursor visible state matches cursor display state
        if (windowDisplay || Cursor.visible == cursorDisplay)
            return;

        // lock or free the cursor
        LockCursor(!cursorDisplay);
    }

    /// <summary>
    /// Method to detect and switch window display state when a new window 
    /// pop up to overlay the screen or an exisiting window that overlay the 
    /// screen disappears
    /// </summary>
    void DetectWindowFocus()
    {
        // check to see if there is a window overlay the screen
        var currentState = uiManager.HasPage();

        // return if the current state matches windowDisplay state
        if (currentState == windowDisplay)
            return;

        // otherwise set window display state to be current state        
        windowDisplay = currentState;

        // lock or free the cursor
        LockCursor(!currentState);
    }

    /// <summary>
    /// Method to lock or free the cursor 
    /// </summary>
    /// <param name="flag">true for locking, false for releasing</param>
    public static void LockCursor(bool flag)
    {
        Cursor.visible = !flag;
        Cursor.lockState = flag ? CursorLockMode.Locked : CursorLockMode.None;
    }

    /// <summary>
    /// Determine whether or not the character is able to move, the character
    /// can not move when cursor is displayed, window overlay the screen or 
    /// the user is typing
    /// </summary>
    /// <returns></returns>
    public static bool IsCharacterLocked() => cursorDisplay || windowDisplay || isTypingText;
}