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
    public Image fadeImage;

    public GameObject pref_environment;
    private GameObject obj_environment;

    public GameObject pref_character;
    private GameObject obj_character;

    public GameObject pref_manager;
    private GameObject obj_manager;

    private List<LoadData> loadData;

    void Start()
    {
        InitializeLoadData();

        SceneManager.SetActiveScene(SceneManager.GetSceneByName(BB.SCENE_INGAME));

        StartCoroutine(Load());
        
    }

    /// <summary>
    /// Method to initialize load data
    /// The usage of this is to split the whole loading progress
    /// into pieces and load them from different time frame of the
    /// loading
    /// </summary>
    void InitializeLoadData()
    {
        loadData = new List<LoadData>();

        loadData.Add(new LoadData(0.4f, LoadCharacter));
        loadData.Add(new LoadData(0.5f, LoadEnvironment));        
        loadData.Add(new LoadData(0.6f, LoadManagers));
    }

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

            if (SceneManager.GetSceneByName(BB.SCENE_HOMEPAGE).isLoaded)
            {
                progress = PlayerPrefs.GetFloat("LoadingBarProgress");
            }
            else
            {
                progress += (Time.deltaTime / BB.loadEstimate);
            }

            //progress = PlayerPrefs.GetFloat("LoadingBarProgress");
            //progress += (Time.deltaTime / 1); 
            yield return new WaitForSeconds(Time.deltaTime);
        }

        // start to fade in and out after 2 seconds
        yield return new WaitForSeconds(2f);
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
        // get the color of the fadeImage
        var color = fadeImage.color;

        // initialize fadeTime and its timer
        float fadeTime;
        float fadeTimer;
        
        // fade into white
        fadeTime = 0.25f;
        fadeTimer = 0f;
        while (fadeTimer < fadeTime)
        {
            color.a = Mathf.Lerp(0f, 1f, (fadeTimer / fadeTime));
            fadeImage.color = color;
            fadeTimer += Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }

        // start the game
        GameStart();

        // fade out from white
        fadeTime = 2f;
        fadeTimer = 0f;
        while (fadeTimer < fadeTime)
        {
            color.a = Mathf.Lerp(1f, 0f, (fadeTimer / fadeTime));
            fadeImage.color = color;
            fadeTimer += Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        
        // destroy the fade image object
        Destroy(fadeImage.gameObject);
    }

    void LoadEnvironment()
    {
        obj_environment = Instantiate(pref_environment);

    }

    void LoadCharacter()
    {
        // first, spawn the character
        obj_character = Instantiate(pref_character);

        // and then load the character model asset
        var path = PlayerPrefs.GetString("CharacterModelPrefab");
        var asset = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject));
        var model = Instantiate(asset, obj_character.transform.GetChild(0));
    }

    void LoadManagers()
    {
        obj_manager = Instantiate(pref_manager);
        obj_manager.GetComponent<SeatManager>().Setup();
        obj_manager.GetComponent<CrowdManager>().Setup();
    }

    void GameStart()
    {
        if (SceneManager.GetSceneByName(BB.SCENE_HOMEPAGE).isLoaded)
            SceneManager.UnloadSceneAsync(BB.SCENE_HOMEPAGE);

        obj_character.SetActive(true);

        obj_character.transform.position = new Vector3(0.7f, 0f, -13f);
        obj_character.transform.eulerAngles = Vector3.zero;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    

}