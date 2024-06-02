using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private int moveSpeed = 125;

    // Note: Input Actions assigned in engine

    [SerializeField]
    private InputActionReference movement, dash, attack, interact, pointerPosition;


    private Vector2 movementInput;
    private Vector2 pointerInput;
    private Rigidbody2D rb;
    private Animator animator;

    // Weapons
    private WeaponParent weaponParent;

    // Dashing
    private bool canDash = true;
    private bool isDashing = false;
    private float dashingPower = 30f;
    private float dashingTime = 0.25f;
    private float dashingCooldown = 0.75f;
    [SerializeField] private TrailRenderer trail;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        weaponParent = GetComponentInChildren<WeaponParent>();
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        movement.action.performed += Move;
        movement.action.canceled += Move;
        dash.action.performed += Dash;
        attack.action.performed += Attack;
    }

    private void OnDisable()
    {
        movement.action.performed -= Move;
        movement.action.canceled -= Move;
        dash.action.performed -= Dash;
        attack.action.performed -= Attack;
    }

    private void Move(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>(); // Get Vector2 from call context

        animator.SetFloat("xAxis", movementInput.x);
        animator.SetFloat("yAxis", movementInput.y);

        // if x or y is non-zero => set isWalking
        animator.SetBool("isWalking", movementInput != Vector2.zero); 
    }

    private void Dash(InputAction.CallbackContext context)
    {
        if(canDash && !isDashing)
        {
            isDashing = true;
            StartCoroutine(DashTimer());
        }
    }

    private void Attack(InputAction.CallbackContext context)
    {
        if(weaponParent == null)
        {
            Debug.LogError("Weapon parent is null ", gameObject);
            return;
        }
        weaponParent.PerformAttack();
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
        rb.AddForce (movementInput * moveSpeed);
    }



    private void Update()
    {
        pointerInput = GetPointerInput();

        weaponParent.PointerPosition = pointerInput;
    }


    private Vector2 GetPointerInput()
    {
        Vector3 mousePosition = pointerPosition.action.ReadValue<Vector2>();
        mousePosition.z = Camera.main.transform.position.z;

        return Camera.main.ScreenToWorldPoint(mousePosition);
    }


    IEnumerator DashTimer()
    {
        canDash = false;

        //rb.velocity = movement * moveSpeed * dashingPower * Time.fixedDeltaTime;
        rb.AddForce(movementInput * moveSpeed * dashingPower);
        trail.emitting = true;

        yield return new WaitForSeconds(dashingTime);

        trail.emitting = false;
        isDashing = false;

        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }
}
