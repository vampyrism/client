using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow : Weapon
{

    [SerializeField] private Transform projectile;
    public Bow() {
        this.weaponName = "bow";
        this.weaponIndex = 1;
        this.weaponDamage = 11f;
        this.reloadSpeed = 0.3f;
        this.isRanged = true;
    }

    public override void MakeAttack(Vector2 targetPosition, Vector2 spawnPosition) {
        Vector2 attackDirection = (targetPosition - (Vector2) spawnPosition).normalized;
        Transform projectileTransform = Instantiate(projectile, spawnPosition, Quaternion.identity);
        projectileTransform.GetComponent<Projectile>().Setup(attackDirection);
    }

}
