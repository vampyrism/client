using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    Rigidbody2D body;
    SpriteRenderer sprite;

    float horizontal;
    float vertical;
    float moveLimiter = 0.7f;
    public float runSpeed = 1.0f;

    public float x = 0.0f;
    public float y = 0.0f;
    public float vx = 0.0f;
    public float vy = 0.0f;

    public bool hasBow = false;

    private GameObject equippedItem;
    private GameObject itemOnFloor;
    [SerializeField] private Transform projectile;
    
    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        x = body.position.x;
        y = body.position.y;
        vx = body.velocity.x;
        vy = body.velocity.y;
    }

    public void Move(float horizontal, float vertical) {
        if (horizontal != 0 && vertical != 0) {// Check for diagonal movement
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

    public void GrabObject() {
        if (itemOnFloor == null) {
            Debug.Log("Trying to grab an item when noone exist");
            return;
        }
        if (itemOnFloor.transform.GetChild(0).gameObject.tag == "Bow") {
            hasBow = true;
            Destroy(itemOnFloor);
            itemOnFloor = null;
        }
    }

    public void ShootProjectile(Vector3 targetPosition) {
        Transform projectileTransform = Instantiate(projectile, transform.position, Quaternion.identity);
        Vector3 shootDir = (targetPosition - transform.position).normalized;
        projectileTransform.GetComponent<Projectile>().Setup(shootDir);

    }

    private void OnTriggerEnter2D(Collider2D collision) {

        if (collision.gameObject.tag == "Equipment") {
            itemOnFloor = collision.gameObject;
            Debug.Log("Inside OnTriggerEnter2D for player");
        }        
    }

    private void OnTriggerExit2D(Collider2D collision) {

        if (collision.gameObject.tag == "Equipment") {
            itemOnFloor = null;
            Debug.Log("Exiting Equipment trigger for player");
        }
    }

}
