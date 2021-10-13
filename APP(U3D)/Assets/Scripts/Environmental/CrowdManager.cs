using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowdManager : MonoBehaviour
{
    [Header("Sitting Crowd")]
    [Tooltip("Number of npc character that have a seat")]
    [Range(20, 40)]
    public int npcWithSeatQty = 20;
    [Tooltip("The prefab of npc character")]
    public GameObject npcPrefab;

    private SeatManager seatManager;       // a manager that handles all the seats in the scene
    private List<int> npcAvatarIndex;      // a list of non-duplicate integer

    /// <summary>
    /// Method to initialize the crowd manager
    /// </summary>
    public void Setup()
    {
        // initialize npcAvararIndex
        npcAvatarIndex = new List<int>();

        // find the seat manager component
        seatManager = GetComponent<SeatManager>();

        // create npcs that have seat
        InitializeNPCOnSeat();
    }

    /// <summary>
    /// Method to create npcs that have seat
    /// </summary>
    void InitializeNPCOnSeat()
    {
        // create a tempolary list
        List<Seat> usedSeat = new List<Seat>();

        // add all dealer seat into the list
        for (int i = 0; i < seatManager.dealerSeats.Count; i++)
            usedSeat.Add(seatManager.dealerSeats[i]);

        // add npc seat into the list
        var seatIndex = new List<int>();
        for (int i = 0; i < npcWithSeatQty; i++)
        {
            var index = seatIndex.GetNonDuplicateInt(0, seatManager.availableSeats.Count);
            usedSeat.Add(seatManager.availableSeats[index]);
        }

        // spawn npc characters onto the usedSeat
        for (int i = 0; i < usedSeat.Count; i++)
        {
            // spawn the npc object
            var npc = Instantiate(npcPrefab, Blackboard.spawnHolder);
            var script = npc.GetComponent<NPCController>();

            // get a random avatar index
            var index = npcAvatarIndex.GetNonDuplicateInt(0, 36);

            // setup the npc model and put it on the seat
            npc.name = $"NPC[{Blackboard.modelName[index / 3]}]";
            script.Setup(index);
            script.IssueSitDownOrder(usedSeat[i]);
        }
    }
}