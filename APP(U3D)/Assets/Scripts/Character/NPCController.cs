using UnityEngine;

public class NPCController : Player
{
    /// <summary>
    /// Core components
    /// </summary>
    private Transform model;     // the model to visualize the npc character
    private Animator animator;   // a component that handles all the animation staff
    private CapsuleCollider col; // capsule collider from the character

    /// <summary>
    /// Dependencies of Sitting Function
    /// </summary>
    private Vector3 originPos;   // origin position before sit
    private Vector3 originRot;   // origin rotation before sit

    /// <summary>
    /// Method to initialize a npc character
    /// </summary>
    /// <param name="modelIndex"></param>
    public void Setup(int modelIndex)
    {
        // create model for the npc
        model = transform.GetChild(0);
        Instantiate(Blackboard.GetModelPrefab(modelIndex), transform.GetChild(0));

        // save model index
        this.modelIndex = modelIndex;

        // get animator component
        animator = GetComponentInChildren<Animator>();

        // get collider component
        col = GetComponent<CapsuleCollider>();
    }

    /// <summary>
    /// Method to designate a npc character to sit on a seat
    /// </summary>
    /// <param name="seat"></param>
    public void IssueSitDownOrder(Seat seat)
    {
        // record original position & rotation before sitting
        originPos = model.position;
        originRot = model.eulerAngles;

        // disable the collider
        col.enabled = false;

        // finally sit donw
        seat.SitDown(this, animator, model);
    }

    /// <summary>
    /// Method to let the npc character to leave the seat
    /// </summary>
    /// <param name="seat"></param>
    public void IssueStandUpOrder(Seat seat)
    {
        // move model back to the origin position & rotation
        model.position = originPos;
        model.eulerAngles = originRot;

        // enable the collider
        col.enabled = true;

        // finally stand up 
        seat.StandUp(animator);        
    }

    /// <summary>
    /// Method to designate a npc character to sit on a seat
    /// without modifying any data on that seat
    /// </summary>
    /// <param name="seat"></param>
    public void IssueSitDownOrderForTableGame(Seat seat)
    {
        // disable the collider
        col.enabled = false;

        // set character animation
        animator.SetInteger("Sit", seat.hasArmrest ? 2 : 1);

        // stick character model to the seat
        model.transform.parent = seat.transform;
        model.transform.localPosition = Vector3.zero;
        model.transform.localEulerAngles = Vector3.zero;
    }
}