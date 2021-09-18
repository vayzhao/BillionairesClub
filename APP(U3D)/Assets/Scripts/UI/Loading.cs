using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Loading : MonoBehaviour
{

    public Slider slider;
    public TextMeshProUGUI loadingText;
    public TextMeshProUGUI loadingContent;

    public List<string> loadingMessages;

    public float loadTimeEstimate = 6f;

    private float progress;
    private float subProgress;
    private Transform model;
    private Transform content;

    public void Setup(Transform model)
    {
        this.model = model;
        this.content = transform.GetChild(0);


        content.gameObject.SetActive(true);

        StartCoroutine(LoadingProgress());

    }


    IEnumerator LoadingProgress()
    {
        progress = 0f;
        subProgress = 0f;

        UpdateRandomLoadText();

        while (progress < 1f) 
        {
            progress += (Time.deltaTime / loadTimeEstimate);

            subProgress += Time.deltaTime;
            if (subProgress > 3f)
            {
                subProgress = 0f;
                UpdateRandomLoadText();
            }


            slider.value = progress;
            loadingText.text = $"Warming Up({(progress * 100).ToString("N0")}%)";

            yield return new WaitForSeconds(Time.deltaTime);
        }

        model.GetComponent<Animator>().SetTrigger("Jump");
        loadingText.text = "complete";
        loadingContent.text = "He is seriously ready!";


        
    }

    /// <summary>
    /// Method to update random text in loading interface
    /// </summary>
    void UpdateRandomLoadText()
    {
        // return if the loading message is ran out
        if (loadingMessages.Count == 0)
            return;

        // pick a random message from the message list and apply to the text component
        var index = UnityEngine.Random.Range(0, loadingMessages.Count);
        loadingContent.text = loadingMessages[index];
        loadingMessages.RemoveAt(index);
    }



}
