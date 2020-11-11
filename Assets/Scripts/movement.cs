﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movement : MonoBehaviour
{
    Rigidbody2D body;
    SpriteRenderer sprite;
    
    float horizontal;
    float vertical;
    float moveLimiter = 0.7f;

    public float runSpeed = 1.0f;

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // Gives a value between -1 and 1
        horizontal = Input.GetAxisRaw("Horizontal"); // -1 is left
        vertical = Input.GetAxisRaw("Vertical"); // -1 is down
    }

    void FixedUpdate()
    {
        if (horizontal != 0 && vertical != 0) // Check for diagonal movement
        {
            // limit movement speed diagonally, so you move at 70% speed
            horizontal *= moveLimiter;
            vertical *= moveLimiter;
        }

        if (horizontal > 0) {
            sprite.flipX = true;
        }
        if (horizontal < 0) {
            sprite.flipX = false;
        }

        //body.AddForce(new Vector2(horizontal * runSpeed, vertical * runSpeed), ForceMode2D.Impulse);
        body.velocity = new Vector2(horizontal * runSpeed, vertical * runSpeed);
    }
}
