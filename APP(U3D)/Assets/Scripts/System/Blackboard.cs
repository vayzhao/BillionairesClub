using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// BB (Blackboard)
/// </summary>
public static class Blackboard 
{   
    public static string SCENE_PREVIOUS = Const.SCENE_HOMEPAGE;
    public static float loadEstimate = 1f;
    public static bool isDebugMode = false;
    public static bool debugChecked = false;

    #region Player
    public static Player localPlayer;
    #endregion

    #region Spawn Holder
    public static Transform spawnHolder;
    public static void SetupSpawnHolder() 
    { 
        spawnHolder = new GameObject().transform;
        spawnHolder.name = "Spawn Holder";
    }
    #endregion

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
    public static string[] portraitPath = new string[36]
    {
        "Assets/Sprites/portrait/portrait_bull0.jpg",
        "Assets/Sprites/portrait/portrait_bull1.jpg",
        "Assets/Sprites/portrait/portrait_bull2.jpg",
        "Assets/Sprites/portrait/portrait_cow0.jpg",
        "Assets/Sprites/portrait/portrait_cow1.jpg",
        "Assets/Sprites/portrait/portrait_cow2.jpg",
        "Assets/Sprites/portrait/portrait_donkey0.jpg",
        "Assets/Sprites/portrait/portrait_donkey1.jpg",
        "Assets/Sprites/portrait/portrait_donkey2.jpg",
        "Assets/Sprites/portrait/portrait_frog0.jpg",
        "Assets/Sprites/portrait/portrait_frog1.jpg",
        "Assets/Sprites/portrait/portrait_frog2.jpg",
        "Assets/Sprites/portrait/portrait_giraffe0.jpg",
        "Assets/Sprites/portrait/portrait_giraffe1.jpg",
        "Assets/Sprites/portrait/portrait_giraffe2.jpg",
        "Assets/Sprites/portrait/portrait_kangaroo0.jpg",
        "Assets/Sprites/portrait/portrait_kangaroo1.jpg",
        "Assets/Sprites/portrait/portrait_kangaroo2.jpg",
        "Assets/Sprites/portrait/portrait_lion0.jpg",
        "Assets/Sprites/portrait/portrait_lion1.jpg",
        "Assets/Sprites/portrait/portrait_lion2.jpg",
        "Assets/Sprites/portrait/portrait_lizard0.jpg",
        "Assets/Sprites/portrait/portrait_lizard1.jpg",
        "Assets/Sprites/portrait/portrait_lizard2.jpg",
        "Assets/Sprites/portrait/portrait_pig0.jpg",
        "Assets/Sprites/portrait/portrait_pig1.jpg",
        "Assets/Sprites/portrait/portrait_pig2.jpg",
        "Assets/Sprites/portrait/portrait_rhino0.jpg",
        "Assets/Sprites/portrait/portrait_rhino1.jpg",
        "Assets/Sprites/portrait/portrait_rhino2.jpg",
        "Assets/Sprites/portrait/portrait_warthog0.jpg",
        "Assets/Sprites/portrait/portrait_warthog1.jpg",
        "Assets/Sprites/portrait/portrait_warthog2.jpg",
        "Assets/Sprites/portrait/portrait_zebra0.jpg",
        "Assets/Sprites/portrait/portrait_zebra1.jpg",
        "Assets/Sprites/portrait/portrait_zebra2.jpg"
    };
    public static GameObject GetModelPrefab(int index) { return (GameObject)AssetDatabase.LoadAssetAtPath(modelPath[index], typeof(GameObject)); }
    public static Sprite GetPortraitPrefab(int index) { return (Sprite)AssetDatabase.LoadAssetAtPath(portraitPath[index], typeof(Sprite)); }
    #endregion

    #region Card
    public static string[] cardSprite = new string[52]
    {
        "Assets/Sprites/Card/diamond_2.png",
        "Assets/Sprites/Card/diamond_3.png",
        "Assets/Sprites/Card/diamond_4.png",
        "Assets/Sprites/Card/diamond_5.png",
        "Assets/Sprites/Card/diamond_6.png",
        "Assets/Sprites/Card/diamond_7.png",
        "Assets/Sprites/Card/diamond_8.png",
        "Assets/Sprites/Card/diamond_9.png",
        "Assets/Sprites/Card/diamond_10.png",
        "Assets/Sprites/Card/diamond_jack.png",
        "Assets/Sprites/Card/diamond_queen.png",
        "Assets/Sprites/Card/diamond_king.png",
        "Assets/Sprites/Card/diamond_ace.png",
        "Assets/Sprites/Card/club_2.png",
        "Assets/Sprites/Card/club_3.png",
        "Assets/Sprites/Card/club_4.png",
        "Assets/Sprites/Card/club_5.png",
        "Assets/Sprites/Card/club_6.png",
        "Assets/Sprites/Card/club_7.png",
        "Assets/Sprites/Card/club_8.png",
        "Assets/Sprites/Card/club_9.png",
        "Assets/Sprites/Card/club_10.png",
        "Assets/Sprites/Card/club_jack.png",
        "Assets/Sprites/Card/club_queen.png",
        "Assets/Sprites/Card/club_king.png",
        "Assets/Sprites/Card/club_ace.png",
        "Assets/Sprites/Card/heart_2.png",
        "Assets/Sprites/Card/heart_3.png",
        "Assets/Sprites/Card/heart_4.png",
        "Assets/Sprites/Card/heart_5.png",
        "Assets/Sprites/Card/heart_6.png",
        "Assets/Sprites/Card/heart_7.png",
        "Assets/Sprites/Card/heart_8.png",
        "Assets/Sprites/Card/heart_9.png",
        "Assets/Sprites/Card/heart_10.png",
        "Assets/Sprites/Card/heart_jack.png",
        "Assets/Sprites/Card/heart_queen.png",
        "Assets/Sprites/Card/heart_king.png",
        "Assets/Sprites/Card/heart_ace.png",
        "Assets/Sprites/Card/spade_2.png",
        "Assets/Sprites/Card/spade_3.png",
        "Assets/Sprites/Card/spade_4.png",
        "Assets/Sprites/Card/spade_5.png",
        "Assets/Sprites/Card/spade_6.png",
        "Assets/Sprites/Card/spade_7.png",
        "Assets/Sprites/Card/spade_8.png",
        "Assets/Sprites/Card/spade_9.png",
        "Assets/Sprites/Card/spade_10.png",
        "Assets/Sprites/Card/spade_jack.png",
        "Assets/Sprites/Card/spade_queen.png",
        "Assets/Sprites/Card/spade_king.png",
        "Assets/Sprites/Card/spade_ace.png"
    };
    public static string[] cardMesh = new string[52]
    {
        "Assets/External Resource/Casino_Environment_Pack/FBX/Card_Diamond_B_02.fbx",
        "Assets/External Resource/Casino_Environment_Pack/FBX/Card_Diamond_B_03.fbx",
        "Assets/External Resource/Casino_Environment_Pack/FBX/Card_Diamond_B_04.fbx",
        "Assets/External Resource/Casino_Environment_Pack/FBX/Card_Diamond_B_05.fbx",
        "Assets/External Resource/Casino_Environment_Pack/FBX/Card_Diamond_B_06.fbx",
        "Assets/External Resource/Casino_Environment_Pack/FBX/Card_Diamond_B_07.fbx",
        "Assets/External Resource/Casino_Environment_Pack/FBX/Card_Diamond_B_08.fbx",
        "Assets/External Resource/Casino_Environment_Pack/FBX/Card_Diamond_B_09.fbx",
        "Assets/External Resource/Casino_Environment_Pack/FBX/Card_Diamond_B_10.fbx",
        "Assets/External Resource/Casino_Environment_Pack/FBX/Card_Diamond_B_jack.fbx",
        "Assets/External Resource/Casino_Environment_Pack/FBX/Card_Diamond_B_queen.fbx",
        "Assets/External Resource/Casino_Environment_Pack/FBX/Card_Diamond_B_king.fbx",
        "Assets/External Resource/Casino_Environment_Pack/FBX/Card_Diamond_B_ace.fbx",
        "Assets/External Resource/Casino_Environment_Pack/FBX/Card_Clover_B_02.fbx",
        "Assets/External Resource/Casino_Environment_Pack/FBX/Card_Clover_B_03.fbx",
        "Assets/External Resource/Casino_Environment_Pack/FBX/Card_Clover_B_04.fbx",
        "Assets/External Resource/Casino_Environment_Pack/FBX/Card_Clover_B_05.fbx",
        "Assets/External Resource/Casino_Environment_Pack/FBX/Card_Clover_B_06.fbx",
        "Assets/External Resource/Casino_Environment_Pack/FBX/Card_Clover_B_07.fbx",
        "Assets/External Resource/Casino_Environment_Pack/FBX/Card_Clover_B_08.fbx",
        "Assets/External Resource/Casino_Environment_Pack/FBX/Card_Clover_B_09.fbx",
        "Assets/External Resource/Casino_Environment_Pack/FBX/Card_Clover_B_10.fbx",
        "Assets/External Resource/Casino_Environment_Pack/FBX/Card_Clover_B_jack.fbx",
        "Assets/External Resource/Casino_Environment_Pack/FBX/Card_Clover_B_queen.fbx",
        "Assets/External Resource/Casino_Environment_Pack/FBX/Card_Clover_B_king.fbx",
        "Assets/External Resource/Casino_Environment_Pack/FBX/Card_Clover_B_ace.fbx",
        "Assets/External Resource/Casino_Environment_Pack/FBX/Card_Heart_B_02.fbx",
        "Assets/External Resource/Casino_Environment_Pack/FBX/Card_Heart_B_03.fbx",
        "Assets/External Resource/Casino_Environment_Pack/FBX/Card_Heart_B_04.fbx",
        "Assets/External Resource/Casino_Environment_Pack/FBX/Card_Heart_B_05.fbx",
        "Assets/External Resource/Casino_Environment_Pack/FBX/Card_Heart_B_06.fbx",
        "Assets/External Resource/Casino_Environment_Pack/FBX/Card_Heart_B_07.fbx",
        "Assets/External Resource/Casino_Environment_Pack/FBX/Card_Heart_B_08.fbx",
        "Assets/External Resource/Casino_Environment_Pack/FBX/Card_Heart_B_09.fbx",
        "Assets/External Resource/Casino_Environment_Pack/FBX/Card_Heart_B_10.fbx",
        "Assets/External Resource/Casino_Environment_Pack/FBX/Card_Heart_B_jack.fbx",
        "Assets/External Resource/Casino_Environment_Pack/FBX/Card_Heart_B_queen.fbx",
        "Assets/External Resource/Casino_Environment_Pack/FBX/Card_Heart_B_king.fbx",
        "Assets/External Resource/Casino_Environment_Pack/FBX/Card_Heart_B_ace.fbx",
        "Assets/External Resource/Casino_Environment_Pack/FBX/Card_Spades_B_02.fbx",
        "Assets/External Resource/Casino_Environment_Pack/FBX/Card_Spades_B_03.fbx",
        "Assets/External Resource/Casino_Environment_Pack/FBX/Card_Spades_B_04.fbx",
        "Assets/External Resource/Casino_Environment_Pack/FBX/Card_Spades_B_05.fbx",
        "Assets/External Resource/Casino_Environment_Pack/FBX/Card_Spades_B_06.fbx",
        "Assets/External Resource/Casino_Environment_Pack/FBX/Card_Spades_B_07.fbx",
        "Assets/External Resource/Casino_Environment_Pack/FBX/Card_Spades_B_08.fbx",
        "Assets/External Resource/Casino_Environment_Pack/FBX/Card_Spades_B_09.fbx",
        "Assets/External Resource/Casino_Environment_Pack/FBX/Card_Spades_B_10.fbx",
        "Assets/External Resource/Casino_Environment_Pack/FBX/Card_Spades_B_jack.fbx",
        "Assets/External Resource/Casino_Environment_Pack/FBX/Card_Spades_B_queen.fbx",
        "Assets/External Resource/Casino_Environment_Pack/FBX/Card_Spades_B_king.fbx",
        "Assets/External Resource/Casino_Environment_Pack/FBX/Card_Spades_B_ace.fbx"
    };
    public static Sprite GetCardSprite(int index) { return (Sprite)AssetDatabase.LoadAssetAtPath(cardSprite[index], typeof(Sprite)); }
    public static Mesh GetCardMesh(int index) { return (Mesh)AssetDatabase.LoadAssetAtPath(cardMesh[index], typeof(Mesh)); }
    #endregion

}
