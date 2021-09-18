using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float speed = 10f;
    private Animator animator;
    private CharacterController cc;
    

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        cc = GetComponent<CharacterController>();        
    }

    // Update is called once per frame
    void Update()
    {
        Walk();
        Turn();
    }

    void Walk()
    {
        var horizontal = Input.GetAxisRaw("Horizontal");
        var vertical = Input.GetAxisRaw("Vertical");
        var isWalking = horizontal != 0f || vertical != 0f;
        var forward = transform.forward * vertical;
        var side = transform.right * horizontal;
        var nextPos = Vector3.ClampMagnitude(forward + side, 1f);

        cc.SimpleMove(nextPos * speed);
        animator.SetBool("IsWalking", isWalking);

    }

    void Turn()
    {
        var mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * 400f;
        var mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * 400f;
        transform.Rotate(Vector3.up * mouseX);
        Cursor.visible = false;
    }

}
