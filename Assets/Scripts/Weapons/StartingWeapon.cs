using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartingWeapon : Weapon
{
    public StartingWeapon() {
        this.weaponName = "startingWeapon";
        this.weaponIndex = 0;
        this.weaponDamage = 1f;
        this.reloadSpeed = 1f;
        this.isRanged = false;
    }

    public override void MakeAttack(Vector2 targetPosition, Vector2 spawnPosition) {
        return;
    }
}
