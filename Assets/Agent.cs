using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour {

    public NeuralNetwork NN;
    Evolution evolution;
    PlayerMovement movementScript;
    public float raycastLength = 1.0f; // Default length of the raycasts
    public LayerMask layerMask; // Layer mask to include trigger colliders
    public bool renderRaycast = true;
    public float fitnessScore = 0f;
    float[] inputLayer;

    void Start() {
        evolution = Evolution.Instance;
        movementScript = GetComponent<PlayerMovement>();
        inputLayer = new float[16];
    }

    void Update() {
        RaycastCheck();
        Move();
    }

    public void CreateNewNN(int inputLayerSize, int hiddenLayerSize, int hiddenLayerCount, int outputLayerSize) {
        NN = new NeuralNetwork(inputLayerSize, hiddenLayerSize, hiddenLayerCount, outputLayerSize); 
    }

    public void InheritNN(NeuralNetwork parentNN, bool mutate = true) {
        NN = parentNN;
        if(mutate) {
            NN.Mutate(Evolution.Instance.mutationAmount, Evolution.Instance.mutationChance);
        }
    }

    public void Move() {
        float[] output = NN.CalculateOutput(inputLayer);
        movementScript.MovePlayer(Normalize(output[0], 0 , 3), Booleanize(output[1], 0, 3));
    }

    // potøebuju vylepšit -> možná ukládáat max hodnotu a podle ní normalizovat?? (maybe a big issue)
    float Normalize(float value, float min, float max) {
        return 2 * (value - min) / (max - min) - 1;
    }

    bool Booleanize(float value, float min, float max) {
        return Normalize(value, min, max) > 0.5f;
    }

    void RaycastCheck() {
        // Shoot raycasts in 8 directions
        RaycastHit2D[] hits = new RaycastHit2D[8];
        Vector2[] raycastDirections = {
            Vector2.right,      // Right
            Vector2.left,       // Left
            Vector2.up,         // Up
            Vector2.down,       // Down
            new Vector2(-1, 1), // Top Left
            new Vector2(1, 1),  // Top Right
            new Vector2(-1, -1),// Bottom Left
            new Vector2(1, -1)  // Bottom Right
        };
        // Temporarily ignore collisions between this object and the raycasts
        foreach (Collider2D collider in GetComponents<Collider2D>()) {
            foreach (Collider2D raycastCollider in collider.gameObject.GetComponentsInChildren<Collider2D>()) {
                Physics2D.IgnoreCollision(collider, raycastCollider, true);
            }
        }
        // Render and shoot raycasts in all directions
        for (int i = 0; i < 8; i++) {
            if(renderRaycast) Debug.DrawRay(transform.position, raycastDirections[i] * raycastLength, Color.red);
            hits[i] = Physics2D.Raycast(transform.position, raycastDirections[i], raycastLength, layerMask);
        }
        // Re-enable collisions between this object and the raycasts
        foreach (Collider2D collider in GetComponents<Collider2D>()) {
            foreach (Collider2D raycastCollider in collider.gameObject.GetComponentsInChildren<Collider2D>()) {
                Physics2D.IgnoreCollision(collider, raycastCollider, false);
            }
        }

        // Convert the raycast results to the input layer
        ConvertRaycastToLayer(hits);
    }

    // inputLayer má 16 -> 8 raycastù (vzdálenost + na co kouká)
    void ConvertRaycastToLayer(RaycastHit2D[] hits) {
        for (int i = 0; i < 8; i++) {
            if (hits[i].collider != null) {
                float distance = hits[i].distance;
                float normalizedDistance = distance / raycastLength; // Normalize the distance
                int layerValue = EncodeLayer(LayerMask.LayerToName(hits[i].collider.gameObject.layer));
                inputLayer[i] = normalizedDistance; // Store normalized distance
                inputLayer[i + 8] = layerValue; // Store layer value
            } else {
                inputLayer[i] = 1f; // No hit, set max distance (normalized)
                inputLayer[i + 8] = 0; // No layer
            }
        }
    }

    int EncodeLayer(string layerName) {
        switch (layerName) {
            case "Wall": return 1;
            case "Ground": return 2;
            case "Goal": return 3;
            default: return 0; // Unknown layer
        }
    }

}
