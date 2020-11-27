using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : Character {
    // Variables regarding physics
    Rigidbody2D body;
    SpriteRenderer sprite;
    private Animator animator;

    // Variables regarding movement
    private float moveLimiter = 0.7f;
    [SerializeField] private float runSpeed = 1.0f;
    [SerializeField] private string playerName;

    private float x = 0.0f;
    private float y = 0.0f;
    private float vx = 0.0f;
    private float vy = 0.0f;

    // Variables regarding weapons and items
    [SerializeField] private List<GameObject> weaponsList;
    private List<bool> weaponsBoolList;
    private Weapon equippedWeapon = null;

    private float timestampForNextAction;

    private GameObject itemOnFloor;

    // UI elements
    [SerializeField] private HealthBar overworldHealthBar;
    [SerializeField] private AvailableWeapons availableWeapons;
    [SerializeField] private Text nameTextBox;
    [SerializeField] private HealthBar playerHealthBar;

    // Start is called before the first frame update
    void Start()
    {
        nameTextBox.text = playerName;
        body = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        weaponsBoolList = new List<bool>();
        // Setting true for "startingWeapon" position
        weaponsBoolList.Add(true);
        // Setting false for all other weapon positions
        for (int i = 1; i < weaponsList.Count; i++) {
            weaponsBoolList.Add(false);
        }

        currentHealth = maxHealth;

        overworldHealthBar = GameObject.Find("OverworldHealthBar").GetComponent<HealthBar>();
        availableWeapons = GameObject.Find("AvailableWeapons").GetComponent<AvailableWeapons>();
        overworldHealthBar.SetMaxHealth(maxHealth);
        playerHealthBar.SetMaxHealth(maxHealth);

        //Setting "StartingWeapon" as first weapon
        SwitchWeapon(0);
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
                availableWeapons.SetActive(weaponOnFloor.weaponIndex);
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
            newWeaponIndex += weaponsList.Count;
        } else if (newWeaponIndex == weaponsList.Count) {
            newWeaponIndex = 0;
        }

        if (weaponsBoolList[newWeaponIndex] == false) {
            if (cycleDirection < 0) {
                CycleEquippedWeapon(cycleDirection - 1);
            } else {
                CycleEquippedWeapon(cycleDirection + 1);
            }
        } else {
            SwitchWeapon(newWeaponIndex);
        }
    }

    private void SwitchWeapon(int weaponIndex) {
        equippedWeapon = weaponsList[weaponIndex].GetComponent<Weapon>();
        availableWeapons.ChooseWeapon(weaponIndex);
    }

    public void TryToAttack(Vector2 targetPosition) {
        if (Time.time >= timestampForNextAction) {
            animator.SetTrigger("Attack");
            this.equippedWeapon.MakeAttack(targetPosition, transform.position);
            timestampForNextAction = Time.time + equippedWeapon.reloadSpeed;
           
        } else {
            Debug.Log("Trying to fire too fast");
        }
    }

    public override void TakeDamage(float damage) {
        Debug.Log("Player took " + damage + " damage!");
        animator.SetTrigger("Hit");
        currentHealth = currentHealth - damage;
        overworldHealthBar.SetHealth(currentHealth);
        playerHealthBar.SetHealth(currentHealth);
        if (currentHealth <= 0) {
            GameManager.instance.HandleKilledPlayer(transform);
            GameManager.instance.GameOver();
            Destroy(gameObject);
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
