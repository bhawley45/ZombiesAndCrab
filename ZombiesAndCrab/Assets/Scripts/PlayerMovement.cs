using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private int moveSpeed = 125;
    
    // Note: Using the explicit approach for player input actions (instead of sending messages[implicit])

    private PlayerInputs inputActions;
    private Vector2 movement;
    private Rigidbody2D rb;
    private Animator animator;

    private bool canDash = true;
    private bool isDashing = false;

    [SerializeField] private float dashingPower = 30f;
    [SerializeField] private float dashingTime = 0.25f;
    [SerializeField] private float dashingCooldown = 0.75f;

    [SerializeField] private TrailRenderer tr;


    private void Awake()
    {
        inputActions = new PlayerInputs();

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        inputActions.Player.Movement.performed += Move;
        inputActions.Player.Movement.canceled += Move;
        inputActions.Player.Dash.performed += Dash;

        inputActions.Player.Enable();
    }

    private void OnDisable()
    {
        inputActions.Player.Movement.performed -= Move;
        inputActions.Player.Movement.canceled -= Move;
        inputActions.Player.Dash.performed -= Dash;

        inputActions.Player.Disable();
    }

    private void Move(InputAction.CallbackContext context)
    {
        movement = context.ReadValue<Vector2>(); // Get Vector2 from call context

        animator.SetFloat("xAxis", movement.x);
        animator.SetFloat("yAxis", movement.y);

        // if x or y is non-zero => set isWalking
        animator.SetBool("isWalking", movement != Vector2.zero); 
    }

    private void Dash(InputAction.CallbackContext context)
    {
        if(canDash && !isDashing)
        {
            isDashing = true;
            StartCoroutine(DashTimer());
        }
    }


    // Framerate Independent
    private void FixedUpdate()
    {
        if (isDashing) return; // Prevent movement while dashing

        // Variant 1
        //rb.MovePosition(rb.position + movement * speed * Time.fixedDeltaTime);

        //Variant 2
        //if(movement.x !=0 || movement.y !=0)
        //{
        //    rb.velocity = movement * speed;
        //}

        // Variant 3  
        rb.AddForce (movement * moveSpeed);
    }
    
    IEnumerator DashTimer()
    {
        canDash = false;

        //rb.velocity = movement * moveSpeed * dashingPower * Time.fixedDeltaTime;
        rb.AddForce(movement * moveSpeed * dashingPower);
        tr.emitting = true;

        yield return new WaitForSeconds(dashingTime);

        tr.emitting = false;
        isDashing = false;

        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }
}
