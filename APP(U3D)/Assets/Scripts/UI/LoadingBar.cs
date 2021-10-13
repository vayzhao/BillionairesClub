using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingBar : MonoBehaviour
{
    [Header("UI Widgets")]
    [Tooltip("A slider shows the loading progress")]
    public Slider slider;
    [Tooltip("A text shows the loading progress percentage")]
    public TextMeshProUGUI loadingText;
    [Tooltip("A text shows the loading text")]
    public TextMeshProUGUI loadingContent;

    [Space(25f)]
    [Tooltip("A list of loading message")]
    public List<string> loadingMessages;
    [Tooltip("The update rate for loading message")]
    public float messageRefreshRate = 3f;
    private float messageRefreshTimer; // a timer that will reset every 'n' second

    private float progress;            // the loading progress    
    private Transform model;           // the selected player character model
    private Transform content;         // a transform that holds all loading ui widgets

    /// <summary>
    /// Method to setup the loading progress bar and get it started 
    /// </summary>
    /// <param name="model">the selected player character model</param>
    public void Setup(Transform model)
    {
        // bind model & content holder to this script
        this.model = model;
        this.content = transform.GetChild(0);

        // active content holder to show all loading ui widgets
        content.gameObject.SetActive(true);

        // start a cortoutine to load the progress bar
        StartCoroutine(LoadingProgress());
    }

    /// <summary>
    /// An IEnumerator to update the loading bar every frame
    /// </summary>
    /// <returns></returns>
    IEnumerator LoadingProgress()
    {
        // initialize progress and messageRefreshTimer
        progress = 0f;
        messageRefreshTimer = 0f;

        // update loading bar progress to player prefs
        PlayerPrefs.SetFloat("LoadingBarProgress", progress);

        // display the first loading message
        MessageRefresh();

        // delte current event system
        Destroy(FindObjectOfType<UnityEngine.EventSystems.EventSystem>().gameObject);

        // load the ingame scene
        SceneManager.LoadScene(Blackboard.SCENE_INCASINO, LoadSceneMode.Additive);

        // constantly load the progress bar
        while (progress < 1f) 
        {
            // update loading bar progress to player prefs
            progress += (Time.deltaTime / Blackboard.loadEstimate);
            PlayerPrefs.SetFloat("LoadingBarProgress", progress);

            // increment message refresh timer and check to see 
            // if it is needed to refresh loading message
            messageRefreshTimer += Time.deltaTime;
            if (messageRefreshTimer > messageRefreshRate)
                MessageRefresh();

            // force progress to be not greater than 1f
            if (progress > 1f)
                progress = 1f;

            // update slider value & text
            slider.value = progress;
            loadingText.text = $"Warming Up({(progress * 100).ToString("N0")}%)";

            yield return new WaitForSeconds(Time.deltaTime);
        }

        // when loading finished, set the model animation to jump
        model.GetComponent<Animator>().SetTrigger("Jump");

        // update loading text & message
        loadingText.text = "complete";
        loadingContent.text = "He is seriously ready!";
    }

    /// <summary>
    /// Method to update random text in loading interface
    /// </summary>
    void MessageRefresh()
    {
        // reset message refresh timer
        messageRefreshTimer = 0f;

        // return if the loading message is ran out
        if (loadingMessages.Count == 0)
            return;

        // pick a random message from the message list and apply to the text component
        var index = UnityEngine.Random.Range(0, loadingMessages.Count);
        loadingContent.text = loadingMessages[index];
        loadingMessages.RemoveAt(index);
    }
}
