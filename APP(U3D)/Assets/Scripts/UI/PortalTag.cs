using System;
using UnityEngine;

[Serializable]
public class PortalTag 
{
    [Header("Object")]
    [Tooltip("The game object that holds the tag UI components")]
    public GameObject tag;
    [Tooltip("The prefab game object that holds the effect")]
    public GameObject pref_portalEffect;
    [Tooltip("The inner color of the portal effect")]
    public Color innerColor;
    [Tooltip("The outer color of the portal effect")]
    public Color outerColor;

    [Header("Range")]
    [Tooltip("Position where the portal effect will be spawned at")]
    public Vector3 portalPosition;
    [Tooltip("Lossy scale for the portal effect")]
    public Vector3 portalLossyScale;
    [Tooltip("Trigger distance for showing tooltip")]
    public float triggerDistance;

    private GameObject obj_portalEffect; // the gameobject that holds the effect

    /// <summary>
    /// Method to spawn the portal effect, re-adjust its
    /// position, scale and colors
    /// </summary>
    public void Spawn()
    {
        // instantiate the effect object
        obj_portalEffect = MonoBehaviour.Instantiate(pref_portalEffect, Blackboard.spawnHolder);

        // adjust the position and scale
        obj_portalEffect.transform.position = portalPosition;
        obj_portalEffect.transform.localScale = portalLossyScale;

        // reset color for the portal effect
        obj_portalEffect.transform.GetChild(0).GetComponent<Renderer>().material.color = innerColor;
        obj_portalEffect.transform.GetChild(1).GetComponent<Renderer>().material.color = outerColor;
    }

    /// <summary>
    /// Method to check whether or not the given position is less
    /// than or equal to the trigger distance
    /// </summary>
    /// <param name="playerPos">the player's position</param>
    /// <returns></returns>
    public bool IsPlayerInRange(Vector3 playerPos)
    {
        return Vector3.Distance(playerPos, obj_portalEffect.transform.position) <= triggerDistance;
    }
}