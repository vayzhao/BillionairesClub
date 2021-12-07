using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using static Blackboard;
using static Const;

/// <summary>
/// A class to handle the process of loading & spawning when switching from
/// a scene to another
/// </summary>
public class Loading : MonoBehaviour
{
    [Header("Scene Settings")]
    [Tooltip("Type of this scene")]
    public SceneType sceneType;
    [Tooltip("Type of this game (only select this when sceneType is InGame)")]
    public GameType gameType;

    [Header("UI Elements")]
    [Tooltip("The canvas transform, which holds all the UI objects")]
    public Transform canvas;
    [Tooltip("A game object that contains all UI prefabs for a specific scene")]
    public GameObject pref_uiManager;

    [Header("Prefab Objects")]
    [Tooltip("The prefab object for the environment in this scene")]
    public GameObject pref_environment;    
    [Tooltip("The prefab object for player's character")]
    public GameObject pref_character;    
    [Tooltip("The prefab object for the core manager in this scene")]
    public GameObject pref_manager;

    private Loader loader;                // a class that contains all sort of methods to spawn objects 
    private GameObject obj_environment;   // a variable to record the environmental objects
    private GameObject obj_character;     // a variable to record the player's character
    private GameObject obj_gameManager;       // a variable to record the core manager for this scene
    private UIManager obj_uiManager;      // a variable to record the UI manager
    private List<GameObject> visibleObjs; // a list to store all the visible object, when intend to load a new scene,
                                          // hide all visble objects from the old scene

    void Start()
    {
        // check to see if this scene is being tested
        DebugMode();

        // initialize visible object list
        visibleObjs = new List<GameObject>();

        // check what type of scene it is loading
        switch (sceneType)
        {
            case SceneType.Homepage:
                break;
            case SceneType.InCasino:
                LoadCasinoScene();
                break;
            case SceneType.InGame:
                switch (gameType)
                {
                    case GameType.None:
                        break;
                    case GameType.TexasBonus:
                        LoadTexasBonus();
                        break;
                    case GameType.Blackjack:
                        LoadBlackjack();
                        break;
                    default:
                        break;
                }
                break;
            default:
                break;
        }
    }

    #region Progress Loader in Debug Mode
    /// <summary>
    /// Method to run progress bar in debug mode
    /// </summary>
    void DebugMode()
    {
        // return if this program has been checked
        if (debugChecked)
            return;

        // otherwise set debugChecked to true
        debugChecked = true;

        // check to see if the user is entering this scene in an appropriate way
        // (e.g. 
        //       Homepage -> Casino
        //       Casino -> InGame
        if ((sceneType == SceneType.InCasino && !SceneManager.GetSceneByName(SCENE_HOMEPAGE).isLoaded)
            || (sceneType == SceneType.InGame && !SceneManager.GetSceneByName(SCENE_INCASINO).isLoaded))
        {
            // switch debug mode on
            isDebugMode = true;
            Debug.Log("Debug Mode is on!");

            // run the progress bar in debug mode
            StartCoroutine(ProgressRunner());

            // create a player for play testing
            localPlayer = new Player();           
            localPlayer.name = "The Tester";
            localPlayer.description = "";
            localPlayer.chip = DEFAULT_CHIP;
            localPlayer.gem = DEFAULT_GEM;
            localPlayer.modelIndex = Storage.LoadInt(LOCAL_PLAYER, StorageType.ModelIndex);
        }
    }
    IEnumerator ProgressRunner()
    {   
        var progress = 0f;
        Storage.SaveFloat(LOCAL_PLAYER, StorageType.Progress, progress);
        while (progress < 1f)
        {
            progress += (Time.deltaTime / loadEstimate);
            Storage.SaveFloat(LOCAL_PLAYER, StorageType.Progress, progress);
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }
    #endregion

    #region Loader Initializer
    /// <summary>
    /// A method to load all contents needed in casino scene
    /// </summary>
    void LoadCasinoScene()
    {
        // setup a loader
        loader = new Loader();
        loader.RegisterLoadingMethod(0.2f, LoadSceneEnvironment);
        loader.RegisterLoadingMethod(0.4f, LoadCharacterController);
        loader.RegisterLoadingMethod(0.6f, LoadCasinoManager);
        loader.RegisterLoadingMethod(0.8f, LoadUIManager);

        // set this scene to be active
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(SCENE_INCASINO));

        // start the loading coroutine
        StartCoroutine(Load());
    }

    /// <summary>
    /// A method to load all contents needed in texas bonus scene
    /// </summary>
    void LoadTexasBonus()
    {
        // setup a loader
        loader = new Loader();
        loader.RegisterLoadingMethod(0.2f, LoadSceneEnvironment);
        loader.RegisterLoadingMethod(0.4f, LoadTableCharacters);
        loader.RegisterLoadingMethod(0.6f, LoadUIManager);
        loader.RegisterLoadingMethod(0.8f, LoadTexasBonusManager);

        // set this scene to be active 
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(SCENE_TEXAS));

        // start the loading coroutine
        StartCoroutine(Load());
    }

    /// <summary>
    /// A method to load all contents needed in blackjack scene
    /// </summary>
    void LoadBlackjack()
    {
        // setup a loader
        loader = new Loader();
        loader.RegisterLoadingMethod(0.2f, LoadSceneEnvironment);
        loader.RegisterLoadingMethod(0.4f, LoadTableCharacters);
        loader.RegisterLoadingMethod(0.6f, LoadUIManager);
        loader.RegisterLoadingMethod(0.8f, LoadBlackjackManager);

        // set this scene to be active
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(SCENE_BLACKJACK));

        // start the loading coroutine
        StartCoroutine(Load());
    }
    #endregion

    #region Loading Bar Progress
    /// <summary>
    /// An IEnumerator to constantly reading the loading progress from player prefs
    /// and load the loadData one by one
    /// </summary>
    /// <returns></returns>
    IEnumerator Load()
    {
        // var progress = PlayerPrefs.GetFloat("LoadingBarProgress");
        var progress = 0f;
        while (progress < 1f)
        {
            // check to see if the current progress percentage is
            // ready to invoke the next load method
            if (loader.IsReady(progress))
                loader.Run();

            // get the updated loading bar progress
            progress = Storage.LoadFloat(LOCAL_PLAYER, StorageType.Progress);
            yield return new WaitForSeconds(Time.deltaTime);
        }

        // set fade delay time, if takes 2 seconds wait time to fade when transiting
        // from homepage scene to casino scene
        var fadeDelay = SceneManager.GetSceneByName(SCENE_HOMEPAGE).isLoaded ? 2f : 0f;

        // start to fade in and out after n seconds
        yield return new WaitForSeconds(fadeDelay);
        StartCoroutine(Fade());
    }

    /// <summary>
    /// An IEnumerator to fade from Homepage scene to InGame scene, 
    /// The game starts and unload the homePage scene when the fade
    /// image is completely turned to white
    /// </summary>
    /// <returns></returns>
    IEnumerator Fade()
    {
        // display the fading sprite in canvas
        var fade = obj_uiManager.fadeSprite;

        // get the color of the fadeImage
        var color = fade.color;

        // initialize fadeTime and its timer
        float fadeTime;
        float fadeTimer;

        // fade into white
        fadeTime = 0.5f;
        fadeTimer = 0f;
        while (fadeTimer < fadeTime)
        {
            color.a = Mathf.Lerp(0f, 1f, (fadeTimer / fadeTime));
            fade.color = color;
            fadeTimer += Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }

        // unload the previous scene if it is loaded
        if (SCENE_PREVIOUS != ""
            && SceneManager.GetSceneByName(SCENE_PREVIOUS).isLoaded)
        {
            SceneManager.UnloadSceneAsync(SCENE_PREVIOUS);
        }

        // officially start the scene
        switch (sceneType)
        {
            case SceneType.Homepage:
                break;
            case SceneType.InCasino:
                GameStart_Casino();
                GameStart_Common(true);
                break;
            case SceneType.InGame:
                GameStart_Common(false);
                switch (gameType)
                {
                    case GameType.None:
                        break;
                    case GameType.TexasBonus:
                        GameStart_Texas();
                        break;
                    case GameType.Blackjack:
                        GameStart_Blackjack();
                        break;
                    default:
                        break;
                }
                break;
            default:
                break;
        }

        // fade out from white
        fadeTime = 1.5f;
        fadeTimer = 0f;
        while (fadeTimer < fadeTime)
        {
            color.a = Mathf.Lerp(1f, 0f, (fadeTimer / fadeTime));
            fade.color = color;
            fadeTimer += Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }

        // destroy the fade image object
        Destroy(fade.gameObject);
    }

    /// <summary>
    /// A method to load from casino scene into a specific game scene
    /// </summary>
    /// <param name="gameType">game type of the loading scene</param>
    public void LoadIntoGame(TableInformation table)
    {
        var sceneName = "";
        switch (table.gameType)
        {
            case GameType.None:
                break;
            case GameType.TexasBonus:
                sceneName = SCENE_TEXAS;                
                break;
            case GameType.Blackjack:
                sceneName = SCENE_BLACKJACK;
                break;
            default:
                break;
        }

        // store player's information in that table
        table.SaveSeatsInfo();

        // hide visble object from the old scene
        HideVisibleObjects();

        // hide portal tags
        FindObjectOfType<PortalTagManager>().Stop();

        // start loading coroutine
        StartCoroutine(SubLoading(sceneName));
    }

    /// <summary>
    /// Method to load from games to casino
    /// </summary>
    public void LoadBackToCasino()
    {
        // before loading back to the casino, we have to remove all the seat components
        // so that the seat manager in casino scene will not capture seats from the game scene
        var seats = FindObjectsOfType<Seat>();
        for (int i = seats.Length - 1; i >= 0 ; i--)
            Destroy(seats[i]);

        // hide visble object from the old scene
        HideVisibleObjects();

        // start loading coroutine
        StartCoroutine(SubLoading(SCENE_INCASINO));
    }

    /// <summary>
    /// Method to hide all the visible object from the current scene. It is called before loading a 
    /// new scene. The purpose of this that is because new object will be spawning into the new scene
    /// when loading it, we don't want the camera from the old scene to be able to see objects in the 
    /// new scene, that's why we move all the visible objects and the camera from the old scene, to a
    /// place that is really far away from the new scene.
    /// </summary>
    void HideVisibleObjects()
    {
        while (visibleObjs.Count > 0)
        {
            // move the object
            var pos = visibleObjs[0].transform.position;
            pos.y -= 99f;
            visibleObjs[0].transform.position = pos;

            // if this object is a player character controller, re-bind its model to the parent
            visibleObjs[0].GetComponent<CController>()?.ReBindModel();

            // remove the object from the list
            visibleObjs.RemoveAt(0);
        }
    }

    /// <summary>
    /// An IEnumerator to update the sub loading bar every frame
    /// </summary>
    /// <returns></returns>
    IEnumerator SubLoading(string sceneName)
    {
        // first, display the sub-loader object
        var subLoader = obj_uiManager.subLoaderBar;
        var slider = subLoader.GetComponentInChildren<Slider>();
        subLoader.SetActive(true);

        // force slider's value to be 0
        slider.value = 0f;

        // reset progress value in player prefs
        var progress = 0f;
        Storage.SaveFloat(LOCAL_PLAYER, StorageType.Progress, progress);

        // disable light component from this scene
        foreach (var light in FindObjectsOfType<Light>())
            light.enabled = false;

        // stop the background music
        audioManager.EnableBGM(false);

        // load the specific scene 
        SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);

        // constantly load the progress bar
        while (slider.value < 1f)
        {
            // update loading bar progress to player prefs
            progress += (Time.deltaTime / loadEstimate);
            Storage.SaveFloat(LOCAL_PLAYER, StorageType.Progress, progress);

            // update slider value & text
            slider.value = progress;
            yield return new WaitForSeconds(Time.deltaTime);
        }

        // destroy the sub loader object
        Destroy(subLoader.gameObject);        
    }
    #endregion

    #region Common Loading Method
    /// <summary>
    /// A common method to load environmental objects in the scene,
    /// and also register a spawn holder in blackboard, a spawn holder
    /// is a transform that hold all the spawned obejcts in the scene
    /// </summary>
    void LoadSceneEnvironment()
    {
        // create environmental object
        var environment = Instantiate(pref_environment);
        environment.name = "Environmental Objects";

        // setup canvas & spawn holder
        SetupCanvas();
        SetupSpawnHolder();

        // add the spawned object into the visible object list
        visibleObjs.Add(environment);
        visibleObjs.Add(spawnHolder.gameObject);
    }

    /// <summary>
    /// A common method to load character objects in all table games
    /// </summary>
    void LoadTableCharacters()
    {
        // get table player's information
        var table = FindObjectOfType<TableInformation>();
        table.LoadSeatsInfo();
    }

    /// <summary>
    /// Method to spawn the UI prefab for this scene, this UI prefab contains 
    /// player board, instruction, buttons...etc. 
    /// </summary>
    void LoadUIManager()
    {
        // spawn the ui manager
        obj_uiManager = Instantiate(pref_uiManager, canvas).GetComponent<UIManager>();
    }

    /// <summary>
    /// A method to implement game start functions, it is applied to every game
    /// </summary>
    void GameStart_Common(bool cursorLock)
    {
        // setup the player board
        obj_uiManager.playerBoard.BindToPlayer(localPlayer);

        // display default hidden UI objects
        obj_uiManager.SetDefaultObjectVisibility(true);

        // check to see whether or not to lock the cursor
        CursorController.LockCursor(cursorLock);
    }

    #endregion

    #region Separate Loading Method
    /// <summary>
    /// Method to create character object in the casino scene
    /// </summary>
    void LoadCharacterController()
    {
        // first, spawn the character
        obj_character = Instantiate(pref_character);
        obj_character.name = "Player";

        // and then load the character model asset
        var index = Storage.LoadInt(LOCAL_PLAYER, StorageType.ModelIndex);
        var asset = GetModelPrefab(index);
        Instantiate(asset, obj_character.transform.GetChild(0));

        // save model index
        obj_character.GetComponent<Player>().modelIndex = index;

        // add the spawned object into the visible object list
        visibleObjs.Add(obj_character);
    }

    /// <summary>
    /// Method to create game mamanger in the casino scene
    /// </summary>
    void LoadCasinoManager()
    {
        // create the game manager
        obj_gameManager = Instantiate(pref_manager);

        // initialie seat manager
        var seatManager = obj_gameManager.GetComponent<SeatManager>();
        seatManager.Setup();

        // attach seat manager to all tables
        foreach (var table in FindObjectsOfType<TableInformation>())
        {
            table.seatManager = seatManager;
            table.Setup();
        }

        // initialize crowd
        obj_gameManager.GetComponent<CrowdManager>().Setup();
    }

    /// <summary>
    /// A method to actually start the casino scene, when this method is
    /// called, the user can start controller his character
    /// </summary>
    void GameStart_Casino()
    {
        // unload the previous scene if it is loaded
        if (SCENE_PREVIOUS != ""
            && SceneManager.GetSceneByName(SCENE_PREVIOUS).isLoaded)
        {
            SceneManager.UnloadSceneAsync(SCENE_PREVIOUS);
        }

        // setup previous scene name
        SCENE_PREVIOUS = SCENE_INCASINO;

        // set character to be active and bind it to the blackboard
        obj_character.SetActive(true);
        localPlayer = obj_character.GetComponent<Player>();        
        localPlayer.chip = Storage.LoadInt(LOCAL_PLAYER, StorageType.Chip);
        localPlayer.gem = Storage.LoadInt(LOCAL_PLAYER, StorageType.Gem);
        localPlayer.name = Storage.LoadString(LOCAL_PLAYER, StorageType.Name);
        localPlayer.description = Storage.LoadString(LOCAL_PLAYER, StorageType.Description);

        // spawn portal effects
        FindObjectOfType<PortalTagManager>().Setup(obj_character.transform);

        // play crowd sound 
        audioManager.EnableEnvironmentSound(true);

        // play background music if it is not playing
        if (!audioManager.srcBgm.isPlaying)
        {
            audioManager.srcBgm.clip = audioManager.bgm0;
            audioManager.EnableBGM(true);
        }

        // check to see if this player has previously entered the casino
        if (Storage.LoadBool(LOCAL_PLAYER,StorageType.HasRecord))
        {
            // if yes, move the character to previous position & rotation
            obj_character.transform.position = 
                Storage.LoadVector3(LOCAL_PLAYER, StorageType.Position);
            obj_character.transform.eulerAngles = 
                Storage.LoadVector3(LOCAL_PLAYER, StorageType.Rotation);
        }
        else
        {
            // otherwise, initialize character's position & rotation
            obj_character.transform.position = new Vector3(0.7f, 0f, -13f);
            obj_character.transform.eulerAngles = Vector3.zero;
        }
    }

    /// <summary>
    /// Method to create game manager for texas bonus
    /// </summary>
    void LoadTexasBonusManager()
    {
        // create the game manager
        obj_gameManager = Instantiate(pref_manager);

        // setup the game manager
        obj_gameManager.GetComponent<TexasBonus.GameManager>().Setup(canvas);
    }
    
    /// <summary>
    /// A method to actually start the texas bonus scene
    /// </summary>
    void GameStart_Texas()
    {
        // call the finished loading method and ready to start the game
        obj_gameManager.GetComponent<TexasBonus.GameManager>().FinishedLoading();

        // setup previous scene name
        SCENE_PREVIOUS = SCENE_TEXAS;

        // start the background music again
        audioManager.srcBgm.clip = audioManager.bgm1;
        audioManager.EnableBGM(true);
    }

    /// <summary>
    /// Method to create game manager for blackjack
    /// </summary>
    void LoadBlackjackManager()
    {
        // create the game manager
        obj_gameManager = Instantiate(pref_manager);

        // setup the game manager
        obj_gameManager.GetComponent<Blackjack.GameManager>().Setup(canvas);
    }

    /// <summary>
    /// A method to actually start the blackjack scene
    /// </summary>
    void GameStart_Blackjack()
    {
        // call the finished loading method and ready to start the game
        obj_gameManager.GetComponent<Blackjack.GameManager>().FinishedLoading();

        // setup previous scene name
        SCENE_PREVIOUS = SCENE_TEXAS;

        // start the background music again
        audioManager.srcBgm.clip = audioManager.bgm1;
        audioManager.EnableBGM(true);
    }
    #endregion

}