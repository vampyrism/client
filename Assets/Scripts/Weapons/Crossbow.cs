using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public override void MakeAttack(Vector2 targetPosition, Vector2 spawnPosition) {
        Vector2 attackDirection = (targetPosition - (Vector2)spawnPosition).normalized;
        Transform projectileTransform = Instantiate(projectile, spawnPosition, Quaternion.identity);
        projectileTransform.GetComponent<Projectile>().Setup(attackDirection);
    }
}
