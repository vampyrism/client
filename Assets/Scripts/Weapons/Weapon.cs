using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public string weaponName;
    public int weaponIndex;
    public float weaponDamage;
    public float reloadSpeed;
    public bool isRanged;

    public abstract void MakeAttack(Vector2 targetPosition, Vector2 myPosition);
}
