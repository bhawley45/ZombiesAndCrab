using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private int speed = 115;
    
    private Vector2 movement;
    private Rigidbody2D rb;
    private Animator animator;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void OnMovement (InputValue value)
    {
        movement = value.Get<Vector2>();

        animator.SetFloat("xAxis", movement.x);
        animator.SetFloat("yAxis", movement.y);

        if(movement.x != 0 || movement.y != 0)
        {
            animator.SetBool("isWalking", true);
        }
        else
        {
            animator.SetBool("isWalking", false);
        }
    }

    // Framerate Independent
    private void FixedUpdate()
    {
        // Variant 1
        //rb.MovePosition(rb.position + movement * speed * Time.fixedDeltaTime);

        //Variant 2
        //if(movement.x !=0 || movement.y !=0)
        //{
        //    rb.velocity = movement * speed;
        //}

        // Variant 3
        rb.AddForce (movement * speed);
    }
}
