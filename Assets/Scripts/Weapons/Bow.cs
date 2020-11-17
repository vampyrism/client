using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow : Weapon
{
    public Bow() {
        this.weaponDamage = 11f;
        this.reloadSpeed = 0.3f;
        this.isRanged = true;
        this.weaponName = "bow";
    }
}
