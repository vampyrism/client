using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class Weapon : Entity
{
    public string weaponName;
    public int weaponIndex;
    public float weaponDamage;
    public float reloadSpeed;
    public bool isRanged;

    public abstract void MakeAttack(Vector2 clickPosition, Vector2 myPosition, UInt32 playerId);

    public override void DirectMove(float x, float y, float dx, float dy) {
        Debug.Log("Direct move inside a weapon is called with x: " + x + ", y: " + y);
        transform.position = new Vector2(x, y);
    }
}
