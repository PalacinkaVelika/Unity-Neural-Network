using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    public LayerMask wallStuckLayers;
    public float wallCheckDistance = 0.55f;
    public float wallCheckHeight = 1f;

    private Rigidbody2D rb;
    private bool isGrounded;
    private Vector2 movement;
    private EyeFollow eyes;

    float moveInput;
    bool jumpInput;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        eyes = GetComponent<EyeFollow>();
    }

    private void Update() {

        movement = new Vector2(moveInput * moveSpeed, rb.velocity.y);

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        bool isTouchingWall = CheckForWall();

        if (jumpInput && isGrounded) {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }

        if (isTouchingWall) {
            movement.x = 0;
        }
        eyes.OffsetEyes(movement);
    }

    private void FixedUpdate() {
        MoveCharacter(movement);
    }

    private void MoveCharacter(Vector2 direction) {
        rb.velocity = new Vector2(direction.x, rb.velocity.y);
    }

    public void MovePlayer(float horizontalInput, bool jumpInput) {
        this.moveInput = horizontalInput;
        this.jumpInput = jumpInput;
    }

    private bool CheckForWall() {
        Vector2 boxSize = new Vector2(wallCheckDistance, wallCheckHeight);
        Vector2 raycastOrigin = (Vector2)transform.position + new Vector2(Mathf.Sign(moveInput) * wallCheckDistance * 0.5f, 0f);
        RaycastHit2D[] hits = Physics2D.BoxCastAll(raycastOrigin, boxSize, 0f, Vector2.right * Mathf.Sign(moveInput), 0f, wallStuckLayers);
     //   DebugDrawBoxCast(raycastOrigin, boxSize, wallCheckDistance, Mathf.Sign(moveInput));
        bool wallHit = hits.Length > 0;
        return wallHit;
    }

    private void DebugDrawBoxCast(Vector2 origin, Vector2 size, float distance, float direction) {
        Vector2 halfSize = size * 0.5f;
        Vector2 center = origin + new Vector2(distance * direction * 0.5f, 0f);
        Debug.DrawLine(center + new Vector2(-halfSize.x, -halfSize.y), center + new Vector2(halfSize.x, -halfSize.y), Color.red);
        Debug.DrawLine(center + new Vector2(halfSize.x, -halfSize.y), center + new Vector2(halfSize.x, halfSize.y), Color.red);
        Debug.DrawLine(center + new Vector2(halfSize.x, halfSize.y), center + new Vector2(-halfSize.x, halfSize.y), Color.red);
        Debug.DrawLine(center + new Vector2(-halfSize.x, halfSize.y), center + new Vector2(-halfSize.x, -halfSize.y), Color.red);
    }
}

