using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy : MonoBehaviour
{
    Rigidbody2D body;

    float moveLimiter = 0.7f;

    public float runSpeed = 0.5f;
    public int MinDistance = 1;

    private Transform target;
    private Animator animator;

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        animator  = GetComponent<Animator>();
        target = GameObject.FindGameObjectWithTag ("Player").transform;
    }

    void Update()
    {
        transform.right = target.position - transform.position;

        //move if distance from target is greater than MinDist
        if (Vector3.Distance(transform.position, target.position) > MinDistance){

            transform.position = Vector2.MoveTowards(transform.position, target.position, runSpeed * Time.deltaTime);
        } else {
            animator.SetTrigger("enemyAttack");
        }
    }
}
