using Assets.Server;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : Character {
    // Networking

    // Variables regarding physics
    Rigidbody2D body;
    SpriteRenderer sprite;
    private Animator animator;
    private Animator bloodAnimator;

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
        bloodAnimator = this.gameObject.transform.GetChild(0).gameObject.GetComponent<Animator>();

        weaponsBoolList = new List<bool>();
        // Setting true for "startingWeapon" position
        weaponsBoolList.Add(true);
        // Setting false for all other weapon positions
        for (int i = 1; i < weaponsList.Count; i++) {
            weaponsBoolList.Add(false);
        }

        currentHealth = maxHealth;
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

      SetFacingDirection(new Vector2(horizontal, vertical));

      animator.SetBool("isMoving", true);

      body.AddForce(new Vector2(horizontal * runSpeed, vertical * runSpeed), ForceMode2D.Impulse);
    }
    public override void DirectMove(float x, float y, float dx, float dy)
    {
        SetFacingDirection(new Vector2(dx, dy));
        if (dx == 0 && dy == 0) {
            animator.SetBool("isMoving", false);
        } else {
            animator.SetBool("isMoving", true);
        }

        this.transform.position = new Vector3(x, y);
        //body.AddForce(new Vector2(dx, dy), ForceMode2D.Impulse);
    }
    public void StopMoving() {
        animator.SetBool("isMoving", false);
        body.velocity = new Vector2(0, 0);
    }

    public void SetFacingDirection(Vector2 direction) {
        if (animator) {
            animator.SetFloat("xInput", direction.x);
            animator.SetFloat("yInput", direction.y);
        } else {
            body = GetComponent<Rigidbody2D>();
            sprite = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();
        }
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
        if (this.Controllable == true) {
            availableWeapons.SetActive(weapon.weaponIndex);
        }
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
        if (this.Controllable == true) {
            availableWeapons.ChooseWeapon(weaponIndex);
        }
        animator.SetFloat("weaponType", (float) weaponIndex);
        Debug.Log("Switching to weapon: " + weaponIndex);
    }

    public override void TryToAttack(Vector2 targetPosition) {
        if (Time.time >= timestampForNextAction) {
            GameManager.instance.HandleAttack(this.ID, targetPosition, (short) this.equippedWeapon.weaponIndex);

            SetFacingDirection(new Vector2(targetPosition.x - transform.position.x, targetPosition.y - transform.position.y));
            animator.SetFloat("xAttack", targetPosition.x - transform.position.x);
            animator.SetFloat("yAttack", targetPosition.y - transform.position.y);
            animator.SetTrigger("Attack");

            this.equippedWeapon.MakeAttack(targetPosition, transform.position, this.ID);

            timestampForNextAction = Time.time + equippedWeapon.reloadSpeed;

        } else {
            Debug.Log("Trying to fire too fast");
        }
    }

    public override void FakeAttack(Vector2 targetPosition, int weaponType) {
        SetFacingDirection(new Vector2(targetPosition.x - transform.position.x, targetPosition.y - transform.position.y));
        animator.SetFloat("xAttack", targetPosition.x - transform.position.x);
        animator.SetFloat("yAttack", targetPosition.y - transform.position.y);
        animator.SetTrigger("Attack");
        Debug.Log("weaponType in FakeAttack: " + weaponType);
        Weapon w = weaponsList[weaponType].GetComponent<Weapon>();
        w.MakeAttack(targetPosition, this.transform.position, this.ID);

    }

    public override void TakeDamage(float damage) {
        Debug.Log("Player took " + damage + " damage!");
        animator.SetTrigger("Hit");
        bloodAnimator.SetTrigger("Hit");
        currentHealth = currentHealth - damage;
        if (this.Controllable == true) {
            overworldHealthBar.SetHealth(currentHealth);
        }
        playerHealthBar.SetHealth(currentHealth);
        if (currentHealth <= 0) {

            GameManager.instance.HandleKilledPlayer(transform);
            GameManager.instance.GameOver();
            //Destroy(gameObject);
        }
    }

    public void SetupControllable() {
        overworldHealthBar = GameObject.Find("OverworldHealthBar").GetComponent<HealthBar>();
        availableWeapons = GameObject.Find("AvailableWeapons").GetComponent<AvailableWeapons>();
        overworldHealthBar.SetMaxHealth(maxHealth);
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
