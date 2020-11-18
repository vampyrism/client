using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OtherPlayer : Character
{

    public override void TakeDamage(int damage) {
        Debug.Log("OtherPlayer took " + damage + " damage!");
    }

}
