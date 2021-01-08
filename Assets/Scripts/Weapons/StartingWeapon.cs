using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StartingWeapon : Weapon
{
    public float attackRangeX;
    public float attackRangeY;
    public float weaponDistanceFromPlayer;
    public float offsetInYDirection;
    public float offsetInXDirection;
    [SerializeField] private GameObject SwingAnimation;
    public StartingWeapon() {
        this.weaponName = "startingWeapon";
        this.weaponIndex = 0;
        this.weaponDamage = 1f;
        this.reloadSpeed = 1f;
        this.isRanged = false;
    }

    public override void MakeAttack(Vector2 clickPosition, Vector2 playerPosition, UInt32 playerId) {
        Vector2 attackDirection = (clickPosition - (Vector2)playerPosition).normalized;
        //Vector2 weaponBoxPosition = playerPosition + (attackDirection * weaponDistanceFromPlayer);

        //DebugDrawBox(weaponBoxPosition, new Vector2(attackRangeX, attackRangeY), AngleBetweenTwoPoints(clickPosition, playerPosition), Color.green, 3);
        GameObject swingAnimation = Instantiate(SwingAnimation, playerPosition + (attackDirection*weaponDistanceFromPlayer/2), Quaternion.identity);
        swingAnimation.transform.eulerAngles = new Vector3(0, 0, GetAngleFromVectorFloat(attackDirection)-90);
        swingAnimation.transform.localScale = new Vector3(0.5f, 0.5f);
        Destroy(swingAnimation, 0.2f);
    }

    public static float GetAngleFromVectorFloat(Vector3 direction) {
        direction = direction.normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        if (angle < 0) angle += 360;

        return angle;
    }


    // Old MakeAttack (now in server)
    /*public override void MakeAttack(Vector2 clickPosition, Vector2 playerPosition, UInt32 playerId) {
        Vector2 attackDirection = (clickPosition - (Vector2)playerPosition).normalized;
        Vector2 weaponBoxPosition = playerPosition + (attackDirection * weaponDistanceFromPlayer);
        weaponBoxPosition.y += offsetInYDirection;

        Collider2D[] hitTargets = Physics2D.OverlapBoxAll(weaponBoxPosition, new Vector2(attackRangeX, attackRangeY), AngleBetweenTwoPoints(clickPosition, playerPosition));
        DebugDrawBox(weaponBoxPosition, new Vector2(attackRangeX, attackRangeY), AngleBetweenTwoPoints(clickPosition, playerPosition), Color.green, 3);
        GameObject swingAnimation = Instantiate(SwingAnimation, playerPosition, Quaternion.identity);
        Destroy(swingAnimation, 0.2f);

        for (int i = 0; i < hitTargets.Length; i++) {
            Character hitCharacter = hitTargets[i].GetComponent<Character>();

            // Did we hit a character
            if (hitCharacter != null) {
                // Did we hit ourselves?
                if (hitCharacter.name == "Player(Clone)") {
                    continue;
                }
                // Hit an Character

            }
            else if (hitTargets[i].name == "Collision_Default") {
                // Hit a wall
                Debug.Log("Hit the wall");
            }
        }
    }

    */

    float AngleBetweenTwoPoints(Vector3 a, Vector3 b) {
        return (Mathf.Atan2(a.y - b.y, a.x - b.x) * Mathf.Rad2Deg) + 90;
    }


    void DebugDrawBox(Vector2 point, Vector2 size, float angle, Color color, float duration) {

        var orientation = Quaternion.Euler(0, 0, angle);

        // Basis vectors, half the size in each direction from the center.
        Vector2 right = orientation * Vector2.right * size.x / 2f;
        Vector2 up = orientation * Vector2.up * size.y / 2f;

        // Four box corners.
        var topLeft = point + up - right;
        var topRight = point + up + right;
        var bottomRight = point - up + right;
        var bottomLeft = point - up - right;

        // Now we've reduced the problem to drawing lines.
        Debug.DrawLine(topLeft, topRight, color, duration);
        Debug.DrawLine(topRight, bottomRight, color, duration);
        Debug.DrawLine(bottomRight, bottomLeft, color, duration);
        Debug.DrawLine(bottomLeft, topLeft, color, duration);
    }


}


