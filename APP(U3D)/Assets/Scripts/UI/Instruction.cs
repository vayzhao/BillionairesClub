using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class Instruction : MonoBehaviour
{
    [Header("UI Components")]
    [Tooltip("An object that holds the previous page button")]
    public GameObject btn_prev;
    [Tooltip("An object that holds the next page button")]
    public GameObject btn_next;
    [Tooltip("Gameobject that holds all pages")]
    public GameObject[] pages;

    [Header("Settings")]
    [Tooltip("Number of the pages")]
    public int maxPage;

    private int currentPage; // index of current page

    void Start()
    {
        Reset();
    }

    private void Reset()
    {
        // reset current page 
        currentPage = 0;
        pages[currentPage].SetActive(true);

        // hide all the pages except the first page
        for (int i = 1; i < pages.Length; i++)
            pages[i].SetActive(false);

        // reset button states
        ResetButtonState();
    }

    /// <summary>
    /// Method to view previous page
    /// </summary>
    public void Prev()
    {
        pages[currentPage].SetActive(false);
        currentPage--;
        pages[currentPage].SetActive(true);

        ResetButtonState();
    }

    /// <summary>
    /// Method to view next page
    /// </summary>
    public void Next()
    {
        pages[currentPage].SetActive(false);
        currentPage++;
        pages[currentPage].SetActive(true);

        ResetButtonState();
    }
    
    /// <summary>
    /// Method to close the instruction panel
    /// </summary>
    public void Close()
    {
        Reset();
        GetComponentInParent<UIManager>().ClosePage();
    }

    /// <summary>
    /// Method to reset button's state
    /// </summary>
    private void ResetButtonState()
    {
        btn_prev.SetActive(currentPage == 0 ? false : true);
        btn_next.SetActive(currentPage == (maxPage - 1) ? false : true);
    }

}