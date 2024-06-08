using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour {

    List<GameObject> collectedBy = new List<GameObject>();

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Agent") && !collectedBy.Contains(collision.gameObject)) {
            collectedBy.Add(collision.gameObject);
            GetCollected(collision.GetComponent<Agent>());
        }
    }

    void GetCollected(Agent agent) {
        agent.fitnessScore += 100;
    }
}
