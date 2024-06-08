using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity;
using static Unity.Burst.Intrinsics.Arm;

public class NeuralNetwork {

    Layer inputLayer;
    List<Layer> hiddenLayerList;
    Layer outputLayer;

    // size = number of neurons, count = number of layers
    public NeuralNetwork(int inputLayerSize, int hiddenLayerSize, int hiddenLayerCount, int outputLayerSize) {
        // Input
        inputLayer = new Layer(inputLayerSize);
        // Hidden
        hiddenLayerList = new List<Layer>();
        for (int i = 0; i < hiddenLayerCount; i++) {
            hiddenLayerList.Add(new Layer(hiddenLayerSize));
        }
        // Output
        outputLayer = new Layer(outputLayerSize);

        connectLayers();
        Mutate(0.2f, 0.5f); // Mutate right away to get different weights
    }

    void connectLayers() {
        // input - first hidden
        foreach (Neuron inputNeuron in inputLayer.getNeuronList()) {
            foreach (Neuron hiddenNeuron in hiddenLayerList[0].getNeuronList()) {
                hiddenNeuron.addInputConnection(inputNeuron);
                inputNeuron.addOutputConnection(hiddenNeuron);
            }
        }
        // first hidden - last hidden
        for (int i = 1; i < hiddenLayerList.Count; i++) {
            Layer currentLayer = hiddenLayerList[i];
            Layer previousHiddenLayer = hiddenLayerList[i - 1];
            foreach (Neuron neuron in currentLayer.getNeuronList()) {
                foreach (Neuron previousNeuron in previousHiddenLayer.getNeuronList()) {
                    neuron.addInputConnection(previousNeuron);
                    previousNeuron.addOutputConnection(neuron);
                }
            }
        }
        // last hidden - output
        Layer lastHiddenLayer = hiddenLayerList[hiddenLayerList.Count - 1];
        foreach (Neuron neuron in lastHiddenLayer.getNeuronList()) {
            foreach (Neuron outputNeuron in outputLayer.getNeuronList()) {
                neuron.addOutputConnection(outputNeuron);
                outputNeuron.addInputConnection(neuron);
            }
        }
    }

    public float[] CalculateOutput(float[] inputLayerValues) {
        // Fill the values into the input Layer
        for (int i = 0; i < inputLayerValues.Length; i++) {
            inputLayer.getNeuronList()[i].value = inputLayerValues[i];
        }

        // Forward prop.
        // input - first hidden
        foreach (Neuron inputNeuron in inputLayer.getNeuronList()) {
            foreach (Neuron hiddenNeuron in hiddenLayerList[0].getNeuronList()) {
              //  print("input->first");
              //  print("Before: " + hiddenNeuron.value);
                CalculateNewNeuronValue(inputNeuron, hiddenNeuron);
             //   print("After: " + hiddenNeuron.value);
            }
        }
        // first hidden - last hidden
        for (int i = 1; i < hiddenLayerList.Count; i++) {
            Layer currentLayer = hiddenLayerList[i];
            Layer previousHiddenLayer = hiddenLayerList[i - 1];
            foreach (Neuron neuron in currentLayer.getNeuronList()) {
                foreach (Neuron previousNeuron in previousHiddenLayer.getNeuronList()) {
               //     print("first->last");
               //     print("Before: " + neuron.value);
                    CalculateNewNeuronValue(previousNeuron, neuron);
               //     print("After: " + neuron.value);
                }
            }
        }
        // last hidden - output
        Layer lastHiddenLayer = hiddenLayerList[hiddenLayerList.Count - 1];
        foreach (Neuron outputNeuron in outputLayer.getNeuronList()) {
            foreach (Neuron hiddenNeuron in lastHiddenLayer.getNeuronList()) {
             //   print("last->out");
             //   print("Before: " + outputNeuron.value);
                CalculateNewNeuronValue(hiddenNeuron, outputNeuron);
              //  print("After: " + outputNeuron.value);
            }
        }

        // return output layer as float[]
        float[] output = new float[outputLayer.getNeuronList().Count];
        for (int i = 0; i < outputLayer.getNeuronList().Count; i++) {
            output[i] = outputLayer.getNeuronList()[i].value;
        }
        return output; 
    }

    void CalculateNewNeuronValue(Neuron nIn, Neuron nOut) {
        // Find connection of the input neuron and the hidden neuron
        nOut.value = 0; // Just to be sure reset the neuron value
        foreach (Connection inputConnection in nIn.outputConnections) {
            if (inputConnection != null && inputConnection.neuron == nOut) {
             //   print("Calculating");
             //   print(nOut.value);
            //    print(nIn.value);
             //  print(inputConnection.weight);
                // Calculate value for hidden neuron
                nOut.value += nIn.value * inputConnection.weight;
            }
        }
        nOut.value += nOut.bias;
        nOut.ActivationFunction();
    }

    public void Mutate(float mutationAmount, float mutationChance) {
        // input - first hidden
        foreach (Neuron inputNeuron in inputLayer.getNeuronList()) {
            foreach (Neuron hiddenNeuron in hiddenLayerList[0].getNeuronList()) {
                CalculateMutation(inputNeuron, hiddenNeuron, mutationAmount, mutationChance);
            }
        }
        // first hidden - last hidden
        for (int i = 1; i < hiddenLayerList.Count; i++) {
            Layer currentLayer = hiddenLayerList[i];
            Layer previousHiddenLayer = hiddenLayerList[i - 1];
            foreach (Neuron neuron in currentLayer.getNeuronList()) {
                foreach (Neuron previousNeuron in previousHiddenLayer.getNeuronList()) {
                    CalculateMutation(previousNeuron, neuron, mutationAmount, mutationChance);
                }
            }
        }
        // last hidden - output
        Layer lastHiddenLayer = hiddenLayerList[hiddenLayerList.Count - 1];
        foreach (Neuron outputNeuron in outputLayer.getNeuronList()) {
            foreach (Neuron hiddenNeuron in lastHiddenLayer.getNeuronList()) {
                CalculateMutation(hiddenNeuron, outputNeuron, mutationAmount, mutationChance);
            }
        }
    }

    void CalculateMutation(Neuron nIn, Neuron nOut, float mutationAmount, float mutationChance) {
        foreach (Connection inputConnection in nIn.outputConnections) {
            if (inputConnection != null && inputConnection.neuron == nOut) {
                // Calculate mutation of the connection weight
                if (UnityEngine.Random.value < mutationChance) {
                    inputConnection.weight += UnityEngine.Random.Range(-1f, 1f) * mutationAmount;
                }
            }
        }
        // Calculate mutation of the neruon bias
        if (UnityEngine.Random.value < mutationChance) {
            nOut.bias += UnityEngine.Random.Range(-1f, 1f) * mutationAmount;
        }
    }
}

public class Layer {

    List<Neuron> neuronList;

    // size = number of neurons
    public Layer(int size) {
        neuronList = new List<Neuron>();
        for (int i = 0; i < size; i++) {
            neuronList.Add(new Neuron());
        }
    }

    public List<Neuron> getNeuronList() {
        return neuronList;
    }
}

public class Connection {
    public Neuron neuron;
    public float weight;
    public Connection(Neuron neuron, float weight) {
        this.neuron = neuron;
        this.weight = weight;
    }
}

public class Neuron {

    public List<Connection> inputConnections = new List<Connection>();
    public List<Connection> outputConnections = new List<Connection>();
    public float bias = 0f;
    public float value = 0f;

    public Neuron() {

    }

    public void addInputConnection(Neuron neuron) {
        inputConnections.Add(new Connection(neuron, 1f));
    }

    public void addOutputConnection(Neuron neuron) {
        outputConnections.Add(new Connection(neuron, 1f));
    }

    public void ActivationFunction() {
        if (value < 0f) value = 0f;
    }

}
