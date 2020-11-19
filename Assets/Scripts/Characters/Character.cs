using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
    [SerializeField] public float health = 5;
    public abstract void TakeDamage(float damage);

}
