using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WagerAnimator : MonoBehaviour
{
    [Header("Chip's slot")]
    [Tooltip("Slot for red poker chips")]
    public Transform slotRed;
    [Tooltip("Slot for blue poker chips")]
    public Transform slotBlue;
    [Tooltip("Slot for yellow poker chips")]
    public Transform slotYellow;
    [Tooltip("Slot for pink poker chips")]
    public Transform slotPink;
    [Tooltip("Slot for black poker chips")]
    public Transform slotBlack;

    private List<ChipAnimation> animations; // a list to hold all animations

    void Start()
    {
        animations = new List<ChipAnimation>();
    }

    /// <summary>
    /// Method to add a new animation into the animation player
    /// </summary>
    /// <param name="animation">new animation</param>
    public void Add(ChipAnimation animation) => animations.Add(animation);

    /// <summary>
    /// Method to clear all animations
    /// </summary>
    public void Clear() => animations.Clear();

    /// <summary>
    /// Method to obtain the associate chip slot for the given tag
    /// </summary>
    /// <param name="tag">tag of the poker chip</param>
    /// <returns></returns>

    public Vector3 GetAssociateChipSlot(string tag)
    {
        if (tag == Const.CHIP_TAG_RED)
            return slotRed.position;

        if (tag == Const.CHIP_TAG_BLUE)
            return slotBlue.position;

        if (tag == Const.CHIP_TAG_YELLOW)
            return slotYellow.position;

        if (tag == Const.CHIP_TAG_PINK)
            return slotPink.position;

        if (tag == Const.CHIP_TAG_BLACK)
            return slotBlack.position;

        return Vector3.zero;
    }

    /// <summary>
    /// Method to play all the animation in animations list one by one
    /// </summary>
    public void Play()
    {
        StartCoroutine(Playing());
    }
    IEnumerator Playing()
    {
        // compute the interval, more animations in the list, faster it plays
        var interval = Const.WAIT_TIME_CHIP_TOTAL / animations.Count;
        
        // keep playing animations until there is none
        while (animations.Count > 0)
        {
            animations[0].Play();
            animations.RemoveAt(0);
            yield return new WaitForSeconds(interval);
        }

        // clear the animation list
        Clear();
    }
}