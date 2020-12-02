﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Enemy : Character
{
    Rigidbody2D body;

    [SerializeField] private float runSpeed = 8f;
    [SerializeField] private float minDistance = 2f;
    [SerializeField] private float enemyReach = 2f;
    [SerializeField] private float enemyDamage = 2f;
    [SerializeField] private float enemyAttackSpeed = 0.5f;

    private Vector2 oldPosition;
    private int stuckCount;

    private float timestampForNextAttack;


    [SerializeField] private int currentPathIndex;
    private List<Vector3> pathVectorList;

    private List<Transform> targetList;
    private Transform currentTarget;
    private Animator animator;

    // UI elements
    [SerializeField] private HealthBar enemyHealthBar;

    void Start()
    {
        enemyHealthBar.SetMaxHealth(maxHealth);
        body = GetComponent<Rigidbody2D>();
        animator  = GetComponent<Animator>();

        GameObject[] OtherPlayerGameObjectList = GameObject.FindGameObjectsWithTag("OtherPlayer");
        targetList = new List<Transform>();
        foreach (GameObject OtherPlayerGameObject in OtherPlayerGameObjectList) {
            targetList.Add(OtherPlayerGameObject.transform);
        }
        targetList.Add(GameObject.FindGameObjectWithTag("Player").transform);

        pathVectorList = null;
        currentPathIndex = 0;
        oldPosition = transform.position;

        timestampForNextAttack = Time.time;

        // Set the initial path
        UpdatePath();
    }

    void Update()
    {

        if (pathVectorList != null && currentTarget != null) {
            Vector3 currentPathPosition = pathVectorList[currentPathIndex];
            // If close to target, try to attack
            if (Vector3.Distance(transform.position, currentTarget.position) < enemyReach) {
                if (Time.time >= timestampForNextAttack) {
                    animator.SetTrigger("enemyAttack");
                    currentTarget.GetComponent<Character>().TakeDamage(enemyDamage);
                    timestampForNextAttack = Time.time + enemyAttackSpeed;
                    body.velocity = new Vector3(0, 0, 0);
                }
            }
            else {
                HandleMovement();
            }
        } else {
            UpdatePath();
        }
    }

    public void HandleMovement() {
        if ((Vector2) transform.position == oldPosition) {
            stuckCount += 1;
        } else {
            stuckCount = 0;
        }

        Vector3 currentPathPosition = pathVectorList[currentPathIndex];

        if (stuckCount > 4) {
            StartCoroutine(LerpPosition((Vector2)currentPathPosition, Vector3.Distance(transform.position, currentPathPosition)/runSpeed));
                
            stuckCount = 0;
        }


        //transform.right = targetPosition - transform.position;
        Debug.DrawLine(transform.position, currentPathPosition, Color.green);

        if (Vector3.Distance(transform.position, currentPathPosition) > minDistance) {
            Vector3 moveDir = (currentPathPosition - transform.position).normalized;
            body.velocity = new Vector2(moveDir.x * runSpeed, moveDir.y * runSpeed);

        } else {
            currentPathIndex++;
            // If the enemy is far away from the target it moves 7 tiles before retargeting.
            if (currentPathIndex >= pathVectorList.Count || currentPathIndex > 7) {
                //Found the position of the list, stop moving or find new target
                pathVectorList = null;
                body.velocity = new Vector3(0,0,0);
            }
        }

        oldPosition = transform.position;
    }

    public Vector3 GetPosition() {
        return transform.position;
    }

    public override void TakeDamage(float damage) {
        Debug.Log("Enemy took " + damage + " damage!");
        animator.SetTrigger("enemyHit");
        currentHealth = currentHealth - damage;
        enemyHealthBar.SetHealth(currentHealth);
        if (currentHealth <= 0) {
            Destroy(gameObject);
        }
    }

    public void RemovePlayerFromTargets(Transform removedPlayer) {
        if (currentTarget == removedPlayer) {
            currentTarget = null;
        }
        targetList.Remove(removedPlayer);
    }


    private void UpdatePath() {
        if (currentTarget == null) {
            SelectTarget();
            
        } else if (Vector2.Distance(transform.position, currentTarget.position) > enemyReach*2) {
            SelectTarget();
        }

        // If we didn't find any targets, return
        if (currentTarget == null) return;

        currentPathIndex = 0;
        pathVectorList = Pathfinding.Instance.FindPath(GetPosition(), currentTarget.position);
    }
    private void SelectTarget() {
        float closestDistance = float.MaxValue;

        foreach (Transform target in targetList) {
            float targetDistance = Vector2.Distance(transform.position, target.position);
            if (targetDistance < closestDistance) {
                closestDistance = targetDistance;
                currentTarget = target;
            }
        }
    }

    IEnumerator LerpPosition(Vector2 targetPosition, float duration) {
        float time = 0;
        Vector2 startPosition = transform.position;

        while (time < duration) {
            transform.position = Vector2.Lerp(startPosition, targetPosition, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        transform.position = targetPosition;
    }

    private void OnCollisionEnter2D(Collision2D collision) {
       
        //pathVectorList[currentPathIndex] = Pathfinding.Instance.FixCornerCollision(transform.position, pathVectorList[currentPathIndex], collision.GetContact(0).point);
    }
}