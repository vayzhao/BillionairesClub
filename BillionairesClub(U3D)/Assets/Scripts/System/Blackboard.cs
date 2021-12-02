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

    public static AudioManager audioManager;

    public delegate void VoidDel();

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
        "Models/Character/TH_Bull_01",
        "Models/Character/TH_Bull_02",
        "Models/Character/TH_Bull_03",
        "Models/Character/TH_Cow_01",
        "Models/Character/TH_Cow_02",
        "Models/Character/TH_Cow_03",
        "Models/Character/TH_Donkey_01",
        "Models/Character/TH_Donkey_02",
        "Models/Character/TH_Donkey_03",
        "Models/Character/TH_Frog_01",
        "Models/Character/TH_Frog_02",
        "Models/Character/TH_Frog_03",
        "Models/Character/TH_Giraffe_01",
        "Models/Character/TH_Giraffe_02",
        "Models/Character/TH_Giraffe_03",
        "Models/Character/TH_Kangaroo_01",
        "Models/Character/TH_Kangaroo_02",
        "Models/Character/TH_Kangaroo_03",
        "Models/Character/TH_Lion_01",
        "Models/Character/TH_Lion_02",
        "Models/Character/TH_Lion_03",
        "Models/Character/TH_Lizard_01",
        "Models/Character/TH_Lizard_02",
        "Models/Character/TH_Lizard_03",
        "Models/Character/TH_Pig_01",
        "Models/Character/TH_Pig_02",
        "Models/Character/TH_Pig_03",
        "Models/Character/TH_Rhino_01",
        "Models/Character/TH_Rhino_02",
        "Models/Character/TH_Rhino_03",
        "Models/Character/TH_Warthog_01",
        "Models/Character/TH_Warthog_02",
        "Models/Character/TH_Warthog_03",
        "Models/Character/TH_Zebra_01",
        "Models/Character/TH_Zebra_02",
        "Models/Character/TH_Zebra_03"
    };
    public static string[] portraitPath = new string[36]
    {
        "sprites/portrait/portrait_bull0",
        "sprites/portrait/portrait_bull1",
        "sprites/portrait/portrait_bull2",
        "sprites/portrait/portrait_cow0",
        "sprites/portrait/portrait_cow1",
        "sprites/portrait/portrait_cow2",
        "sprites/portrait/portrait_donkey0",
        "sprites/portrait/portrait_donkey1",
        "sprites/portrait/portrait_donkey2",
        "sprites/portrait/portrait_frog0",
        "sprites/portrait/portrait_frog1",
        "sprites/portrait/portrait_frog2",
        "sprites/portrait/portrait_giraffe0",
        "sprites/portrait/portrait_giraffe1",
        "sprites/portrait/portrait_giraffe2",
        "sprites/portrait/portrait_kangaroo0",
        "sprites/portrait/portrait_kangaroo1",
        "sprites/portrait/portrait_kangaroo2",
        "sprites/portrait/portrait_lion0",
        "sprites/portrait/portrait_lion1",
        "sprites/portrait/portrait_lion2",
        "sprites/portrait/portrait_lizard0",
        "sprites/portrait/portrait_lizard1",
        "sprites/portrait/portrait_lizard2",
        "sprites/portrait/portrait_pig0",
        "sprites/portrait/portrait_pig1",
        "sprites/portrait/portrait_pig2",
        "sprites/portrait/portrait_rhino0",
        "sprites/portrait/portrait_rhino1",
        "sprites/portrait/portrait_rhino2",
        "sprites/portrait/portrait_warthog0",
        "sprites/portrait/portrait_warthog1",
        "sprites/portrait/portrait_warthog2",
        "sprites/portrait/portrait_zebra0",
        "sprites/portrait/portrait_zebra1",
        "sprites/portrait/portrait_zebra2"
    };
    public static GameObject GetModelPrefab(int index) { return Resources.Load<GameObject>(modelPath[index]); }
    public static Sprite GetPortraitPrefab(int index) { return Resources.Load<Sprite>(portraitPath[index]); }
    #endregion

    #region Card
    public static string[] cardSprite = new string[52]
    {
        "sprites/card/diamond_2",
        "sprites/card/diamond_3",
        "sprites/card/diamond_4",
        "sprites/card/diamond_5",
        "sprites/card/diamond_6",
        "sprites/card/diamond_7",
        "sprites/card/diamond_8",
        "sprites/card/diamond_9",
        "sprites/card/diamond_10",
        "sprites/card/diamond_jack",
        "sprites/card/diamond_queen",
        "sprites/card/diamond_king",
        "sprites/card/diamond_ace",
        "sprites/card/club_2",
        "sprites/card/club_3",
        "sprites/card/club_4",
        "sprites/card/club_5",
        "sprites/card/club_6",
        "sprites/card/club_7",
        "sprites/card/club_8",
        "sprites/card/club_9",
        "sprites/card/club_10",
        "sprites/card/club_jack",
        "sprites/card/club_queen",
        "sprites/card/club_king",
        "sprites/card/club_ace",
        "sprites/card/heart_2",
        "sprites/card/heart_3",
        "sprites/card/heart_4",
        "sprites/card/heart_5",
        "sprites/card/heart_6",
        "sprites/card/heart_7",
        "sprites/card/heart_8",
        "sprites/card/heart_9",
        "sprites/card/heart_10",
        "sprites/card/heart_jack",
        "sprites/card/heart_queen",
        "sprites/card/heart_king",
        "sprites/card/heart_ace",
        "sprites/card/spade_2",
        "sprites/card/spade_3",
        "sprites/card/spade_4",
        "sprites/card/spade_5",
        "sprites/card/spade_6",
        "sprites/card/spade_7",
        "sprites/card/spade_8",
        "sprites/card/spade_9",
        "sprites/card/spade_10",
        "sprites/card/spade_jack",
        "sprites/card/spade_queen",
        "sprites/card/spade_king",
        "sprites/card/spade_ace"
    };
    public static string[] cardMesh = new string[52]
    {
        "models/cardmesh/Card_Diamond_B_02",
        "models/cardmesh/Card_Diamond_B_03",
        "models/cardmesh/Card_Diamond_B_04",
        "models/cardmesh/Card_Diamond_B_05",
        "models/cardmesh/Card_Diamond_B_06",
        "models/cardmesh/Card_Diamond_B_07",
        "models/cardmesh/Card_Diamond_B_08",
        "models/cardmesh/Card_Diamond_B_09",
        "models/cardmesh/Card_Diamond_B_10",
        "models/cardmesh/Card_Diamond_B_jack",
        "models/cardmesh/Card_Diamond_B_queen",
        "models/cardmesh/Card_Diamond_B_king",
        "models/cardmesh/Card_Diamond_B_ace",
        "models/cardmesh/Card_Clover_B_02",
        "models/cardmesh/Card_Clover_B_03",
        "models/cardmesh/Card_Clover_B_04",
        "models/cardmesh/Card_Clover_B_05",
        "models/cardmesh/Card_Clover_B_06",
        "models/cardmesh/Card_Clover_B_07",
        "models/cardmesh/Card_Clover_B_08",
        "models/cardmesh/Card_Clover_B_09",
        "models/cardmesh/Card_Clover_B_10",
        "models/cardmesh/Card_Clover_B_jack",
        "models/cardmesh/Card_Clover_B_queen",
        "models/cardmesh/Card_Clover_B_king",
        "models/cardmesh/Card_Clover_B_ace",
        "models/cardmesh/Card_Heart_B_02",
        "models/cardmesh/Card_Heart_B_03",
        "models/cardmesh/Card_Heart_B_04",
        "models/cardmesh/Card_Heart_B_05",
        "models/cardmesh/Card_Heart_B_06",
        "models/cardmesh/Card_Heart_B_07",
        "models/cardmesh/Card_Heart_B_08",
        "models/cardmesh/Card_Heart_B_09",
        "models/cardmesh/Card_Heart_B_10",
        "models/cardmesh/Card_Heart_B_jack",
        "models/cardmesh/Card_Heart_B_queen",
        "models/cardmesh/Card_Heart_B_king",
        "models/cardmesh/Card_Heart_B_ace",
        "models/cardmesh/Card_Spades_B_02",
        "models/cardmesh/Card_Spades_B_03",
        "models/cardmesh/Card_Spades_B_04",
        "models/cardmesh/Card_Spades_B_05",
        "models/cardmesh/Card_Spades_B_06",
        "models/cardmesh/Card_Spades_B_07",
        "models/cardmesh/Card_Spades_B_08",
        "models/cardmesh/Card_Spades_B_09",
        "models/cardmesh/Card_Spades_B_10",
        "models/cardmesh/Card_Spades_B_jack",
        "models/cardmesh/Card_Spades_B_queen",
        "models/cardmesh/Card_Spades_B_king",
        "models/cardmesh/Card_Spades_B_ace"
    };
    public static Sprite GetCardSprite(int index) { return Resources.Load<Sprite>(cardSprite[index]); }
    public static Mesh GetCardMesh(int index) { return Resources.Load<Mesh>(cardMesh[index]); }
    #endregion
}
