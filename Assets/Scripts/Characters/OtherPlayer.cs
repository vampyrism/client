using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OtherPlayer : Character
{

    public override void TakeDamage(float damage) {

        Debug.Log("OtherPlayer took " + damage + " damage!");
        currentHealth = currentHealth - damage;
        if (currentHealth <= 0) {
            GameManager.instance.HandleKilledPlayer(transform);
            Destroy(gameObject);
        }
    }

}
