using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Blackboard;

public class WagerBehaviour : MonoBehaviour
{
    [Header("Wager Prefab")]
    [Tooltip("A wager model for red($5)")]
    public GameObject redChip;
    [Tooltip("A wager model for blue($10)")]
    public GameObject blueChip;
    [Tooltip("A wager model for yellow($25)")]
    public GameObject yellowChip;
    [Tooltip("A wager model for pink($50)")]
    public GameObject pinkChip;
    [Tooltip("A wager model for black($100)")]
    public GameObject blackChip;

    [Header("Other")]
    [Tooltip("Example wager models")]
    public GameObject[] wager;
    [Tooltip("A component that register/play wager movement")]
    public WagerAnimator wagerAnimator; 

    [HideInInspector]
    public Vector3[][] wagerPos;       // positions where the wager stacks are spawned
    [HideInInspector]
    public GameObject[][] wagerStacks; // object to hold wager stacks
    
    private float wagerHeight;         // used to determine current wager height when spawning heaps of wagers
    private float wagerAngle;          // used to determine current wager angle when spawning heaps of wagers

    /// <summary>
    /// Method to instantiate a wager stack for a player, in a specific wager slot
    /// </summary>
    /// <param name="playerId">index of the player</param>
    /// <param name="slotId">index of the wager slot</param>
    /// <param name="value">value of the wager stack</param>
    public void InstantiateWagerStack(int playerId, int slotId, int value)
    {
        // first of all, create an empty game object that holds wager stack
        wagerStacks[playerId][slotId] = new GameObject("Chip");
        wagerStacks[playerId][slotId].transform.parent = spawnHolder;
        wagerStacks[playerId][slotId].transform.localScale = Vector3.one * 1.75f;
        wagerStacks[playerId][slotId].transform.position = wagerPos[playerId][slotId];

        // initialize the spawning data
        var remaining = value;
        wagerHeight = 0f;
        wagerAngle = 0f;

        // play bet sound effect
        audioManager.PlayAudio(audioManager.clipPlaceWager, AudioType.Sfx);

        // keep spawning wager models to the wager stack until the remaining reaches 0
        while (remaining > 0)
        {
            // the spawning wager
            GameObject obj;

            // case1: black chip($100)
            if (remaining >= 100)
            {
                remaining -= 100;
                obj = Instantiate(blackChip, wagerStacks[playerId][slotId].transform);
            }
            // case2: pink chip($50)
            else if (remaining >= 50)
            {
                remaining -= 50;
                obj = Instantiate(pinkChip, wagerStacks[playerId][slotId].transform);
            }
            // case3: yellow chip($25)
            else if (remaining >= 25)
            {
                remaining -= 25;
                obj = Instantiate(yellowChip, wagerStacks[playerId][slotId].transform);
            }
            // case4: blue chip($10)
            else if (remaining >= 10)
            {
                remaining -= 10;
                obj = Instantiate(blueChip, wagerStacks[playerId][slotId].transform);
            }
            // default case: red chip($5)
            else 
            {
                remaining = 0;
                obj = Instantiate(redChip, wagerStacks[playerId][slotId].transform);
            }

            // adjust spawned object's position & rotation
            obj.transform.localPosition = new Vector3(0f, wagerHeight, 0f);
            obj.transform.localEulerAngles = new Vector3(-90f, wagerAngle, 0f);

            // increment height & angle
            wagerAngle += Const.TABLE_WAGER_ANGLE_INCREMENT;
            wagerHeight += Const.TABLE_WAGER_HEIGHT_INCREMENT;
        }
    }

    /// <summary>
    /// Method to clone a ante wager stack, to a specific wager slot 
    /// </summary>
    /// <param name="playerId">index of the player</param>
    /// <param name="slotId">index of the wager slot</param>
    /// <param name="isDouble">whether or not the amount is doubled</param>
    public void CloneAnteWagerStack(int playerId, int slotId, bool isDouble)
    {
        // first of all, spawn a parent object to hold all the clone chips
        wagerStacks[playerId][slotId] = new GameObject("Chip");
        wagerStacks[playerId][slotId].transform.parent = spawnHolder;
        wagerStacks[playerId][slotId].transform.localScale = Vector3.one * 1.75f;
        wagerStacks[playerId][slotId].transform.position = wagerPos[playerId][slotId];

        // initial default height & angle for the wager stack
        wagerHeight = 0f;
        wagerAngle = 0f;

        // keep spawning wager models 
        for (int i = 0; i < (isDouble ? 2 : 1); i++)
        {
            for (int j = 0; j < wagerStacks[playerId][0].transform.childCount; j++)
            {
                // spawn a wager model
                var obj = Instantiate(wagerStacks[playerId][0].transform.GetChild(j), wagerStacks[playerId][slotId].transform);
                obj.transform.localPosition = Vector3.up * wagerHeight;
                obj.transform.localEulerAngles = new Vector3(-90f, wagerAngle, 0f);

                // increment height & angle
                wagerAngle += Const.TABLE_WAGER_ANGLE_INCREMENT;
                wagerHeight += Const.TABLE_WAGER_HEIGHT_INCREMENT;
            }
        }
    }

    /// <summary>
    /// Method to multiply wager on a specific wager stack, 
    /// it is called when the player wins
    /// </summary>
    /// <param name="playerId">index of the player</param>
    /// <param name="slotId">index of the wager slot</param>
    /// <param name="multiplier">multiplier</param>
    public void MultiplyWagerModel(int playerId, int slotId, int multiplier)
    {
        // find the parent object that holds wager models in this specific part
        var parent = wagerStacks[playerId][slotId].transform;

        // find the childcount so that we know the height of the parent object
        var childCount = parent.transform.childCount;
        wagerHeight = parent.GetChild(childCount - 1).localPosition.y;
        wagerAngle = parent.GetChild(childCount - 1).localEulerAngles.y;

        // spawn 'n' times
        for (int i = 0; i < multiplier; i++)
        {
            for (int j = 0; j < childCount; j++)
            {
                // increment height & angle
                wagerAngle += Const.TABLE_WAGER_ANGLE_INCREMENT;
                wagerHeight += Const.TABLE_WAGER_HEIGHT_INCREMENT;

                // spawn a wager model
                var obj = Instantiate(parent.GetChild(j), parent);

                // calculate spawning position and target position
                var spawnPos = wagerAnimator.GetAssociateChipSlot(obj.tag);
                var targetPos = new Vector3(0f, wagerHeight, 0f);

                // register the movement animation
                var chipAnimation = obj.gameObject.AddComponent<ChipAnimation>();
                chipAnimation.Take(spawnPos, targetPos, new Vector3(-90f, wagerAngle, 0f));

                // add the animation to wager animator's queue
                wagerAnimator.Add(chipAnimation);
            }
        }
    }

    /// <summary>
    /// Method to add a single wager model to a player's specific wager stack
    /// </summary>
    /// <param name="playerId">the player id</param>
    /// <param name="slotId">the index of the specific slot</param>
    /// <param name="amount">value of the chip</param>
    public void AddWagerModel(int playerId, int slotId, int amount)
    {
        // create an empty game object that holds the wager stack
        if (wagerStacks[playerId][slotId] == null)
        {
            wagerHeight = 0f;
            wagerAngle = 0f;
            wagerStacks[playerId][slotId] = new GameObject("Chip");
            wagerStacks[playerId][slotId].transform.parent = spawnHolder;
            wagerStacks[playerId][slotId].transform.localScale = Vector3.one * 1.75f;
            wagerStacks[playerId][slotId].transform.position = wagerPos[playerId][slotId];
        }

        // play bet sound effect
        audioManager.PlayAudio(audioManager.clipPlaceWager, AudioType.Sfx);

        // spawn a single chip into the holder
        GameObject obj;
        if (amount == 5)
            obj = Instantiate(redChip, wagerStacks[playerId][slotId].transform);

        else if (amount == 10)
            obj = Instantiate(blueChip, wagerStacks[playerId][slotId].transform);

        else if (amount == 25)
            obj = Instantiate(yellowChip, wagerStacks[playerId][slotId].transform);

        else if (amount == 50)
            obj = Instantiate(pinkChip, wagerStacks[playerId][slotId].transform);

        else
            obj = Instantiate(blackChip, wagerStacks[playerId][slotId].transform);

        // adjust spawned object's position and rotation
        obj.transform.localPosition = new Vector3(0f, wagerHeight, 0f);
        obj.transform.localEulerAngles = new Vector3(-90f, wagerAngle, 0f);

        // increment height & angle
        wagerAngle += Const.TABLE_WAGER_ANGLE_INCREMENT;
        wagerHeight += Const.TABLE_WAGER_HEIGHT_INCREMENT;
    }

    /// <summary>
    /// Method to clear all wager model for a player in a specific slot
    /// </summary>
    /// <param name="playerId">index of the player</param>
    /// <param name="slotId">index of the wager slot</param>
    public void ClearWagerStackForSingleSlot(int playerId, int slotId)
    {
        if (wagerStacks[playerId][slotId] != null)
            Destroy(wagerStacks[playerId][slotId]);
    }
    public void ClearWagerStackForSinglePlayer(int playerId)
    {
        for (int i = 0; i < wagerStacks[playerId].Length; i++)
            ClearWagerStackForSingleSlot(playerId, i);
    }
    public void ClearWagerStackForAll()
    {
        for (int i = 0; i < wagerStacks.Length; i++)
            ClearWagerStackForSinglePlayer(i);
    }

    /// <summary>
    /// Method to setup a animation queue for a specific wager stack, each wager
    /// in the wager stack will be throwing into banker's wager slot one by one
    /// </summary>
    /// <param name="playerId">index of the player</param>
    /// <param name="wagerIndex">index of the wager slot</param>
    public void TakingChipsAway(int playerId, int wagerIndex)
    {
        // return if the wager stack does not exist
        if (wagerStacks[playerId][wagerIndex] == null)
            return;

        // define the wager stack as the parent
        var parent = wagerStacks[playerId][wagerIndex].transform;

        // run through each wager in this wager stack
        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            // get the wager model
            var obj = parent.GetChild(i);

            // get the wager slot position for this type of chip
            var targetPos = wagerAnimator.GetAssociateChipSlot(obj.tag);

            // register the movement animation
            var chipAnimation = obj.gameObject.AddComponent<ChipAnimation>();
            chipAnimation.Lose(targetPos);

            // add the animation to wager animator's queue
            wagerAnimator.Add(chipAnimation);
        }
    }

}
