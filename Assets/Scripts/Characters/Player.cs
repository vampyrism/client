using Assets.Server;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : Character {
    // Networking
    public bool IsCurrentPlayer { get; set; } = false;

    // Variables regarding physics
    Rigidbody2D body;
    SpriteRenderer sprite;
    private Animator animator;

    // Variables regarding movement
    private float moveLimiter = 0.7f;
    [SerializeField] private float runSpeed = 1.0f;
    [SerializeField] private string playerName;

    /*public float x { get; private set; } = 0.0f;
    public float y { get; private set; } = 0.0f;
    public float vx { get; private set; } = 0.0f;
    public float vy { get; private set; } = 0.0f;*/

    // Variables regarding weapons and items
    [SerializeField] public List<GameObject> weaponsList;
    private List<bool> weaponsBoolList;
    public Weapon equippedWeapon = null;

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

        body.AddForce(new Vector2(horizontal * runSpeed, vertical * runSpeed), ForceMode2D.Impulse);
    }
    public override void DirectMove(float x, float y, float dx, float dy)
    {
        this.transform.position = new Vector3(x, y);
        body.AddForce(new Vector2(dx, dy), ForceMode2D.Impulse);
    }

    public void TryGrabObject() {
        if (itemOnFloor == null) {
            Debug.Log("Trying to grab an item when noone exist");
            return;
        }
        if (itemOnFloor.tag == "Weapon") {
            Weapon weaponOnFloor = itemOnFloor.GetComponent<Weapon>();

            if (weaponsBoolList[weaponOnFloor.weaponIndex] == false) {
                ItemPickupMessage m = new ItemPickupMessage(
                    0,
                    this.ID,
                    0,
                    0,
                    weaponOnFloor.ID,
                    0
                    );
                Debug.Log("I send this debug message: " + m);
                NetworkClient.GetInstance().MessageQueue.Enqueue(m);
            } else {
                // Already had the weapon which we tried to pick up.
            }
        }
    }

    public void GrabWeapon( Weapon weapon) {
        weaponsBoolList[weapon.weaponIndex] = true;
        SwitchWeapon(weapon.weaponIndex);
        availableWeapons.SetActive(weapon.weaponIndex);
        itemOnFloor = null;
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

    public void TryToAttack(Vector2 targetPosition, int weaponId) {
        if (Time.time >= timestampForNextAction) {

            equippedWeapon = weaponsList[weaponId].GetComponent<Weapon>();
            animator.SetTrigger("Attack");

            this.equippedWeapon.MakeAttack(targetPosition, transform.position, this.ID);
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

    private void FixedUpdate()
    {
        if(this.Controllable && transform.hasChanged)
        {
            transform.hasChanged = false;

            base.X = body.position.x;
            base.Y = body.position.y;
            base.DX = body.velocity.x;
            base.DY = body.velocity.y;

            GameManager.instance.UpdateEntityPosition(this);
        }
    }
}
