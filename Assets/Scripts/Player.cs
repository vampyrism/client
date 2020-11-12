using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Rigidbody2D body;

    public float x = 0.0f;
    public float y = 0.0f;
    public float vx = 0.0f;
    public float vy = 0.0f;
    public float intendedVx = 0.0f;
    public float intendedVy = 0.0f;
    
    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        x = body.position.x;
        y = body.position.y;
        vx = body.velocity.x;
        vy = body.velocity.y;
    }
}
