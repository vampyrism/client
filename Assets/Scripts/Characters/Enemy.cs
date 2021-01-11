using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Enemy : Character
{
    private Rigidbody2D body;
    private SpriteRenderer sprite;
    private Animator animator;

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

    // UI elements
    [SerializeField] private HealthBar enemyHealthBar;

    void Start()
    {
        enemyHealthBar.SetMaxHealth(maxHealth);
        body = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        animator  = GetComponent<Animator>();
    }

    public Vector3 GetPosition() {
        return transform.position;
    }

    public override void TakeDamage(float damage) {
        Debug.Log("Enemy took " + damage + " damage!");
        animator.SetTrigger("Hit");
        currentHealth = currentHealth - damage;
        enemyHealthBar.SetHealth(currentHealth);
        if (currentHealth <= 0) {
            //Destroy(gameObject);
        }
    }

    public void RemovePlayerFromTargets(Transform removedPlayer) {
        if (currentTarget == removedPlayer) {
            currentTarget = null;
        }
        targetList.Remove(removedPlayer);
    }

    public override void TryToAttack(Vector2 targetPosition)
    {
        throw new NotImplementedException();
    }

    public override void FakeAttack(Vector2 targetPosition, int notUsedInEnemy)
    {
        float newdx = targetPosition.x - transform.position.x;
        float newdy = targetPosition.y - transform.position.y;

        if (newdx > 0) {
            sprite.flipX = false;
        }
        if (newdx < 0) {
            sprite.flipX = true;
        }

        animator.SetFloat("xInput", newdx);
        animator.SetFloat("yInput", newdy);

        animator.SetTrigger("Attack");
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

    public override void DirectMove(float x, float y, float dx, float dy) {
        float newdx = x - transform.position.x;
        float newdy = y - transform.position.y;

        if (newdx > 0) {
            sprite.flipX = false;
        }
        if (newdx < 0) {
            sprite.flipX = true;
        }

        animator.SetFloat("xInput", newdx);
        animator.SetFloat("yInput", newdy);


        if (Mathf.Abs(newdx) < 0.1 && Mathf.Abs(newdy) < 0.1) {
            animator.SetBool("isMoving", false);
        }
        else {
            animator.SetBool("isMoving", true);
        }
        this.transform.position = new Vector3(x, y);
        //body.AddForce(new Vector2(dx, dy), ForceMode2D.Impulse);
    }
}
