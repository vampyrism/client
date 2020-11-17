using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float moveSpeed = 20;
  public void Setup(Vector3 shootDirection) {
        Rigidbody2D rigidbody2D = GetComponent<Rigidbody2D>();
        rigidbody2D.AddForce(shootDirection * moveSpeed, ForceMode2D.Impulse);
        transform.eulerAngles = new Vector3(0, 0, GetAngleFromVectorFloat(shootDirection));
        Destroy(gameObject, 5f);
    }

    public static float GetAngleFromVectorFloat(Vector3 direction) {
        direction = direction.normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        if (angle < 0) angle += 360;

        return angle;
    }

    private void OnTriggerEnter2D(Collider2D collider) {
        Enemy enemy = collider.GetComponent<Enemy>();
        if (collider.GetComponent<Enemy>() != null) {
            // Hit an enemy
            enemy.Damage();
            Destroy(gameObject);
        } else if (collider.name == "Collision_Default"){
            // Hit a wall
            Debug.Log("Hit the wall");
            Destroy(gameObject);
        }

        

    }
}
