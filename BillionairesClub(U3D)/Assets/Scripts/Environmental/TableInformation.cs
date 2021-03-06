using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TableInformation : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Game type of this table")]
    public GameType gameType;
    [Tooltip("Prefab object of the spawning npcs")]
    public GameObject pref_npc;

    [Header("Seat")]
    [Tooltip("Seat that is used by the dealer")]
    public Seat dealerSeat;
    [Tooltip("Seats that are used by the players")]
    public Seat[] seats;
    
    [HideInInspector]
    public SeatManager seatManager; // the seat manager script
    [HideInInspector]
    public Player[] players;        // data for all players in this table

    /// <summary>
    /// A method to bind table information to all the related seats
    /// </summary>
    public void Setup()
    {
        foreach (var seat in seats)
            seat.Bind(this);
    }
    
    /// <summary>
    /// A method to save all player information into the seats, it is called before
    /// loading into the table game scene. The usage of this is to pass players data
    /// from casino scene into the specific game scene 
    /// </summary>
    public void SaveSeatsInfo()
    {
        // get the dealer information
        var dealerModelndex = dealerSeat.GetPlayer().modelIndex;
        Storage.SaveInt(Const.DEALER_PLAYER, StorageType.ModelIndex, dealerModelndex);

        // check other player's index
        var playerIndexs = new int[seats.Length];
        for (int i = 0; i < playerIndexs.Length; i++)
        {
            // check to see if this seat has anyone sits on
            var player = seats[i].GetPlayer();
            if (player != null)
            {
                // if this seat has player on it, save player's info
                Storage.SaveString(i, StorageType.Name, player.name);
                Storage.SaveString(i, StorageType.Description, player.description);
                Storage.SaveInt(i, StorageType.Chip, player.chip);
                Storage.SaveInt(i, StorageType.Gem, player.gem);
                Storage.SaveInt(i, StorageType.ModelIndex, player.modelIndex);
                Storage.SaveInt(i, StorageType.PlayerState, player.isNPC ? 2 : 1);
            }
            else
            {
                // otherwise, set player i state to be 0
                Storage.SaveInt(i, StorageType.PlayerState, 0);
            }
        }
    }

    /// <summary>
    /// A method to load all player information that are stored in the seats, it is
    /// called after the table-game scene is loaded
    /// </summary>
    public void LoadSeatsInfo()
    {
        // get the dealer information
        var dealerModelIndex = Storage.LoadInt(Const.DEALER_PLAYER, StorageType.ModelIndex);
        CreateCharacter(dealerSeat, dealerModelIndex);

        // create an array to hold all the players
        players = new Player[seats.Length];
        for (int i = 0; i < players.Length; i++)
        {
            // get the specific player state
            // 0:empty
            // 1:player
            // 2:npc-player
            var playerState = Storage.LoadInt(i, StorageType.PlayerState);

            // check to see if this seat has player on it
            // if it does, spawn a model to repersent the player
            if (playerState != 0)
            {
                players[i] = CreateCharacter(seats[i], Storage.LoadInt(i, StorageType.ModelIndex));
                players[i].isNPC = playerState == 2 ? true : false;
                players[i].name = Storage.LoadString(i, StorageType.Name);
                players[i].description = Storage.LoadString(i, StorageType.Description);
                players[i].gem = Storage.LoadInt(i, StorageType.Gem);
                players[i].chip = Storage.LoadInt(i, StorageType.Chip);

                // bind the user player to blackboard
                if (playerState == 1)
                    Blackboard.localPlayer = players[i];                
            }
        }
    }

    /// <summary>
    /// A method to create character that sit on a specific seat
    /// </summary>
    /// <param name="whichSeat">the specific seat</param>
    /// <param name="modelIndex">model index for the character</param>
    Player CreateCharacter(Seat whichSeat, int modelIndex)
    {
        // spawn the npc object and obtain its script
        var npc = Instantiate(pref_npc, Blackboard.spawnHolder);
        var script = npc.GetComponent<NPCController>();

        // rename the npc object
        npc.name = $"NPC[{Blackboard.modelName[modelIndex / 3]}]";

        // setup the npc model and play sitting animation
        script.Setup(modelIndex);
        script.IssueSitDownOrderForTableGame(whichSeat);

        return script;
    }
}