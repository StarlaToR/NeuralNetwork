using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ActivationFunction
{
    SIGMOID,
    TANH,
    RELU,
}

public class Network : MonoBehaviour
{
    #region ActivationFunctions
    public ActivationFunction function;

    static public float Lambda = 15f;

    float Sigmoid(float input)
    {
        return 1f / (1f + Mathf.Exp(-1 * Lambda * input));
    }

    float DerivativeSigmoid(float input)
    {
        return input * (1 - input);
    }

    float TanH(float input)
    {
        float exp1 = Mathf.Exp(input);
        float exp2 = Mathf.Exp(-input);

        return (exp1 - exp2) / (exp1 + exp2);
    }

    float DerivativeTanH(float input)
    {
        return (1 - input * input) / 2.0f;
    }

    float ReLU(float input)
    {
        return Mathf.Max(0, input);
    }

    float DerivativeReLU(float input)
    {
        return input > 0f ? 1f : 0f;
    }

    public float ComputeActivation(float input)
    {
        switch (function)
        {
            case ActivationFunction.SIGMOID:
                return Sigmoid(input);
            case ActivationFunction.RELU:
                return ReLU(input);
            case ActivationFunction.TANH:
                return TanH(input);
            default:
                return 0;
        }
    }

    public float ComputeDerivative(float input)
    {
        switch (function)
        {
            case ActivationFunction.SIGMOID:
                return DerivativeSigmoid(input);
            case ActivationFunction.RELU:
                return DerivativeReLU(input);
            case ActivationFunction.TANH:
                return DerivativeTanH(input);
            default:
                return 0;
        }
    }
    #endregion

    #region Variables
    public int NbInputPerceptrons = 2;
    public List<int> NbPerceptronsPerHiddenLayer = new List<int>();
    public int NbOutputPerceptrons = 2;

    List<Perceptron> inputLayer = new List<Perceptron>();
    List<List<Perceptron>> hiddenLayers = new List<List<Perceptron>>();
    List<Perceptron> outputLayer = new List<Perceptron>();

    public bool useBias = true;
    public float InitWeightRange = 0.2f;

    public float Gain = 0.3f;
    #endregion

    #region General Methods

    public void FeedForward(List<float> inputs)
    {
        int i = 0;
        if (useBias)
        {
            inputLayer[i].output = 1f;
            i++;
        }

        for (; i < inputLayer.Count; i++)
        {
            int a = useBias ? i - 1 : i;
            if (a < inputs.Count)
            {
                inputLayer[i].output = inputs[useBias ? i - 1 : i];
            }
        }

        for (int j = 0; j < hiddenLayers.Count; j++)
            for (int k = 0; k < hiddenLayers[j].Count; k++)
                hiddenLayers[j][k].FeedForward();

        for (int j = 0; j < outputLayer.Count; j++)
            outputLayer[j].FeedForward();
    }

    public List<float> GetOutputs()
    {
        List<float> outputs = new List<float>();

        for (int i = 0; i < outputLayer.Count; i++)
        {
            outputs.Add(outputLayer[i].output);
        }

        return outputs;
    }

    #endregion

    #region Learning
    public void LearningProcess(List<float> inputs, List<float> wantedOutput)
    {
        FeedForward(inputs);
        BackPropagation(wantedOutput);
    }

    void BackPropagation(List<float> wantedOutputs)
    {
        for (int i = 0; i < outputLayer.Count; i++)
        {
            Perceptron outputPerceptron = outputLayer[i];
            float errorValue = outputPerceptron.output * (1 - outputPerceptron.output) * (wantedOutputs[i] - outputPerceptron.output);
            outputPerceptron.AdjustWeight(errorValue);
        }

        for (int i = hiddenLayers.Count - 1; i >= 0; i--)
        {
            List<Perceptron> hiddenLayer = hiddenLayers[i];
            List<Perceptron> nextLayer = i == hiddenLayers.Count - 1 ? outputLayer : hiddenLayers[i + 1];

            for (int j = 0; j < hiddenLayer.Count; j++)
            {
                Perceptron hiddenPerceptron = hiddenLayer[i];
                float sum = 0f;

                for (int k = 0; k < nextLayer.Count; k++)
                {
                    Perceptron nextPerceptron = nextLayer[k];
                    sum += nextPerceptron.GetIncomingWeight(hiddenPerceptron) * nextPerceptron.error;
                }

                float errorValue = ComputeDerivative(hiddenPerceptron.output) * sum;
                hiddenPerceptron.AdjustWeight(errorValue);
            }
        }
    }
    #endregion

    #region Network Creation
    // Start is called before the first frame update
    public void Generate()
    {
        if (useBias)
        {
            Perceptron bias = new Perceptron(this);
            inputLayer.Add(bias);
        }

        for (int i = 0; i < NbInputPerceptrons; i++)
        {
            Perceptron inputPerceptron = new Perceptron(this);
            inputLayer.Add(inputPerceptron);
        }

        for (int i = 0; i < NbPerceptronsPerHiddenLayer.Count; i++)
        {
            int nbPerceptrons = NbPerceptronsPerHiddenLayer[i];
            List<Perceptron> hiddenLayer = new List<Perceptron>();
            List<Perceptron> prevLayer = i == 0? inputLayer : hiddenLayers[i - 1];

            for (int j = 0; j < nbPerceptrons; j++)
            {
                Perceptron hiddenPerceptron = new Perceptron(this);

                if (useBias)
                {
                    Input bias = new Input();
                    bias.input = null;
                    bias.weight = Random.Range(-InitWeightRange, InitWeightRange);
                    hiddenPerceptron.inputs.Add(bias);
                }

                foreach (Perceptron perceptron in prevLayer)
                {
                    Input input = new Input();
                    input.input = perceptron;
                    input.weight = Random.Range(-InitWeightRange, InitWeightRange);
                    hiddenPerceptron.inputs.Add(input);
                }

                hiddenLayer.Add(hiddenPerceptron);
            }
            hiddenLayers.Add(hiddenLayer);
        }

        for (int i = 0; i < NbOutputPerceptrons; i++)
        {
            Perceptron hiddenPerceptron = new Perceptron(this);
            List<Perceptron> prevLayer = NbPerceptronsPerHiddenLayer.Count > 0 ? hiddenLayers[hiddenLayers.Count - 1] : inputLayer;

            if (useBias)
            {
                Input bias = new Input();
                bias.input = null;
                bias.weight = Random.Range(-InitWeightRange, InitWeightRange);
                hiddenPerceptron.inputs.Add(bias);
            }

            foreach (Perceptron perceptron in prevLayer)
            {
                Input input = new Input();
                input.input = perceptron;
                input.weight = Random.Range(-InitWeightRange, InitWeightRange);
                hiddenPerceptron.inputs.Add(input);
            }

            outputLayer.Add(hiddenPerceptron);
        }
    }
    #endregion

    #region Save & Load

    public string Save()
    {
        string data = "";

        switch (function)
        {
            case ActivationFunction.SIGMOID:
                data += "0\n";
                break;
            case ActivationFunction.TANH:
                data += "1\n";
                break;
            case ActivationFunction.RELU:
                data += "2\n";
                break;
            default:
                break;
        }

        data += useBias.ToString() + '\n';
        data += NbInputPerceptrons.ToString() + ';';
        data += NbPerceptronsPerHiddenLayer.Count.ToString();

        string weights = "";

        for (int i = 0; i < NbPerceptronsPerHiddenLayer.Count; i++)
        {
            data += ';' + NbPerceptronsPerHiddenLayer[i].ToString();

            for (int j = 0; j < hiddenLayers[i].Count; j++)
            {
                List<Input> inputs = hiddenLayers[i][j].inputs;

                for (int k = 0; k < inputs.Count; k++)
                {
                    if (k != 0) weights += ';';
                    weights += inputs[k].weight.ToString();
                }
            }
        }

        data += ';' + NbOutputPerceptrons.ToString() + '\n';

        for (int j = 0; j < outputLayer.Count; j++)
        {
            List<Input> inputs = outputLayer[j].inputs;

            for (int k = 0; k < inputs.Count; k++)
            {
                weights += ';' + inputs[k].weight.ToString();
            }
        }

        data += weights;

        return data;
    }

    public void Load(string data)
    {
        inputLayer.Clear();
        hiddenLayers.Clear();
        outputLayer.Clear();

        string[] splittedData = data.Split('\n');

        //Get from data the dimensions of the neural network
        function = (ActivationFunction)int.Parse(splittedData[0]);
        useBias = bool.Parse(splittedData[1]);

        string[] networkDim = splittedData[2].Split(';');

        NbInputPerceptrons = int.Parse(networkDim[0]);

        int nbHiddenLayers = int.Parse(networkDim[1]);

        int i = 0;
        for (; i < nbHiddenLayers; i++)
        {
            NbPerceptronsPerHiddenLayer.Add(int.Parse(networkDim[2 + i]));
        }

        NbOutputPerceptrons = int.Parse(networkDim[2 + i]);

        //Generate Network
        Generate();

        //Fill Network with weights got from data
        string[] weights = splittedData[3].Split(';');

        i = 0;

        for (int j = 0; j < nbHiddenLayers; j++)
        {
            for (int k = 0; k < hiddenLayers[j].Count; k++)
            {
                List<Input> inputs = hiddenLayers[j][k].inputs;

                for (int l = 0; l < inputs.Count; l++)
                {
                    Input input = inputs[l];
                    input.weight = float.Parse(weights[i]);
                    i++;
                }
            }
        }

        for (int j = 0; j < outputLayer.Count; j++)
        {
            List<Input> inputs = outputLayer[j].inputs;

            for (int l = 0; l < inputs.Count; l++)
            {
                Input input = inputs[l];
                input.weight = float.Parse(weights[i]);
                i++;
            }
        }
    }

    #endregion
}
