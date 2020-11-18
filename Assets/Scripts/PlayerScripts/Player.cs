using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    Rigidbody2D body;
    SpriteRenderer sprite;

    private float moveLimiter = 0.7f;
    [SerializeField] private float runSpeed = 1.0f;

    public float x = 0.0f;
    public float y = 0.0f;
    public float vx = 0.0f;
    public float vy = 0.0f;

    [SerializeField] private bool hasBow = false;
    [SerializeField] private bool hasCrossbow = false;
    [SerializeField] private float reloadSpeed = 1f;
    private float timestampForNextAction;

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
        // Check for diagonal movement
        if (horizontal != 0 && vertical != 0) {
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

        body.velocity = new Vector2(horizontal * runSpeed, vertical * runSpeed);
    }

    public void GrabObject() {
        if (itemOnFloor == null) {
            Debug.Log("Trying to grab an item when noone exist");
            return;
        }
        if (itemOnFloor.tag == "Weapon") {
            Weapon weapon = itemOnFloor.GetComponent<Weapon>();
            reloadSpeed = weapon.reloadSpeed;
            if (weapon.weaponName == "bow") {
                hasBow = true;
            } else if (weapon.weaponName == "crossbow") {
                hasCrossbow = true;
            }
            Destroy(itemOnFloor);
            itemOnFloor = null;
        }
    }

    public void ShootProjectile(Vector2 targetPosition) {
        if (Time.time >= timestampForNextAction) {
            if (hasBow || hasCrossbow) {
                Transform projectileTransform = Instantiate(projectile, transform.position, Quaternion.identity);
                Vector3 shootDir = (targetPosition - (Vector2)transform.position).normalized;
                projectileTransform.GetComponent<Projectile>().Setup(shootDir);

                timestampForNextAction = Time.time + reloadSpeed;
            } else {
                Debug.Log("Trying to shoot without a bow");
            }
        } else {
            Debug.Log("Trying to fire too fast");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {

        if (collision.gameObject.tag == "Weapon") {
            itemOnFloor = collision.gameObject;
            Debug.Log("Inside Weapon OnTriggerEnter2D for player");
        }        
    }

    private void OnTriggerExit2D(Collider2D collision) {

        if (collision.gameObject.tag == "Weapon") {
            itemOnFloor = null;
            Debug.Log("Exiting Weapon trigger for player");
        }
    }

}
