using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Label : MonoBehaviour
{
    [HideInInspector]
    public Image bg;            // background image for this label
    [HideInInspector]
    public TextMeshProUGUI tmp; // text component for this label

    void Awake()
    {
        bg = GetComponentInChildren<Image>();
        tmp = GetComponentInChildren<TextMeshProUGUI>();
    }

    /// <summary>
    /// Methdo to enable or disable the label
    /// </summary>
    /// <param name="flag">true for enable, false for disable</param>
    public void Switch(bool flag)
    {
        // enable / disable background image and text
        bg.enabled = flag;
        tmp.enabled = flag;

        // when disabling, remove contents from text 
        if (flag)
            tmp.text = "";
    }

    /// <summary>
    /// Method to enable the label and add content to the text
    /// </summary>
    /// <param name="message"></param>
    public void Display(string message)
    {
        Switch(true);
        tmp.text = message;
    }
}