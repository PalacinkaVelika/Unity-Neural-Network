using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Evolution : MonoBehaviour {
    public static Evolution Instance { get; private set; }


    // Mutation
    public float mutationChance = 0.1f;
    public float mutationAmount = 0.1f;
    // NN
    public int inputLayerSize = 16; // záleží jak vymyslím oèi
    public int hiddenLayerCount = 4;
    public int hiddenLayerSize = 8;
    public int outputLayerSize = 2; // horizontal movement and jump
    // Agents (generation stuff)
    public GameObject agentPrefab;
    public Transform spawnPosition;
    public bool showRaycasts = false;
    public int parentPerGeneration = 2;
    public int agentsPerGeneration = 10;
    public float timePerGeneration = 15f;
    public float raycastLength = 1.5f;
    // Simulation
    public float simulationSpeed = 1f;

    List<Agent> allAgents = new List<Agent>();

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            //   DontDestroyOnLoad(gameObject);
        } else if (Instance != this) {
            Destroy(gameObject);
        }
    }

    void Start() {
        StartSimulation();
    }

    void Update() {
        Time.timeScale = simulationSpeed;
    }

    // Start new simulation
    public void StartSimulation() {
        CreateNewAgents();
        StartGeneration();
    }

    // Starts a round
    public void StartGeneration() {
        StartCoroutine(GenerationTimer()); // Ukonèi generaci po daném èase
    }

    public void EndGeneration() {
        // Kill everyone
        foreach (Agent agent in allAgents) {
            Destroy(agent.gameObject);
        }
        allAgents = new List<Agent>(); // must reset
        // Choose parents (best fit)
        // Create new agents based on parents
        CreateNewAgents();
        StartGeneration();
    }

    // Top parentPerGeneration agents by fitness score
    List<Agent> ChooseParents() {
        allAgents.Sort((agent1, agent2) => agent2.fitnessScore.CompareTo(agent1.fitnessScore));
        List<Agent> parents = new List<Agent>();
        for (int i = 0; i < parentPerGeneration; i++) {
            parents.Add(allAgents[i]);
        }
        return parents;
    }

    // Parents create children, children mutate by themselves after being born
    void CreateNewGenerationAgents() {
        List<Agent> parents = ChooseParents();
        int group_size = (int)(agentsPerGeneration / parentPerGeneration);
        foreach (Agent parent in parents) {
            for(int i = 0; i < group_size; i++) {
                GameObject agent = Instantiate(agentPrefab, spawnPosition.position, Quaternion.identity);
                Agent agentScript = agent.GetComponent<Agent>();
                agentScript.InheritNN(parent.NN);
                agentScript.renderRaycast = showRaycasts;
                allAgents.Add(agentScript);
            }
        }
    }

    // Spawn new agents
    void CreateNewAgents() {
        for (int i = 0; i < agentsPerGeneration; i++) {
            GameObject agent = Instantiate(agentPrefab, spawnPosition.position, Quaternion.identity);
            Agent agentScript = agent.GetComponent<Agent>();
            agentScript.CreateNewNN(inputLayerSize, hiddenLayerSize, hiddenLayerCount, outputLayerSize);
            agentScript.renderRaycast = showRaycasts;
            allAgents.Add(agentScript);
        }
    }

    IEnumerator GenerationTimer() {
        yield return new WaitForSeconds(timePerGeneration);
        EndGeneration();
    }


}
