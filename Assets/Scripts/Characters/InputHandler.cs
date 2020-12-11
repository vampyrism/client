using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour {
    float horizontal;
    float vertical;

    public Player player;

    void Start() {
        player = (Player) this.gameObject.GetComponent<Player>(); //GameObject.FindGameObjectWithTag("Player").transform.GetComponent<Player>();
    }

    void Update() {
        // Gives a value between -1 and 1
        horizontal = Input.GetAxisRaw("Horizontal"); // -1 is left
        vertical = Input.GetAxisRaw("Vertical"); // -1 is down

        if (horizontal != 0 || vertical != 0) {
            player.Move(horizontal, vertical);
        } else {
            player.StopMoving();
        }


        if (Input.GetKeyDown(KeyCode.F)) {
            player.TryGrabObject();
        }

        if (Input.GetKeyDown(KeyCode.E)) {
            // Cycling weapon to the right
            player.CycleEquippedWeapon(1);
        }

        if (Input.GetKeyDown(KeyCode.Q)) {
            // Cycling weapon to the left
            player.CycleEquippedWeapon(-1);
        }

        if (Input.GetKeyDown(KeyCode.Mouse0)) {
            player.TryToAttack((Vector2) Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }

    }
}
