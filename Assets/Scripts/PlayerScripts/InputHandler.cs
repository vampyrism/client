using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour {
    float horizontal;
    float vertical;

    public Player player;

    void Start() {
        player = GameObject.FindGameObjectWithTag("Player").transform.GetComponent<Player>();
    }

    void Update() {
        // Gives a value between -1 and 1
        horizontal = Input.GetAxisRaw("Horizontal"); // -1 is left
        vertical = Input.GetAxisRaw("Vertical"); // -1 is down
        player.Move(horizontal, vertical);

        if (Input.GetKeyDown(KeyCode.E)) {
            player.GrabObject();
        }
        if (Input.GetKeyDown(KeyCode.Mouse0)) {
            player.ShootProjectile((Vector2) Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }

    }
}
