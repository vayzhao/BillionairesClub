using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seat : MonoBehaviour
{
    [Tooltip("When this option is on, the sitting character of this " +
        "seat will play sit animation with hands on the table")]
    public bool hasArmrest = false;

    [Tooltip("When this option is on, NPC charcter will not sit on this seat")]
    public bool availableGuarantee = false;

    private SeatManager manager;   // the seat manager

    /// <summary>
    /// A method to bind the seat manager to this script
    /// </summary>
    /// <param name="manager">the seat manager</param>
    public void Bind(SeatManager manager)
    {
        this.manager = manager;
    }

    /// <summary>
    /// Method for the user to sit on the seat
    /// </summary>
    /// <param name="animator">animator of the user</param>
    /// <param name="model">model of the user</param>
    public void SitDown(Animator animator, Transform model)
    {
        // remove this seat from available seat list
        manager.availableSeats.Remove(this);

        // set character animation
        animator.SetInteger("Sit", hasArmrest ? 2 : 1);

        // stick character model to this seat
        model.position = transform.position;
        model.eulerAngles = transform.eulerAngles;
    }

    /// <summary>
    /// Model to set the seat to be availble again
    /// </summary>
    /// <param name="animator">animator of the user</param>
    public void StandUp(Animator animator)
    {
        // add this seat from available seat list
        manager.availableSeats.Add(this);

        // reset character animation
        animator.SetInteger("Sit", 0);
    }
}
