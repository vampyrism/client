using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Enemy : MonoBehaviour
{
    Rigidbody2D body;

    //float moveLimiter = 0.7f;

    public float runSpeed = 0.5f;
    public float MinDistance = 0.5f;

    public int currentPathIndex;
    public List<Vector3> pathVectorList;

    private Transform target;
    private Animator animator;

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        animator  = GetComponent<Animator>();
        target = GameObject.FindGameObjectWithTag ("Player").transform;
        pathVectorList = null;
        currentPathIndex = 0;
    }

    void Update()
    {
        HandleMovement();
    }


    public void HandleMovement() { 

        if (pathVectorList != null) {
            Vector3 currentPathPosition = pathVectorList[currentPathIndex];
            //transform.right = targetPosition - transform.position;
            Debug.DrawLine(transform.position, currentPathPosition, Color.green);

            if (Vector3.Distance(transform.position, currentPathPosition) > 2f) {
                Vector3 moveDir = (currentPathPosition - transform.position).normalized;
                body.velocity = new Vector2(moveDir.x * runSpeed, moveDir.y * runSpeed);

            } else {
                currentPathIndex++;
                if (currentPathIndex >= pathVectorList.Count) {
                    //Found the position of the list, stop moving or find new target
                    pathVectorList = null;
                    body.velocity = new Vector3(0,0,0);
                }
            }
        } else {
            SetTargetPosition(target.position);
        }
    }

    public Vector3 GetPosition() {
        return transform.position;
    }

    public void Damage() {
        Debug.Log("Ouch, that hurt //Enemy");
    }

    public void SetTargetPosition(Vector3 targetPosition) {
        currentPathIndex = 0;
        pathVectorList = Pathfinding.Instance.FindPath(GetPosition(), targetPosition);
        /*
        if (pathVectorList != null && pathVectorList > 1) {
            pathVectorList.RemoveAt(0);
        }
        */
    }

    private void OnCollisionEnter2D(Collision2D collision) {
       
        //pathVectorList[currentPathIndex] = Pathfinding.Instance.FixCornerCollision(transform.position, pathVectorList[currentPathIndex], collision.GetContact(0).point);

        
       
    }
}
