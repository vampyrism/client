using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Vector3 shootDir;
    public float moveSpeed = 20;
  public void Setup(Vector3 shootDirection) {
        this.shootDir = shootDirection;
    }

    private void Update() {
        Debug.Log("shootDir");
        transform.position += shootDir * moveSpeed * Time.deltaTime;
    }
}
