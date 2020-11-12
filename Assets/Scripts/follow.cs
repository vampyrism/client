using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class follow : MonoBehaviour
{
    public Transform target;

    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.FindGameObjectWithTag ("Player").transform;
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 whereToView = target.position;
        whereToView.z = -10;
        transform.position = whereToView;
    }
}
