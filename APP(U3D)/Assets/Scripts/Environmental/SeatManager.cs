using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeatManager : MonoBehaviour
{
    [HideInInspector]
    public List<Seat> allSeats;
    [HideInInspector]
    public List<Seat> availableSeats;
    [HideInInspector]
    public List<Seat> dealerSeats;
    [HideInInspector]
    public List<Seat> userSeats;

    /// <summary>
    /// Method to initialize the seat manager
    /// </summary>
    public void Setup()
    {
        // setup lists for managing seat
        allSeats = new List<Seat>();
        availableSeats = new List<Seat>();
        dealerSeats = new List<Seat>();
        userSeats = new List<Seat>();

        // find all seats object in the scene
        foreach (var seat in FindObjectsOfType<Seat>())
        {
            // add it to allSeat and available seats
            allSeats.Add(seat);
            
            switch (seat.seatAvailablity)
            {
                case SeatAvailability.All:
                    availableSeats.Add(seat);
                    break;
                case SeatAvailability.UserOnly:
                    userSeats.Add(seat);
                    break;
                case SeatAvailability.DealerOnly:
                    dealerSeats.Add(seat);
                    break;
                default:
                    break;
            }

            // bind this seat to seat manager
            seat.Bind(this);
        }
    }
}