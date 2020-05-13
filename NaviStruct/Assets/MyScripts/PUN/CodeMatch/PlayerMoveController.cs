using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveController : MonoBehaviour
{
    public float walkSpeed = 2;
    public float runSpeed = 5;
    public float turnSpeed = 1.5f;
    public float gravity = 3f;

    private float angle;
    private float currentVelocity;
    private float speedSmoothVelocity = 0f;
    private float speedSmoothTime = 0.1f;

    Vector3 lastPos;
    private Vector2 input;    
    private Quaternion targetRotation;

    private Transform cam;
    private CharacterController characterController;

    private void Start()
    {
        cam = CameraController.instance.gameObject.transform;
        characterController = GetComponent<CharacterController>();
        AnimationController.instance.SetAvatarAnimationIdle();
    }

    private void Update()
    {
        GetInput();

        if(Mathf.Abs(input.x) < 1 && Mathf.Abs(input.y) < 1)
            return;

        CalculateDirection();
        Rotate();
        Move();
    }

    private void GetInput()
    {
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");
    }

    private void CalculateDirection()
    {
        angle = Mathf.Atan2(input.x, input.y);
        angle = Mathf.Rad2Deg * angle;
        angle += cam.eulerAngles.y;
    }

    private void Rotate()
    {
        targetRotation = Quaternion.Euler(0, angle, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
    }

    private void Move()
    {
        Vector2 movementInput = new Vector2(input.x, input.y);
        Vector3 gravityVector = Vector3.zero;
        float targetSpeed = walkSpeed * input.magnitude;        

        if (!characterController.isGrounded)
            gravityVector.y -= gravity;

        currentVelocity = Mathf.SmoothDamp(currentVelocity, targetSpeed, ref speedSmoothVelocity, speedSmoothTime);

        //characterController.Move(transform.forward * velocity * Time.deltaTime);
        characterController.Move(transform.forward * currentVelocity * Time.deltaTime);
        characterController.Move(gravityVector * Time.deltaTime);

        AnimationController.instance.SetAvatarFloatAnimation("MovementSpeed", 0.5f * movementInput.magnitude);

        //if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0) 
        //{            
        //    if (Input.GetButton("Fire3"))
        //    {
        //        walkSpeed = 6;
        //        ApplyMovementAnimation(true, "isRunning");
        //    }
        //    else
        //    {
        //        walkSpeed = 3;
        //        ApplyMovementAnimation(true, "isWalking");
        //    }
        //}
        //else
        //{
        //    ApplyMovementAnimation(false, "isWalking");
        //}
    }

    private void ApplyMovementAnimation(bool isMoving, string animName)
    {
        if (isMoving)
            AnimationController.instance.SetAvatarAnimation(animName);
        else
            AnimationController.instance.SetAvatarAnimationIdle();
    }
}
