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

    [SerializeField] private List<GameObject> weaponsList;
    [SerializeField] private List<bool> weaponsBoolList;
    [SerializeField] private Weapon equippedWeapon = null;

    private GameObject itemOnFloor;
    [SerializeField] private Transform projectile;
    
    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();

        for (int i = 0; i < weaponsList.Count; i++) {
            weaponsBoolList.Add(false);
        }
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
            Weapon weaponOnFloor = itemOnFloor.GetComponent<Weapon>();

            if (weaponsBoolList[weaponOnFloor.weaponIndex] == false) {
                weaponsBoolList[weaponOnFloor.weaponIndex] = true;
                SwitchWeapon(weaponOnFloor.weaponIndex);
                Destroy(itemOnFloor);
                itemOnFloor = null;
            } else {
                // Already had the weapon which we tried to pick up.
            }
        }
    }

    public void CycleEquippedWeapon(int cycleDirection) {
        int newWeaponIndex = equippedWeapon.weaponIndex + cycleDirection;

        if (newWeaponIndex < 0) {
            newWeaponIndex += weaponsList.Count - 1;
        } else if (newWeaponIndex == weaponsList.Count) {
            newWeaponIndex = 0;
        }

        if (weaponsBoolList[newWeaponIndex] == false) {
            CycleEquippedWeapon(cycleDirection);
        } else {
            SwitchWeapon(newWeaponIndex);
        }
    }

    private void SwitchWeapon(int weaponIndex) {
        equippedWeapon = weaponsList[weaponIndex].GetComponent<Weapon>();
    }

    public void TryToAttack(Vector2 targetPosition) {
        if (Time.time >= timestampForNextAction) {
            this.equippedWeapon.MakeAttack(targetPosition, transform.position);
            timestampForNextAction = Time.time + equippedWeapon.reloadSpeed;
           
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
