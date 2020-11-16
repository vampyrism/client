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

            if (Vector3.Distance(transform.position, currentPathPosition) > 1f) {
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
        Debug.Log(body.velocity);
        Debug.Log(body.velocity.x);
        Debug.Log(body.velocity.x == 0);
        Vector3 currentPathPosition = pathVectorList[currentPathIndex];

        pathVectorList[currentPathIndex] = Pathfinding.Instance.FixCornerCollision(pathVectorList[currentPathIndex], collision.GetContact(0).point);

        if (body.velocity.x < 0.01f) {
            Debug.Log("Inside if");
            if (currentPathPosition.x > transform.position.x) {
                Debug.Log("Inside if 1");
                if (body.velocity.y > 0) {
                    Debug.Log("Inside if 11");
                    body.AddForce(new Vector2(0, 1000000), ForceMode2D.Impulse);
                } else {
                    Debug.Log("Inside if 12");
                    body.AddForce(new Vector2(0, -100), ForceMode2D.Impulse);
                }
            } else {
                Debug.Log("Inside if 2");
                transform.position = transform.position + new Vector3(10, 0);
            }
        } else {
            //body.AddForce(body.velocity, ForceMode2D.Impulse);
        }
        
       
    }
}
