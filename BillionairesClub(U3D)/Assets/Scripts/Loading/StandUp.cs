using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StandUp : MonoBehaviour
{
    private void Start()
    {
        RegisterButton();
    }

    /// <summary>
    /// Method to register click event method for the button
    /// </summary>
    public void RegisterButton()
    {
        // find the button component from this object
        var btn = this.gameObject.GetComponent<Button>();

        // and then add leave method to the button's event
        btn.onClick.AddListener(() => Leave());
    }

    /// <summary>
    /// Method to leave the game scene and go back to casino scene
    /// </summary>
    private void Leave()
    {
        // find the uiManager & and loading script
        var loading = FindObjectOfType<Loading>();
        var uiManager = FindObjectOfType<UIManager>();

        // disable all intercatable ui objects
        uiManager.SetInitButtonVisbility(false);
        uiManager.SetInteractableVisibility(false);

        // record player's current data (e.g. poker chip & gem)
        StorePlayerData();

        // stop game loop couroutine
        switch (loading.gameType)
        {
            case GameType.None:
                break;
            case GameType.TexasBonus:
                FindObjectOfType<TexasBonus.GameManager>().StopAllCoroutines();
                break;
            case GameType.Blackjack:
                break;
            default:
                break;
        }

        // start loading back to the casino
        loading.LoadBackToCasino();
    }

    /// <summary>
    /// Method to store local player's data before leaving the table
    /// </summary>
    private void StorePlayerData()
    {
        Storage.SaveInt(Const.LOCAL_PLAYER, StorageType.Chip, Blackboard.localPlayer.chip);
        Storage.SaveInt(Const.LOCAL_PLAYER, StorageType.Gem, Blackboard.localPlayer.gem);
        Storage.SaveString(Const.LOCAL_PLAYER, StorageType.Description, Blackboard.localPlayer.description);
    }
}