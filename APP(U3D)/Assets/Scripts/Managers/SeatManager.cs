using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeatManager : MonoBehaviour
{
    [HideInInspector]
    public List<Seat> allSeats;
    //[HideInInspector]
    public List<Seat> availableSeats;

    /// <summary>
    /// Method to initialize the seat manager
    /// </summary>
    public void Setup()
    {
        // setup lists for managing seat
        allSeats = new List<Seat>();
        availableSeats = new List<Seat>();

        // find all seats object in the scene
        foreach (var seat in FindObjectsOfType<Seat>())
        {
            // add it to allSeat and available seats
            allSeats.Add(seat);
            availableSeats.Add(seat);

            // bind the manager to the seat script
            seat.Bind(this);
        }
    }

    /// <summary>
    /// Method to return a random seat from available seat list
    /// </summary>
    /// <returns></returns>
    public Seat GetRandomAvailableSeat() { return availableSeats[Random.Range(0, availableSeats.Count)]; }
}