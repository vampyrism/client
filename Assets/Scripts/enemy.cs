using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy : MonoBehaviour
{
    Rigidbody2D body;

    float moveLimiter = 0.7f;

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
        transform.right = target.position - transform.position;
        HandleMovement();
    }

    public void HandleMovement() {
        if (pathVectorList != null) {
            Vector3 targetPosition = pathVectorList[currentPathIndex];
            //Debug.Log("targetPosition: " + targetPosition);
            
            if (Vector3.Distance(transform.position, targetPosition) > 1f) {
                Vector3 moveDir = (targetPosition - transform.position).normalized;

                float distanceBefore = Vector3.Distance(transform.position, targetPosition);
                transform.position = transform.position + moveDir*runSpeed * Time.deltaTime;
            } else {
                currentPathIndex++;
                if (currentPathIndex >= pathVectorList.Count) {
                    //Found the position of the list, stop moving or find new target
                    pathVectorList = null;
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
}
