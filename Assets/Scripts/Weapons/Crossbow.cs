using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crossbow : Weapon
{
    public Crossbow() {
        this.weaponDamage = 15f;
        this.reloadSpeed = 0.5f;
        this.isRanged = true;
        this.weaponName = "crossbow";
    }
}
