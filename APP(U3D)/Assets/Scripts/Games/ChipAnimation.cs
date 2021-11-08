using System;
using System.Collections;
using UnityEngine;

public class ChipAnimation : MonoBehaviour
{
    private Vector3 from;          // the start position in local of this obj
    private Vector3 toWhere;       // the end position in local of this obj
    private Vector3 originEuler;   // the start rotation in local of this obj
    private Vector3 finalEuler;    // the end rotation in local of this obj
    private bool removeWhenFinish; // whether or not to remove the obj when done
    private bool isTaking;         // determine whether or not this animation is for taking chips


    /// <summary>
    /// Method to setup a chip moving animation when gaining chips
    /// records the start and end positoin for the spawned chip
    /// </summary>
    /// <param name="from">spawning position</param>
    /// <param name="toWhere">destination</param>
    public void Take(Vector3 from, Vector3 toWhere, Vector3 finalEuler)
    {
        // update information
        this.from = transform.parent.InverseTransformPoint(from);
        this.toWhere = toWhere;
        this.originEuler = transform.localEulerAngles;
        this.finalEuler = finalEuler;
        this.removeWhenFinish = false;
        this.isTaking = true;

        // hide the chip game object
        gameObject.SetActive(false);
    }
    
    /// <summary>
    /// Method to setup a chip moving animation when losing chips
    /// records the end position
    /// </summary>
    /// <param name="toWhere"></param>
    public void Lose(Vector3 toWhere)
    {
        // update information
        this.from = transform.localPosition;
        this.toWhere = transform.parent.InverseTransformPoint(toWhere);
        this.originEuler = transform.localEulerAngles;
        this.finalEuler = Vector3.zero;
        this.removeWhenFinish = true;
    }

    /// <summary>
    /// Method to start playing the chip animation
    /// </summary>
    public void Play()
    {
        gameObject.SetActive(true);
        StartCoroutine(Movement());
    }
    IEnumerator Movement()
    {
        // initialize the progress 
        float progress = 0f;

        // play associated sound effect
        if (isTaking)
            Blackboard.audioManager.PlayAudio(Blackboard.audioManager.clipChipAnimationStart, AudioType.Sfx);
        else
            Blackboard.audioManager.PlayAudio(Blackboard.audioManager.clipChipAnimationEnd, AudioType.Sfx);

        // keep moving the object until the progress finishes
        while (progress < 1f)
        {
            // increment the progress
            progress += Time.deltaTime / Const.WAIT_TIME_CHIP_TRAVEL;
            
            // move and spin the chip object
            transform.localPosition = Vector3.Lerp(from, toWhere, progress);
            transform.localEulerAngles = Vector3.Slerp(originEuler, finalEuler, progress);

            yield return new WaitForSeconds(Time.deltaTime);
        }

        // play associated sound effect
        if (isTaking)
            Blackboard.audioManager.PlayAudio(Blackboard.audioManager.clipChipAnimationEnd, AudioType.Sfx);
        else
            Blackboard.audioManager.PlayAudio(Blackboard.audioManager.clipChipAnimationStart, AudioType.Sfx);

        // determine whether or not the remove the obj
        if (removeWhenFinish)
            Destroy(gameObject);
    }
}