using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentReceiver : MonoBehaviour {
    public static EnvironmentReceiver Instance { get; private set; }

    private void Awake() {
        if (Instance == null) {
            Instance = this;
         //   DontDestroyOnLoad(gameObject);
        } else if (Instance != this) {
            Destroy(gameObject);
        }
    }

    void Start() {

    }

    public void CoinCollected() {
        print("Coin collected");
    }

    public void GoalReached() {
        print("Goal reached");
    }
}
