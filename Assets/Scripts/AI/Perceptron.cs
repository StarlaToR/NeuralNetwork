using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Input
{
    public float weight;
    public Perceptron input;
}

public class Perceptron
{
    public List<Input> inputs;
    public float output;
    public float error;

    Network network;

    public Perceptron(Network net)
    {
        network = net;
        inputs = new List<Input>();
    }

    public void FeedForward()
    {
        float sum = 0;

        foreach (Input input in inputs)
        {
            if (input.input == null)
                sum += input.weight * 1f;
            else
                sum += input.weight * input.input.output;
        }

        output = network.ComputeActivation(sum); 
    }

    public void AdjustWeight(float errorValue)
    {
        error = errorValue;

        for (int i = 0; i < inputs.Count; i++)
        {
            Input input = inputs[i];
            float inputOutput = input.input == null ? 1f : input.input.output;
            float deltaWeight = network.Gain * errorValue * inputOutput;
            input.weight += deltaWeight;
        }
    }

    public float GetIncomingWeight(Perceptron perceptron)
    {
        foreach (Input input in inputs)
        {
            if (input.input == perceptron)
                return input.weight;
        }
        return 0;
    }
}
