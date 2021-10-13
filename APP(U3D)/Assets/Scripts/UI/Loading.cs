using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadData
{
    public float atPercentage;         // start to load this data at ??% of the total loading progress
    public delegate void loadMethod(); // loading method to call for this loadData
    public loadMethod method;          // the stored method

    // Method to instantiate a LoadData
    public LoadData(float atPercentage, loadMethod method)
    {
        this.atPercentage = atPercentage;
        this.method = method;
    }
}

public class Loading : MonoBehaviour
{
    [Header("Scene Settings")]
    public SceneType sceneType;
    public GameType gameType;

    [Header("UI Elements")]
    public Transform canvas;
    public Image pref_fadeSprite;
    public GameObject pref_playerBoard;
    public GameObject pref_subLoadingBar;

    [Header("Prefab Objects")]
    public GameObject pref_environment;    
    public GameObject pref_character;    
    public GameObject pref_manager;

    private GameObject obj_environment;
    private GameObject obj_character;
    private GameObject obj_manager;
    private GameObject obj_playerBoard;
    private List<LoadData> loadData;

    private bool isDebugMode;

    void Start()
    {
        DebugMode();

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
        if ((sceneType == SceneType.InCasino && !SceneManager.GetSceneByName(Blackboard.SCENE_HOMEPAGE).isLoaded)
            || (sceneType == SceneType.InGame && !SceneManager.GetSceneByName(Blackboard.SCENE_INCASINO).isLoaded))
        {
            isDebugMode = true;
            Debug.Log("Debug Mode is on!");
            StartCoroutine(ProgressRunner());


            // create a test player
            var player = new Player();
            player.modelIndex = PlayerPrefs.GetInt("CharacterModelIndex");
            player.chip = 10000;
            player.gem = 50;
            Blackboard.thePlayer = player;
        }
    }
    IEnumerator ProgressRunner()
    {   
        var progress = 0f;
        PlayerPrefs.SetFloat("LoadingBarProgress", progress);
        while (progress < 1f)
        {
            progress += (Time.deltaTime / Blackboard.loadEstimate);
            PlayerPrefs.SetFloat("LoadingBarProgress", progress);
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
        // setup load data list
        loadData = new List<LoadData>();
        loadData.Add(new LoadData(0.20f, LoadSceneEnvironment));
        loadData.Add(new LoadData(0.40f, LoadCharacterController));
        loadData.Add(new LoadData(0.60f, LoadCasinoManager));
        loadData.Add(new LoadData(0.80f, LoadPlayerBoard));

        // set this scene to be active
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(Blackboard.SCENE_INCASINO));

        // start the loading coroutine
        StartCoroutine(Load());
    }

    /// <summary>
    /// A method to load all contents needed in texas bonus scene
    /// </summary>
    void LoadTexasBonus()
    {
        // setup load data list
        loadData = new List<LoadData>();
        loadData.Add(new LoadData(0.20f, LoadSceneEnvironment));
        loadData.Add(new LoadData(0.40f, LoadTableCharacters));
        loadData.Add(new LoadData(0.60f, LoadTexasBonusManager));
        loadData.Add(new LoadData(0.80f, LoadPlayerBoard));

        // set this cene to be active 
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(Blackboard.SCENE_TEXAS));

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
            // ready to load the next loadData
            if (loadData.IsReady(progress))
                loadData.Run();

            // get the updated loading bar progress
            progress = PlayerPrefs.GetFloat("LoadingBarProgress");
            yield return new WaitForSeconds(Time.deltaTime);
        }

        // set fade delay time, if takes 2 seconds wait time to fade when transiting
        // from homepage scene to casino scene
        var fadeDelay = SceneManager.GetSceneByName(Blackboard.SCENE_HOMEPAGE).isLoaded ? 2f : 0f;

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
        // create a fade sprite in canvas
        var fade = Instantiate(pref_fadeSprite, canvas);

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

        // unload casino scene if it is not in debug mode
        if (!isDebugMode && sceneType != SceneType.InCasino)
            SceneManager.UnloadSceneAsync(Blackboard.SCENE_INCASINO);

        // start the game
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
                sceneName = Blackboard.SCENE_TEXAS;
                break;
            default:
                break;
        }

        // store player's information in that table
        table.SaveSeatsInfo();

        // start loading coroutine
        StartCoroutine(SubLoading(sceneName));
    }

    /// <summary>
    /// An IEnumerator to update the sub loading bar every frame
    /// </summary>
    /// <returns></returns>
    IEnumerator SubLoading(string sceneName)
    {
        // first, spawn the sub-loader object
        var subLoader = Instantiate(pref_subLoadingBar, canvas);
        var slider = subLoader.GetComponentInChildren<Slider>();

        // force slider's value to be 0
        slider.value = 0f;

        // reset progress value in player prefs
        var progress = 0f;        
        PlayerPrefs.SetFloat("LoadingBarProgress", progress);

        // disable light component from this scene
        FindObjectOfType<Light>().enabled = false;

        // load the specific scene 
        SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);

        // constantly load the progress bar
        while (slider.value < 1f)
        {
            // update loading bar progress to player prefs
            progress += (Time.deltaTime / Blackboard.loadEstimate);
            PlayerPrefs.SetFloat("LoadingBarProgress", progress);

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

        // setup spawn holder
        Blackboard.SetupSpawnHolder();
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
    /// Method to spawn the player board, player board contains information
    /// such as player's portrait image, remaning chips and gems
    /// </summary>
    void LoadPlayerBoard()
    {
        obj_playerBoard = Instantiate(pref_playerBoard, canvas);
        obj_playerBoard.SetActive(false);

    }

    /// <summary>
    /// A method to implement game start functions, it is applied to every game
    /// </summary>
    void GameStart_Common(bool cursorLock)
    {
        // display the playerboard
        obj_playerBoard.SetActive(true);

        // bind the player to the player board
        obj_playerBoard.GetComponent<PlayerBoard>().BindToPlayer(Blackboard.thePlayer);

        // check to see whether or not to lock the cursor
        if (cursorLock)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
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
        var index = PlayerPrefs.GetInt("CharacterModelIndex");
        var asset = Blackboard.GetModelPrefab(index);
        Instantiate(asset, obj_character.transform.GetChild(0));

        // save model index
        obj_character.GetComponent<Player>().modelIndex = index;
    }

    /// <summary>
    /// Method to create game mamanger in the casino scene
    /// </summary>
    void LoadCasinoManager()
    {
        // create the game manager
        obj_manager = Instantiate(pref_manager);

        // initialie seat manager
        var seatManager = obj_manager.GetComponent<SeatManager>();
        seatManager.Setup();

        // attach seat manager to all tables
        foreach (var table in FindObjectsOfType<TableInformation>())
        {
            table.seatManager = seatManager;
            table.Setup();
        }

        // initialize crowd
        obj_manager.GetComponent<CrowdManager>().Setup();
    }

    /// <summary>
    /// A method to actually start the casino scene, when this method is
    /// called, the user can start controller his character
    /// </summary>
    void GameStart_Casino()
    {
        // unload the casino scene if it is loaded
        if (SceneManager.GetSceneByName(Blackboard.SCENE_HOMEPAGE).isLoaded)
            SceneManager.UnloadSceneAsync(Blackboard.SCENE_HOMEPAGE);

        // set character to be active and move it to the start position
        obj_character.SetActive(true);
        obj_character.transform.position = new Vector3(0.7f, 0f, -13f);
        obj_character.transform.eulerAngles = Vector3.zero;

        // initialize player's default chip and gem
        Blackboard.thePlayer = obj_character.GetComponent<Player>();
        Blackboard.thePlayer.chip = 15000;
        Blackboard.thePlayer.gem = 200;
    }

    /// <summary>
    /// Method to create game manager for texas bonus
    /// </summary>
    void LoadTexasBonusManager()
    {
        // create the game manager
        obj_manager = Instantiate(pref_manager);

        // setup the game manager
        obj_manager.GetComponent<TexasBonus.GameManager>().Setup(canvas);
    }

    /// <summary>
    /// A method to actually start the texas bonus scene
    /// </summary>
    void GameStart_Texas()
    {
        obj_manager.GetComponent<TexasBonus.GameManager>().FinishedLoading();
    }
    #endregion

}