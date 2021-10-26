using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CController : Player
{
    [Header("Movement Setting")]
    [Tooltip("Speed for walking forward")]
    public float moveSpeed = 3f;
    [Tooltip("Speed for walking backward")]
    public float moveSpeedBackward = 1.5f;
    [Tooltip("Speed for sprinting forward")]
    public float sprintSpeed = 5f;

    /// <summary>
    /// Core components
    /// </summary>
    private Seat seat;                      // a seat which the character will sit / is sitting on
    private Transform model;                // the model to visualize the player character
    private Animator animator;              // a component that handles all the animation staff
    private SeatManager seatManager;        // a manager that handle all the seats in the scene
    private CharacterController controller; // a controller that handles all the movement function
    private bool isFrozen;                  // determine whether or not the user has control of the character

    /// <summary>
    /// Dependencies of Sitting Function
    /// </summary>
    private bool isSiting;         // a boolean determines whether the character is sitting
    private bool hasAvailableSeat; // a boolean determines whether a seat is found
    private float seatScanTimer;   // a timer determines when to scan for seats
    private Vector3 oldModelPos;   // model position before sit
    private Vector3 oldModelRot;   // model rotation before sit
    private Vector3 oldParentPos;  // parent position before sit
    private Vector3 oldParentRot;  // parent rotation before sit

    private const float DISTANCE_SIT = 2f;      // effective distance for sitting command
    private const float RATE_SIT_DETECT = 0.2f; // how often the script scans seats around the character

    // Start is called before the first frame update
    void Start()
    {
        // find the character model of this object
        model = transform.GetChild(0);

        // find animator component from this object
        animator = GetComponentInChildren<Animator>();

        // find character controller component from this object
        controller = GetComponent<CharacterController>();

        // find seat manager script in the scene
        seatManager = FindObjectOfType<SeatManager>();
    }

    // Update is called once per frame
    void Update()
    {
        // detect camera rotation 
        Turn();

        // return if the character is fronzen
        if (isFrozen)
            return;

        // if the character is sitting, detect stand up function and terminate 
        if (isSiting)
        {
            StandUp();
            return;
        }

        // otherwise, walk, find seats and detect sit down function
        Walk();
        FindSeat();
        SitDown();
    }

    /// <summary>
    /// A method to rotate the character based on the mouse axis input
    /// </summary>
    void Turn()
    {
        var mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * 400f;
        transform.Rotate(Vector3.up * mouseX);
    }

    /// <summary>
    /// A method to allow the user to move the character based on the input axis
    /// </summary>
    void Walk()
    {
        // obtain player's input
        var vertical = Input.GetAxisRaw("Vertical");
        var horizontal = Input.GetAxisRaw("Horizontal");

        // detect whether or not the character is moving by checking the input
        // set up a moveState index for animation purpose 
        // 0(idle)
        // 1(walk forward)
        // 2(walk backward)
        // 3(sprint)
        var moveState = 0;
        if (horizontal != 0f || vertical != 0f)
        {
            // if the character is moving, calculate move position based on the given input
            var verticalMove = transform.right * horizontal;
            var horizontalMove = transform.forward * vertical;
            var movePosition = Vector3.ClampMagnitude(horizontalMove + verticalMove, 1f);

            // case0: walking backward
            if (vertical < 0f)
            {
                moveState = 2;
                movePosition *= moveSpeedBackward;
            }
            // case1: moving sprinting forward
            else if (Input.GetKey(KeyCode.LeftShift))
            {
                moveState = 3;
                movePosition *= sprintSpeed;
            }
            // default case: walking forward
            else
            {
                moveState = 1;
                movePosition *= moveSpeed;
            }
            controller.SimpleMove(movePosition);
        }

        // play character move animation
        animator.SetInteger("MoveState", moveState);
    }

    /// <summary>
    /// Method to scan around and find an available seat within a certain distance
    /// </summary>
    void FindSeat()
    {
        // increase the timer and check if it is time to scan for seats
        seatScanTimer += Time.deltaTime;
        if (seatScanTimer > RATE_SIT_DETECT)
        {
            // set hasAvailableSeat to false by default and check if there is any 
            // available seat within a certain distance
            seat = null;
            hasAvailableSeat = false;            
            for (int i = 0; i < seatManager.availableSeats.Count + seatManager.userSeats.Count; i++)
            {
                // find the accordance seat
                seat = i < seatManager.availableSeats.Count ?
                    seatManager.availableSeats[i] :
                    seatManager.userSeats[i - seatManager.availableSeats.Count];

                if (Vector3.Distance(seat.transform.position, transform.position) <= DISTANCE_SIT)
                {
                    // once an available seat is found, switch the boolean to true and return
                    hasAvailableSeat = true;
                    break;
                }
            }

            // reset scan timer
            seatScanTimer = 0f;
        }
    }

    /// <summary>
    /// Method for the player character to sit down
    /// </summary>
    void SitDown()
    {
        // check through all conditions for siting action
        if (Input.GetKeyDown(KeyCode.Space)
            && hasAvailableSeat
            && (seatManager.availableSeats.Contains(seat)
                || seatManager.userSeats.Contains(seat))
            && Vector3.Distance(seat.transform.position, transform.position) <= DISTANCE_SIT) 
        {
            // before actually sit down, store the position and rotation of the character
            Storage.SaveBool(Const.LOCAL_PLAYER, StorageType.HasRecord, true);
            Storage.SaveVector3(Const.LOCAL_PLAYER, StorageType.Position, transform.position);
            Storage.SaveVector3(Const.LOCAL_PLAYER, StorageType.Rotation, transform.eulerAngles);

            // switch sitting state to be true, unparent model and reset move state
            isSiting = true;
            model.parent = null;
            animator.SetInteger("MoveState", 0);

            // record the origin transform data before making any change
            oldModelPos = model.position;
            oldModelRot = model.eulerAngles;
            oldParentPos = transform.position;
            oldParentRot = transform.eulerAngles;

            // disable character controller and move the object onto the seat
            controller.enabled = false;
            transform.position = seat.transform.position;

            // finally sit down
            seat.SitDown(this, animator, model);

            // check to see if this seat has binded to any table
            var table = seat.GetTable();

            // return if the table does not exsit
            if (table == null)
                return;

            // return if the table has no game type
            if (table.gameType == GameType.None)
                return;

            // otherwise, freezee the character
            isFrozen = true;

            // load into game
            FindObjectOfType<Loading>().LoadIntoGame(table);
        }
    }

    /// <summary>
    /// Method for the player character to stand up
    /// </summary>
    void StandUp()
    {
        // return if space key is not pressed
        if (!Input.GetKeyDown(KeyCode.Space))
            return;

        // otherwise, switch sitting state to be false
        isSiting = false;

        // reset every transform data back to the original        
        model.position = oldModelPos;
        model.eulerAngles = oldModelRot;
        transform.position = oldParentPos;
        transform.eulerAngles = oldParentRot;
        model.parent = transform;

        // enable character controller again
        controller.enabled = true;

        // finally stand up
        seat.StandUp(animator);
    }

    /// <summary>
    /// Method to reset model's parent
    /// </summary>
    public void ReBindModel()
    {
        model.parent = this.transform;
        model.localPosition = Vector3.zero;
        model.parent = null;
    }
}
