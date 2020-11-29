using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    public Transform target;

    // Start is called before the first frame update
    void Start()
    {
        //target = GameObject.FindGameObjectWithTag ("Player").transform;
    }

    public void SetTarget(GameObject target)
    {
        this.target = target.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (this.target)
        {
            transform.position = new Vector3(target.position.x, target.position.y, transform.position.z);
        }
    }
}
