﻿using TMPro;
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterSelection : MonoBehaviour
{
    [Header("Widgets")]
    [Tooltip("A gameobject that holds everything about character selection.(include 2D & 3D")]
    public GameObject content;
    [Tooltip("A transform that holds the displayed model")]
    public Transform modelHolder;
    [Tooltip("A tag that shows the name of the model")]
    public TextMeshProUGUI nameTag;
    [Tooltip("A gameobject that holds all the buttons")]
    public GameObject[] buttons;

    [Header("Prefab Data")]
    [Tooltip("Names of all models")]
    public string[] names;
    [Tooltip("Prefab objects of all models")]
    public GameObject[] prefabs;

    private int modelIndex;      // the index of current model
    private int styleIndex;      // the style of current model
    private int prefabIndex;     // accurate index of the model style
    private Transform model;     // model displayed in the window
    private bool hasSelected;    // determine whether or not the user has selected a character
    private float rotationAngle; // Y value of model's current eulerAngles

    // Update is called once per frame
    private void Update() { RotateModel(); }

    /// <summary>
    /// Method to display / hide character selection interface
    /// </summary>
    /// <param name="value">true to display, false to hide</param>
    public void SetActive(bool value)
    {
        if (value)
        {
            // when showing, reset rotation angle and indexs
            rotationAngle = 180f;
            modelIndex = 0;
            styleIndex = 0;

            // create a clone model from the first prefab
            model = Instantiate(prefabs[0], modelHolder).transform;
            model.transform.eulerAngles = new Vector3(0f, rotationAngle, 0f);

            // reset name tag
            nameTag.text = names[0];            
        }
        else
        {
            // when hidding, destroy the model
            Destroy(model.gameObject);            
        }

        // display / hide the content object
        content.SetActive(value);
    }

    /// <summary>
    /// Method to swap model prefab based on the prefab index
    /// </summary>
    void SwapModel()
    {
        // destroy the current model
        Destroy(model.gameObject);

        // get prefab index and swap model
        prefabIndex = (modelIndex * 3) + styleIndex;
        model = Instantiate(prefabs[prefabIndex], modelHolder).transform;

        // synchronize position & rotation seamlessly
        RotateModel();
    }

    /// <summary>
    /// Method to rotate the model in every frame
    /// </summary>
    void RotateModel()
    {
        // return if the character selection window is hidden
        if (!content.activeSelf || hasSelected)
            return;

        // lock position and rotate
        rotationAngle -= 50f * Time.deltaTime;
        //model.transform.localPosition = Vector3.zero;
        modelHolder.transform.eulerAngles = new Vector3(0f, rotationAngle, 0f);

    }

    /// <summary>
    /// Method to edit model index
    /// </summary>
    /// <param name="change">increment or decrement</param>
    public void EditModelIndex(int change)
    {
        // update model index
        styleIndex = 0;
        modelIndex += change;

        // fix the index when it is smaller than the minimum
        // or greater than the maximum
        if (modelIndex < 0)
            modelIndex = 11;
        else if (modelIndex > 11)
            modelIndex = 0;

        // swap model prefab and update name tag
        SwapModel();
        nameTag.text = names[modelIndex];
    }

    /// <summary>
    /// Method to edit style index
    /// </summary>
    /// <param name="newIndex">the new index</param>
    public void EditStyleIndex(int newIndex)
    {
        styleIndex = newIndex;
        SwapModel();
    }

    /// <summary>
    /// Method for the user to final select the character
    /// Disable all other irrelevant buttons and lock rotation
    /// </summary>
    public void Ready()
    {
        // switch hasSelected boolean to be true
        hasSelected = true;

        // disable irrelevant buttons
        foreach (var obj in buttons)
            obj.SetActive(false);

        // play push-up animation
        model.gameObject.GetComponent<Animator>().SetTrigger("Selected");

        // lock rotation angle 
        StartCoroutine(LockRotation());
    }

    /// <summary>
    /// A coroutine to lock rotation angle and force it to face the camera
    /// </summary>
    /// <returns></returns>
    IEnumerator LockRotation()
    {
        // obtain current y euler angle
        var yEuler = modelHolder.localEulerAngles.y;

        // keep spining y euler angle back to 0
        while (yEuler != 315f)
        {
            yEuler = Mathf.MoveTowardsAngle(yEuler, 315f, 5f);    
            modelHolder.transform.localEulerAngles = Vector3.up * yEuler;
            yield return new WaitForSeconds(Time.deltaTime);
        }

        // start to load in 2 seconds
        yield return new WaitForSeconds(2f);
        FindObjectOfType<Loading>().Setup(model);
    }
}