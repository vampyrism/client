using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public float x;
    public float y;
    public float vx;
    public float vy;
    
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        float newX = transform.position.x;
        float newY = transform.position.y;

        vx = (newX - x) * Time.deltaTime;
        vy = (newY - y) * Time.deltaTime;

        x = newX;
        y = newY;
    }
}
