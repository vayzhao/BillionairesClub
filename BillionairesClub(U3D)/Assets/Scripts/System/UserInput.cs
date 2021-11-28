using System;
using UnityEngine;

public static class UserInput 
{
    public static PlatformType platform;

    public static bool GetKey(KeyCode whichKey)
    {
        switch (platform)
        {
            case PlatformType.Window:
                return Input.GetKey(whichKey);
            case PlatformType.iOS:
                break;
            case PlatformType.Android:
                break;
            default:
                break;
        }        
        return false;
    }

    public static bool GetKeyDown(KeyCode whichKey)
    {
        switch (platform)
        {
            case PlatformType.Window:
                return Input.GetKeyDown(whichKey);
            case PlatformType.iOS:
                break;
            case PlatformType.Android:
                break;
            default:
                break;
        }
        return false;
    }

    public static bool GetKeyUp(KeyCode whichKey)
    {
        switch (platform)
        {
            case PlatformType.Window:
                return Input.GetKeyUp(whichKey);
            case PlatformType.iOS:
                break;
            case PlatformType.Android:
                break;
            default:
                break;
        }
        return false;
    }

    public static float GetAxis(string axisName)
    {
        switch (platform)
        {
            case PlatformType.Window:
                return Input.GetAxis(axisName);
            case PlatformType.iOS:
                break;
            case PlatformType.Android:
                break;
            default:
                break;
        }
        return 0f;        
    }

    public static float GetAxisRaw(string axisName)
    {
        switch (platform)
        {
            case PlatformType.Window:
                return Input.GetAxisRaw(axisName);
            case PlatformType.iOS:
                break;
            case PlatformType.Android:
                break;
            default:
                break;
        }
        return 0f;
    }
}