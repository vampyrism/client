using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Projectile : MonoBehaviour
{
    public float moveSpeed = 20;
    private float projectileDamage;
    private UInt32 senderId;
    public Vector2 clickPos;
    public short weaponType;
    public void Setup(Vector3 shootDirection, float weapondamage, UInt32 sendId, Vector2 cPos, short wepType) {
        projectileDamage = weapondamage;
        senderId = sendId;
        clickPos = cPos;
        weaponType = wepType;
        Rigidbody2D body = GetComponent<Rigidbody2D>();
        body.AddForce(shootDirection * moveSpeed, ForceMode2D.Impulse);
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
        
        Character hitCharacter = collider.GetComponent<Character>();
        if (hitCharacter != null) {
            if (hitCharacter.ID == senderId) {
                return;
            }
        
            Destroy(this.gameObject);
        } else if (collider.name == "Collision_Default"){
            // Hit a wall
            Debug.Log("Hit the wall");
            Rigidbody2D body = GetComponent<Rigidbody2D>();
            GetComponent<BoxCollider2D>().enabled = false;
            body.velocity = new Vector2(0, 0);
            GetComponent<Animator>().SetBool("Destroy", true);
            Destroy(this.gameObject, 0.5f);
        }
    }
}
