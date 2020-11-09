using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class camera : MonoBehaviour
{
    [Header("Camera Options")]
    [SerializeField]
    protected Transform trackingTarget;
    private Vector3 offset;

    [Tooltip("Speed at which camera follows (float)")]
    public float followSpeed;


    // Start is called before the first frame update
    void Start()
    {
        offset = transform.position - trackingTarget.transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position =
            new Vector3
            (
                Mathf.Lerp(transform.position.x, trackingTarget.position.x, Time.fixedDeltaTime * followSpeed),
                Mathf.Lerp(transform.position.y, trackingTarget.position.y, Time.fixedDeltaTime * followSpeed),
                -20
            );
    }
}
