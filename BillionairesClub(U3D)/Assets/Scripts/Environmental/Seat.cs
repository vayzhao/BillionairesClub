using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seat : MonoBehaviour
{
    [Tooltip("When this option is on, NPC charcter will not sit on this seat")]
    public SeatAvailability seatAvailablity;

    [Tooltip("When this option is on, the sitting character of this " +
        "seat will play sit animation with hands on the table")]
    public bool hasArmrest = false;

    [Tooltip("Who's sitting on this seat")]
    public Player sittingPlayer;   

    private SeatManager manager;    // the seat manager
    private TableInformation table; // which table does this seat belong to
    
    /// <summary>
    /// A method to bind the seat manager & table to this script
    /// </summary>
    /// <param name="manager">the seat manager</param>
    /// <param name="table">which table this seat belongs to</param>
    public void Bind(SeatManager manager)
    {
        this.manager = manager;
    }
    public void Bind(TableInformation table)
    {
        this.table = table;
    }
    
    /// <summary>
    /// Method to access the table information from this seat
    /// </summary>
    /// <returns></returns>
    public TableInformation GetTable() { return this.table; }

    /// <summary>
    /// Method for the user to sit on the seat
    /// </summary>
    /// <param name="animator">animator of the user</param>
    /// <param name="model">model of the user</param>
    public void SitDown(Player player, Animator animator, Transform model)
    {
        // remove this seat from available seat list
        switch (seatAvailablity)
        {
            case SeatAvailability.All:
                manager.availableSeats.Remove(this);
                break;
            case SeatAvailability.UserOnly:
                manager.userSeats.Remove(this);
                break;
            case SeatAvailability.DealerOnly:
                manager.dealerSeats.Remove(this);
                break;
            default:
                break;
        }

        // set character animation
        animator.SetInteger("Sit", hasArmrest ? 2 : 1);

        // stick character model to this seat
        model.position = transform.position;
        model.eulerAngles = transform.eulerAngles;

        // bind sitting player
        sittingPlayer = player;
    }

    /// <summary>
    /// Model to set the seat to be availble again
    /// </summary>
    /// <param name="animator">animator of the user</param>
    public void StandUp(Animator animator)
    {
        // add this seat back to its original seat list
        switch (seatAvailablity)
        {
            case SeatAvailability.All:
                manager.availableSeats.Add(this);
                break;
            case SeatAvailability.UserOnly:
                manager.userSeats.Add(this);
                break;
            case SeatAvailability.DealerOnly:
                manager.dealerSeats.Add(this);
                break;
            default:
                break;
        }

        // reset character animation
        animator.SetInteger("Sit", 0);

        // reset sitting player
        sittingPlayer = null;
    }
    
    /// <summary>
    /// Method to return sitting player
    /// </summary>
    /// <returns></returns>
    public Player GetPlayer() { return this.sittingPlayer; }
}
