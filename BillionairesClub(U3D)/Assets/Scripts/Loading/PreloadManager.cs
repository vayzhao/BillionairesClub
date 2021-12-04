using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PreloadManager : MonoBehaviour
{
    [Header("Panels")]
    [Tooltip("A panel that displays preload UI elements")]
    public GameObject panel_preLoad;
    [Tooltip("A panel that display contents after preloaded")]
    public GameObject panel_afterLoad;

    [Header("UI Components")]
    [Tooltip("A text displays loading progress")]
    public TextMeshProUGUI loadingText;
    [Tooltip("A text displays lodaing contents")]
    public TextMeshProUGUI loadingContentText;
    [Tooltip("A slider that represents the loading progress")]
    public Slider loadingSlider;

    [Header("Cache")]
    [Tooltip("The holder of all spaning caches")]
    public Transform cacheHolder;
    [Tooltip("All character models that are used in game")]
    public GameObject[] cache_characters;
    [Tooltip("All environment models that are used in game")]
    public GameObject[] cache_environments;

    private Loader loader; // a class that contains all sort of methods to spawn objects 

    void Start() => InitializePreload();

    /// <summary>
    /// Method to detect whether or not the application has been preloaded,
    /// Return if it has, otherwise display a preload panel and start a
    /// loading coroutine to spawn heaps of resources 
    /// </summary>
    void InitializePreload()
    {
        // return if preload has been done already
        if (Blackboard.hasPreloaded)
        {
            panel_afterLoad.SetActive(true);
            return;
        }

        // otherwise, hide normal content and display pre load panel
        panel_preLoad.SetActive(true);
        panel_afterLoad.SetActive(false);

        // initialize the loading slider
        loadingSlider.value = 0f;
        loadingContentText.text = "";
        loadingText.text = $"Loading...({loadingSlider.value * 100f:N0}%)";

        // setup a loader
        loader = new Loader();
        loader.RegisterLoadingMethod(0.1f, LoadCharacterModels);
        loader.RegisterLoadingMethod(0.6f, LoadEnvironmentModels);

        // start the loading coroutine
        StartCoroutine(LoadingMethod());
    }

    /// <summary>
    /// An enumerator to run the loading slider, and to spawn caches during loading,
    /// also detect user's space key input inorder to start the game
    /// </summary>
    /// <returns></returns>
    IEnumerator LoadingMethod()
    {
        // keep increment value of the slider until it reaches 1f
        while (loadingSlider.value < 1f)
        {
            // check to see if it is ready to invoke the next spawn method
            if (loader.IsReady(loadingSlider.value))
                loader.Run();

            // increment value of the slider
            loadingSlider.value += Time.deltaTime / Blackboard.loadEstimate;
            loadingText.text = $"Loading...({this.loadingSlider.value * 100f:N0}%)";
            yield return new WaitForSeconds(Time.deltaTime);
        }

        // notify the player that the loading is completed
        loadingText.text = "Completed!";
        loadingContentText.text = "Press 'Space Key' to Enter the game.";
        
        // keep holding until the player presses space key
        while (!Input.GetKeyDown(KeyCode.Space))
            yield return new WaitForSeconds(Time.deltaTime);

        // switch the state to be true, so we don't have to do it again
        // next time when we come to scene
        Blackboard.hasPreloaded = true;
        
        // replace preLoad panel contents with afterLoad contents
        panel_preLoad.SetActive(false);
        panel_afterLoad.SetActive(true);
    }

    /// <summary>
    /// Method to spawn character models for preloading 
    /// </summary>
    void LoadCharacterModels()
    {
        // notify the player what it is loading
        loadingContentText.text = "Inviting VIP members to join...";

        // run through the character models, spawn and delete them
        for (int i = 0; i < cache_characters.Length; i++)
        {
            var cache = Instantiate(cache_characters[i], cacheHolder);
            Destroy(cache);
        }
    }

    /// <summary>
    /// Method to spawn environmental models for preloading
    /// </summary>
    void LoadEnvironmentModels()
    {
        // notify the player what it is loading
        loadingContentText.text = "Cleaning up tables and chairs...";

        // run through the environmental models, spawn and delete them
        for (int i = 0; i < cache_environments.Length; i++)
        {
            var cache = Instantiate(cache_environments[i], cacheHolder);
            Destroy(cache);
        }
    }
}