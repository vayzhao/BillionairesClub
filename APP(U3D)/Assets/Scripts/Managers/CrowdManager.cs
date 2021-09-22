using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowdManager : MonoBehaviour
{
    [Header("Sitting Crowd")]
    [Tooltip("Number of npc character that have a seat")]
    [Range(25, 50)]
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
        // find the seat manager component
        seatManager = GetComponent<SeatManager>();

        // map a list of non-duplicate integer
        ResetAvatarIndex();

        // create npcs that have seat
        InitializeNPCOnSeat();
    }

    /// <summary>
    /// Method to re-map a list of non-duplicate integer
    /// It's called when all the values have been used
    /// </summary>
    void ResetAvatarIndex()
    {
        npcAvatarIndex = new List<int>();
        for (int i = 0; i < 36; i++)
            npcAvatarIndex.Add(i);
    }

    /// <summary>
    /// Method to randomly select an integer from a list of
    /// integer, reset the list when all values have been used
    /// </summary>
    /// <returns></returns>
    int PopRandomIndex()
    {
        // randomly select & remove an element from the list
        var index = npcAvatarIndex[Random.Range(0, npcAvatarIndex.Count)];
        npcAvatarIndex.Remove(index);

        // when all the elemented are used, reset the list
        if (npcAvatarIndex.Count == 0)
            ResetAvatarIndex();

        return index;
    }

    /// <summary>
    /// Method to create npcs that have seat
    /// </summary>
    void InitializeNPCOnSeat()
    {
        // keep spawning npc characters till created reaches the spawn amount
        var created = 0;
        while (created < npcWithSeatQty)
        {
            // select a random available seat, only spawn when the 
            // seat is not "available guarantee"
            var seat = seatManager.GetRandomAvailableSeat();
            if (!seat.availableGuarantee)
            {
                // spawn the npc object                
                var npc = Instantiate(npcPrefab);
                var script = npc.GetComponent<NPCController>();

                // get the random avatar index
                var avatarIndex = PopRandomIndex();           
                
                // set up the npc model and designate it to sit a seat
                npc.name = $"NPC[{BB.modelName[avatarIndex / 3]}]";
                script.Setup(avatarIndex);
                script.IssueSitDownOrder(seat);

                // increment created
                created++;
            }
        }
    }
}