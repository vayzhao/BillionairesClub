using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    [Header("Sources")]
    public AudioSource srcBgm;
    public AudioSource srcEnvironment;
    public AudioSource srcSfx;
    public AudioSource srcUI;

    [Header("BGMs")]
    public AudioClip bgm0;
    public AudioClip bgm1;

    [Header("Environments")]
    public AudioClip clipEnvironment;

    [Header("Sfxs")]
    public AudioClip clipMouseEnterBtn;
    public AudioClip clipBonusPopup;
    public AudioClip clipPurchaseSuccessful;
    public AudioClip clipMouseClickBtn;
    public AudioClip clipReadyAndSave;
    public AudioClip clipSpaceBarTrigger;
    public AudioClip clipWarning;
    public AudioClip clipEnterPortal;
    public AudioClip[] clipPlaceWager;
    public AudioClip[] clipInputFieldChange;
    public AudioClip[] cashierGreetings;
    public AudioClip[] clipDealCards;
    public AudioClip[] clipShuffling;
    public AudioClip[] clipCheck;
    public AudioClip[] clipFold;
    public AudioClip[] clipChipAnimationStart;
    public AudioClip[] clipChipAnimationEnd;

    void Start()
    {
        Blackboard.audioManager = this;

        // play bgm0 by default
        srcBgm.clip = bgm0;
        EnableBGM(true);
    }

    public void EnableBGM(bool flag)
    {
        if (flag)
            srcBgm.Play();
        else
            srcBgm.Stop();
    }

    public void EnableEnvironmentSound(bool flag)
    {
        srcEnvironment.clip = clipEnvironment;
        if (flag)
            srcEnvironment.Play();
        else
            srcEnvironment.Stop();
    }

    public void PlayAudio(AudioClip clip, AudioType audioType)
    {
        switch (audioType)
        {
            case AudioType.Sfx:
                srcSfx.PlayOneShot(clip);
                break;
            case AudioType.UI:
                srcUI.PlayOneShot(clip);
                break;
            default:
                break;
        }
    }

    public void PlayAudio(AudioClip[] clips, AudioType audioType) 
    {
        switch (audioType)
        {
            case AudioType.Sfx:
                srcSfx.PlayOneShot(clips[UnityEngine.Random.Range(0, clips.Length)]);
                break;
            case AudioType.UI:
                srcUI.PlayOneShot(clips[UnityEngine.Random.Range(0, clips.Length)]);
                break;
            default:
                break;
        }
    }


}