﻿using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StandalonePlayerMoveController : MonoBehaviourPun
{
    private float walkSpeed = 3f;
    private float runSpeed = 8f;
    public float turnSpeed = 1.5f;
    public float gravity = 3f;

    private float angle;
    private float currentVelocity;
    private float speedSmoothVelocity = 0f;
    private float speedSmoothTime = 0.1f;
    
    public Vector2 input;    
    private Quaternion targetRotation;

    private GameObject s_Rig;
    private Camera cam;

    private CharacterController characterController;
    private AnimationController animController;    

    private void Start()
    {
        animController = MasterManager.ClassReference.AnimController;
        s_Rig = GameObject.Find("Standalone_Rig");

        if(s_Rig != null)
            cam = s_Rig.GetComponentInChildren<Camera>();

        characterController = GetComponent<CharacterController>();        
    }

    private void Update()
    {
        if (!photonView.IsMine)
            return;

        GetInput();
        CalculateDirection();
        Rotate();
        Move();
    }

    public void GetInput()
    {
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");
    }

    public void CalculateDirection()
    {
        angle = Mathf.Atan2(input.x, input.y);
        angle = Mathf.Rad2Deg * angle;
        angle += cam.transform.eulerAngles.y;
    }

    public void Rotate()
    {
        targetRotation = Quaternion.Euler(0, angle, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
    }

    public void Move()
    {            
        Vector3 forward = cam.transform.forward;
        Vector3 right = cam.transform.right;             

        forward.Normalize();
        right.Normalize();

        Vector3 desiredMoveDirection = (forward * input.y + right * input.x).normalized;
        Vector3 gravityVector = Vector3.zero;

        if (!characterController.isGrounded)
            gravityVector.y -= gravity;

        float targetSpeed = walkSpeed * input.magnitude;

        if (Input.GetKey(KeyCode.LeftShift))
            targetSpeed = runSpeed * input.magnitude;

        currentVelocity = Mathf.SmoothDamp(currentVelocity, targetSpeed, ref speedSmoothVelocity, speedSmoothTime);        
        characterController.Move(desiredMoveDirection * currentVelocity * Time.deltaTime);
        characterController.Move(gravityVector * Time.deltaTime);
       
        if (Input.GetKey(KeyCode.LeftShift))
            animController.SetAvatarFloatAnimation("MovementSpeed", 1f * input.magnitude, speedSmoothTime);                                            
        else if (Input.GetKey(KeyCode.Space))
            animController.SetAvatarFloatAnimation("MovementSpeed", -0.5f, speedSmoothTime);        
        else
            animController.SetAvatarFloatAnimation("MovementSpeed", 0.5f * input.magnitude, speedSmoothTime);    
                                   
    }
}
