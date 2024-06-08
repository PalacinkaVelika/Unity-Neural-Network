using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour {

    public bool AI = false;

    PlayerMovement movementScript;
    float horizontalInput = 0f;
    bool jumpInput = false;

    void Start() {
        movementScript = GetComponent<PlayerMovement>();
    }

    private void Update() {
        if (!AI) {
            movementScript.MovePlayer(Input.GetAxis("Horizontal"), Input.GetButtonDown("Jump"));
        } else {
            horizontalInput = Random.Range(-1f, 1f);
            jumpInput = Random.value > 0.5f;
            movementScript.MovePlayer(horizontalInput, jumpInput);
        }
    }
}
