using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerArchive : MonoBehaviour
{
    [Tooltip("Button to go to the next page")]
    public Button nextBtn;  

    private string newName; // a name that the player is going to use

    /// <summary>
    /// Method to call when the inputfiled has been changed
    /// </summary>
    /// <param name="newName"></param>
    public void InputChange(string newName)
    {
        // record the newName
        this.newName = newName;

        // if the character length is 0, disable the 'next' button
        if (newName.Length == 0 && nextBtn.enabled)
        {
            nextBtn.enabled = false;
            nextBtn.GetComponent<Image>().sprite = nextBtn.spriteState.disabledSprite;
        }
        // enable the 'next' button as long as the user has entered anything
        else if (newName.Length > 0 && !nextBtn.enabled)
        {
            nextBtn.enabled = true;
            nextBtn.GetComponent<Image>().sprite = nextBtn.spriteState.pressedSprite;
        }
    }
    
    /// <summary>
    /// Method to set the last edited name as the local player name
    /// </summary>
    public void UpdateLocalPlayerName() => Blackboard.localPlayerName = newName;
}