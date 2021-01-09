using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Crossbow : Weapon
{
    [SerializeField] private Transform projectile;
    public Crossbow() {
        this.weaponName = "crossbow";
        this.weaponIndex = 2;
        this.weaponDamage = 15f;
        this.reloadSpeed = 0.5f;
        this.isRanged = true;
    }

    public override void MakeAttack(Vector2 clickPosition, Vector2 spawnPosition, UInt32 shooterID) {
        Vector2 attackDirection = (clickPosition - (Vector2)spawnPosition).normalized;
        Transform projectileTransform = Instantiate(projectile, spawnPosition, Quaternion.identity);
        projectileTransform.GetComponent<Projectile>().Setup(attackDirection, weaponDamage, shooterID, clickPosition, 2);
    }
}
