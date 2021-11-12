using System;
using UnityEngine;

public static class Storage
{
    private static string GetKey(int playerIndex, StorageType storageType)
    {
        return $"{playerIndex}{storageType.ToString()}";
    }

    public static void SaveBool(int playerIndex, StorageType storageType, bool flag)
    {
        PlayerPrefs.SetInt(GetKey(playerIndex, storageType), flag ? 1 : 0);
    }
    public static bool LoadBool(int playerIndex, StorageType storageType)
    {
        return PlayerPrefs.GetInt(GetKey(playerIndex, storageType)) == 1;
    }

    public static void SaveVector3(int playerIndex, StorageType storageType, Vector3 value)
    {
        PlayerPrefs.SetFloat($"{GetKey(playerIndex, storageType)}.x", value.x);
        PlayerPrefs.SetFloat($"{GetKey(playerIndex, storageType)}.y", value.y);
        PlayerPrefs.SetFloat($"{GetKey(playerIndex, storageType)}.z", value.z);
    }
    public static Vector3 LoadVector3(int playerIndex, StorageType storageType)
    {
        return new Vector3(
            PlayerPrefs.GetFloat($"{GetKey(playerIndex, storageType)}.x"),
            PlayerPrefs.GetFloat($"{GetKey(playerIndex, storageType)}.y"),
            PlayerPrefs.GetFloat($"{GetKey(playerIndex, storageType)}.z"));
    }

    public static void SaveInt(int playerIndex, StorageType storageType, int value)
    {
        PlayerPrefs.SetInt(GetKey(playerIndex, storageType), value);
    }
    public static int LoadInt(int playerIndex, StorageType storageType)
    {
        return PlayerPrefs.GetInt(GetKey(playerIndex, storageType));
    }

    public static void SaveFloat(int playerIndex, StorageType storageType, float value)
    {
        PlayerPrefs.SetFloat(GetKey(playerIndex, storageType), value);
    }
    public static float LoadFloat(int playerIndex, StorageType storageType)
    {
        return PlayerPrefs.GetFloat(GetKey(playerIndex, storageType));
    }

    public static void SaveString(int playerIndex, StorageType storageType, string value)
    {
        PlayerPrefs.SetString(GetKey(playerIndex, storageType), value);
    }
    public static string LoadString(int playerIndex, StorageType storageType)
    {
        return PlayerPrefs.GetString(GetKey(playerIndex, storageType));
    }
}