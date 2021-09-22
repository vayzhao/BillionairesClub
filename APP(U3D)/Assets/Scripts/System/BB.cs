using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// BB (Blackboard)
/// </summary>
public static class BB 
{
    public const string SCENE_HOMEPAGE = "Homepage";
    public const string SCENE_INGAME = "InGame";
    public const string SCENE_TEXAS = "InTexas";

    public const float DISTANCE_SIT = 2f;
    public const float RATE_SIT_DETECT = 0.2f;

    public static float loadEstimate = 1f;

    #region Character Prefabs
    public static string[] modelName = new string[12]
    {
        "Bull",
        "Cow",
        "Donkey",
        "Frog",
        "Giraffe",
        "Kangaroo",
        "Lion",
        "Lizard",
        "Pig",
        "Rhino",
        "Warthog",
        "Zebra"
    };
    public static string[] modelPath = new string[36] {
        "Assets/External Resource/Toon Humanoid Animals Ultimate Vol 1 PBR/Generic/Prefabs/TH_Bull_01.prefab",
        "Assets/External Resource/Toon Humanoid Animals Ultimate Vol 1 PBR/Generic/Prefabs/TH_Bull_02.prefab",
        "Assets/External Resource/Toon Humanoid Animals Ultimate Vol 1 PBR/Generic/Prefabs/TH_Bull_03.prefab",
        "Assets/External Resource/Toon Humanoid Animals Ultimate Vol 1 PBR/Generic/Prefabs/TH_Cow_01.prefab",
        "Assets/External Resource/Toon Humanoid Animals Ultimate Vol 1 PBR/Generic/Prefabs/TH_Cow_02.prefab",
        "Assets/External Resource/Toon Humanoid Animals Ultimate Vol 1 PBR/Generic/Prefabs/TH_Cow_03.prefab",
        "Assets/External Resource/Toon Humanoid Animals Ultimate Vol 1 PBR/Generic/Prefabs/TH_Donkey_01.prefab",
        "Assets/External Resource/Toon Humanoid Animals Ultimate Vol 1 PBR/Generic/Prefabs/TH_Donkey_02.prefab",
        "Assets/External Resource/Toon Humanoid Animals Ultimate Vol 1 PBR/Generic/Prefabs/TH_Donkey_03.prefab",
        "Assets/External Resource/Toon Humanoid Animals Ultimate Vol 1 PBR/Generic/Prefabs/TH_Frog_01.prefab",
        "Assets/External Resource/Toon Humanoid Animals Ultimate Vol 1 PBR/Generic/Prefabs/TH_Frog_02.prefab",
        "Assets/External Resource/Toon Humanoid Animals Ultimate Vol 1 PBR/Generic/Prefabs/TH_Frog_03.prefab",
        "Assets/External Resource/Toon Humanoid Animals Ultimate Vol 1 PBR/Generic/Prefabs/TH_Giraffe_01.prefab",
        "Assets/External Resource/Toon Humanoid Animals Ultimate Vol 1 PBR/Generic/Prefabs/TH_Giraffe_02.prefab",
        "Assets/External Resource/Toon Humanoid Animals Ultimate Vol 1 PBR/Generic/Prefabs/TH_Giraffe_03.prefab",
        "Assets/External Resource/Toon Humanoid Animals Ultimate Vol 1 PBR/Generic/Prefabs/TH_Kangaroo_01.prefab",
        "Assets/External Resource/Toon Humanoid Animals Ultimate Vol 1 PBR/Generic/Prefabs/TH_Kangaroo_02.prefab",
        "Assets/External Resource/Toon Humanoid Animals Ultimate Vol 1 PBR/Generic/Prefabs/TH_Kangaroo_03.prefab",
        "Assets/External Resource/Toon Humanoid Animals Ultimate Vol 1 PBR/Generic/Prefabs/TH_Lion_01.prefab",
        "Assets/External Resource/Toon Humanoid Animals Ultimate Vol 1 PBR/Generic/Prefabs/TH_Lion_02.prefab",
        "Assets/External Resource/Toon Humanoid Animals Ultimate Vol 1 PBR/Generic/Prefabs/TH_Lion_03.prefab",
        "Assets/External Resource/Toon Humanoid Animals Ultimate Vol 1 PBR/Generic/Prefabs/TH_Lizard_01.prefab",
        "Assets/External Resource/Toon Humanoid Animals Ultimate Vol 1 PBR/Generic/Prefabs/TH_Lizard_02.prefab",
        "Assets/External Resource/Toon Humanoid Animals Ultimate Vol 1 PBR/Generic/Prefabs/TH_Lizard_03.prefab",
        "Assets/External Resource/Toon Humanoid Animals Ultimate Vol 1 PBR/Generic/Prefabs/TH_Pig_01.prefab",
        "Assets/External Resource/Toon Humanoid Animals Ultimate Vol 1 PBR/Generic/Prefabs/TH_Pig_02.prefab",
        "Assets/External Resource/Toon Humanoid Animals Ultimate Vol 1 PBR/Generic/Prefabs/TH_Pig_03.prefab",
        "Assets/External Resource/Toon Humanoid Animals Ultimate Vol 1 PBR/Generic/Prefabs/TH_Rhino_01.prefab",
        "Assets/External Resource/Toon Humanoid Animals Ultimate Vol 1 PBR/Generic/Prefabs/TH_Rhino_02.prefab",
        "Assets/External Resource/Toon Humanoid Animals Ultimate Vol 1 PBR/Generic/Prefabs/TH_Rhino_03.prefab",
        "Assets/External Resource/Toon Humanoid Animals Ultimate Vol 1 PBR/Generic/Prefabs/TH_Warthog_01.prefab",
        "Assets/External Resource/Toon Humanoid Animals Ultimate Vol 1 PBR/Generic/Prefabs/TH_Warthog_02.prefab",
        "Assets/External Resource/Toon Humanoid Animals Ultimate Vol 1 PBR/Generic/Prefabs/TH_Warthog_03.prefab",
        "Assets/External Resource/Toon Humanoid Animals Ultimate Vol 1 PBR/Generic/Prefabs/TH_Zebra_01.prefab",
        "Assets/External Resource/Toon Humanoid Animals Ultimate Vol 1 PBR/Generic/Prefabs/TH_Zebra_02.prefab",
        "Assets/External Resource/Toon Humanoid Animals Ultimate Vol 1 PBR/Generic/Prefabs/TH_Zebra_03.prefab"
    };
    public static GameObject GetModelPrefab(int index) { return (GameObject)AssetDatabase.LoadAssetAtPath(modelPath[index], typeof(GameObject)); }
    #endregion


}
