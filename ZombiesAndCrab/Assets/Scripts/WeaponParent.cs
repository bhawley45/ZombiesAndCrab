using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponParent : MonoBehaviour
{
    [SerializeField]private SpriteRenderer characterRenderer, weaponRenderer;
    [SerializeField]private Animator animator;
    public Vector2 PointerPosition { get; set; }

    public float attackDelay = 0.3f;
    private bool canAttack = true;

    private void Update()
    {
        // Point towards the mouse position
        Vector2 direction = (PointerPosition - (Vector2)transform.position).normalized;
        transform.right = direction;


        // Flip sprite if rotated far enough
        Vector2 scale = transform.localScale;
        if(direction.x < 0)
        {
            scale.y = -1;
        }
        else if (direction.x > 0) 
        { 
            scale.y = 1;
        }
        transform.localScale = scale;

        
        // Render weapon in front or behind player depending on angle
        if(transform.eulerAngles.z > 0 && transform.eulerAngles.z < 180)
        {
            // Place behind Player sprite
            weaponRenderer.sortingOrder = characterRenderer.sortingOrder - 1; 
        }
        else
        {
            // Place weapon in front of Player sprite
            weaponRenderer.sortingOrder = characterRenderer.sortingOrder + 1;
        }
    }


    public void PerformAttack()
    {
        if(!canAttack) return;
        
        animator.SetTrigger("Attack");
        canAttack = false;

        StartCoroutine(DelayAttack());
    }


    private IEnumerator DelayAttack()
    {
        yield return new WaitForSeconds(attackDelay);
        canAttack = true;
    }
}
