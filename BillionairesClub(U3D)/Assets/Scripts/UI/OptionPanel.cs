using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;

public class OptionPanel : MonoBehaviour
{
    public static float volMaster = 0f;      // default value for master volume
    public static float volBgm = 0f;         // default value for bgm volume
    public static float volEnvironment = 0f; // default value for environment volume
    public static float volSfx = 0f;         // default value for sfx volume
    public static float volUI = 0f;          // default value for UI
    public static int qualityLevel = 2;      // default value for quality level
    public static int resolutionIndex = -1;  // default value for resolution index

    private const float VOL_MIN = -10f;  // minimum value for the volume slider
    private const float VOL_MUTE = -80f; // volume that completely muted

    [Header("Resolution")]
    [Tooltip("The dropdown menu of resolution")]
    public TMP_Dropdown dropdown;
    [Tooltip("Images from all quality level buttons")]
    public Image[] qualityBtnsImg;
    [Tooltip("Sprite used for selected quality level button")]
    public Sprite btnSelected;
    [Tooltip("Sprite used for unselected quality level button")]
    public Sprite btnUnselected;
    [Tooltip("The check box for full screen option")]
    public Toggle fullScreenToggle;

    [Header("Audio")]
    [Tooltip("The audio mixer for all channels")]
    public AudioMixer audioMixer;
    [Tooltip("Slider for Master volume")]
    public Slider sliderMaster;
    [Tooltip("Slider for BGM volume")]
    public Slider sliderBgm;
    [Tooltip("Slider for Environment volume")]
    public Slider sliderEnvironment;
    [Tooltip("Slider for Sfx volume")]
    public Slider sliderSfx;
    [Tooltip("Slider for UI volume")]
    public Slider sliderUI;

    private Resolution[] resolutions;

    private void Start()
    {
        InitializeResolutionDropdown();
        ResetOptionPanel();
    }

    /// <summary>
    /// Method to fix every component in this option menu
    /// </summary>
    private void ResetOptionPanel()
    {
        // resolution
        dropdown.value = resolutionIndex;
        dropdown.RefreshShownValue();

        // full-screen
        fullScreenToggle.isOn = Screen.fullScreen;

        // quality level
        for (int i = 0; i < qualityBtnsImg.Length; i++)
            qualityBtnsImg[i].sprite = i == qualityLevel ? btnSelected : btnUnselected;

        // volume sliders
        sliderMaster.value = volMaster;
        sliderBgm.value = volBgm;
        sliderEnvironment.value = volEnvironment;
        sliderSfx.value = volSfx;
        sliderUI.value = volUI;
    }

    #region Resolution
    /// <summary>
    /// Method to initialize resolution dropdown panel
    /// </summary>
    private void InitializeResolutionDropdown()
    {
        // get all the avaialble resolutions for this monitor 
        resolutions = Screen.resolutions.Select(x => new Resolution { width = x.width, height = x.height }).OrderBy(x => x.width).Distinct().ToArray();

        // apply resolution data into the dropdown panel
        dropdown.ClearOptions();
        dropdown.AddOptions(resolutions.Select(x => $"{x.width}x{x.height}").ToList());

        // automatically select the best resolution
        var bestIndex = resolutionIndex == -1 ? dropdown.options.Count - 1 : resolutionIndex;
        dropdown.value = bestIndex;
        SetResolution(bestIndex);

        // update resolution dropdown
        StartCoroutine(UpdateResolutionDropdown(Screen.fullScreen));
    }
    /// <summary>
    /// Method to change resolution
    /// </summary>
    /// <param name="resIndex">index of the select resolution</param>
    public void SetResolution(int resIndex)
    {
        // apply change globally
        resolutionIndex = resIndex;

        // find the resolution accordingly
        Resolution resolution = resolutions[resIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);

        // change the value in dropdown as well
        dropdown.value = resIndex;
    }

    /// <summary>
    /// Method to change full screen
    /// </summary>
    /// <param name="isFullscreen"></param>
    public void SetFullscreen(bool isFullscreen)
    {
        // automatically change to the best resolution when switching to full screen
        if (isFullscreen)
            SetResolution(resolutions.Length - 1);

        // change full screen mode
        Screen.fullScreen = isFullscreen;

        // update resolution dropdown
        StartCoroutine(UpdateResolutionDropdown(isFullscreen));
    }

    /// <summary>
    /// Method to change graphic quality level
    /// </summary>
    /// <param name="level"></param>
    public void SetGraphicQuality(int level)
    {
        // apply change globally
        qualityLevel = level;

        // change the image of the radio button
        for (int i = 0; i < qualityBtnsImg.Length; i++)
            qualityBtnsImg[i].sprite = i == level ? btnSelected : btnUnselected;

        // update graphic quality level
        QualitySettings.SetQualityLevel(level);
    }

    /// <summary>
    /// Method to update resolution dropdown panel when changing full screen option
    /// </summary>
    IEnumerator UpdateResolutionDropdown(bool flag)
    {
        // break the corotine when the game is running in editor
        if (Application.isEditor)
            yield break;

        // otherwise wait till the change to take effect
        while (Screen.fullScreen != flag)
            yield return new WaitForSeconds(Time.deltaTime);

        // disable dropdown panel if full-screen is on
        dropdown.enabled = !Screen.fullScreen;

        // refresh dropdown panel color
        dropdown.image.color = new Color(1f, 1f, 1f, dropdown.enabled ? 1f : 0.5f);

        // refresh value in resolution dropdown
        dropdown.RefreshShownValue();
    }

    #endregion

    #region Volume
    /// <summary>
    /// Methods to update volume for different channel of audio mixer group
    /// </summary>
    /// <param name="value"></param>
    public void OnMasterVolumeChange(float value)
    {
        volMaster = value;
        audioMixer.SetFloat(Const.VOL_CHANNEL_MASTER, value > VOL_MIN ? value : VOL_MUTE);        
    }
    public void OnBGMVolumeChange(float value)
    {
        volBgm = value;
        audioMixer.SetFloat(Const.VOL_CHANNEL_BGM, value > VOL_MIN ? value : VOL_MUTE);
    }
    public void OnEnvironmentChange(float value)
    {
        volEnvironment = value;
        audioMixer.SetFloat(Const.VOL_CHANNEL_ENVIRONMENT, value > VOL_MIN ? value : VOL_MUTE);
    }
    public void OnSfxChange(float value)
    {
        volSfx = value;
        audioMixer.SetFloat(Const.VOL_CHANNEL_SFX, value > VOL_MIN ? value : VOL_MUTE);
    }
    public void OnUIChange(float value)
    {
        volUI = value;
        audioMixer.SetFloat(Const.VOL_CHANNEL_UI, value > VOL_MIN ? value : VOL_MUTE);
    }
    #endregion
}