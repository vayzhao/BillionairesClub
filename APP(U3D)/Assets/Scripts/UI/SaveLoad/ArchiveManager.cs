using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArchiveManager : MonoBehaviour
{
    public Sprite normalSprite;
    public Sprite selectedSprite;

    public Archive[] archives;

    public Button deleteButton;
    public Button saveButton;
    public Button loadButton;


    private int selectIndex;

    private void Start()
    {
        ResetSelection();
    }



    private void ResetSelectionSingle(int selectId)
    {
        archives[selectId].img.sprite = normalSprite;
    }

    public void ResetSelection()
    {
        selectIndex = -1;

        deleteButton.Switch(false);
        saveButton.Switch(false);
        loadButton.Switch(false);

        for (int i = 0; i < archives.Length; i++)
            ResetSelectionSingle(i);
    }

    public void SelectAnArchive(int selectIndex)
    {
        // deselect the previous selection if it exists
        if (this.selectIndex != -1)
            ResetSelectionSingle(this.selectIndex);

        // update selection index
        this.selectIndex = selectIndex;

        // change sprite for the selected archive
        archives[selectIndex].img.sprite = selectedSprite;


    }
}