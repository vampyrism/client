using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OtherPlayer : Character
{

    public override void TakeDamage(float damage) {

        Debug.Log("OtherPlayer took " + damage + " damage!");
        currentHealth = currentHealth - damage;
        if (currentHealth <= 0) {
            Destroy(gameObject);
        }
    }

    public override void FakeAttack(Vector2 targetPosition, int weaponType)
    {
        throw new System.NotImplementedException();
    }

    public override void TryToAttack(Vector2 targetPosition)
    {
        throw new System.NotImplementedException();
    }

}
