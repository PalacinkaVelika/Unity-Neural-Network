using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class EyeFollow : MonoBehaviour {
    public Transform eyes;
    public float smoothSpeed = 0.1f;
    public float offsetStrength = 0.1f;
    public float maxOffsetX = 0.1f;
    public float maxOffsetY = 0.1f;

    Vector2 velocity = Vector2.zero;
    Vector2 offset;

    private void Update() {
        UpdateEyePosition();
    }

    void UpdateEyePosition() {
        Vector2 targetPosition = transform.position + (Vector3)offset;
        Vector2 smoothedPosition = Vector2.SmoothDamp(eyes.position, targetPosition, ref velocity, smoothSpeed);
        eyes.position = smoothedPosition;
    }

    public void OffsetEyes(Vector2 moveVector) {
        offset.x = Mathf.Clamp(moveVector.x * offsetStrength, -maxOffsetX, maxOffsetX);
        offset.y = Mathf.Clamp(moveVector.y * offsetStrength, -maxOffsetY, maxOffsetY);
    }

}
