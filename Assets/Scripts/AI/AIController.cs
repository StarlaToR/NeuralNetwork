using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    #region Variables

    [SerializeField] TextAsset objectList;
    [SerializeField] TextAsset networkSaved;
    [SerializeField] CharacterVision vision;
    [SerializeField] Network network;
    [SerializeField] Character character;

    [HideInInspector] public Transform levelEnd;

    public int trainingIteration = 1000;
    public bool networkCreated = false;

    public int nbPlatform = 7;
    public int nbEnemy = 2;
    public int nbKillzone = 3;

    public float defaultInputValue = 0f;

    public bool isPlaying = false;

    List<List<float>> data;
    bool dataLoaded = false;

    public bool trainingStarted = false;

    #endregion

    #region MonoBehaviour

    void Start()
    {

    }

    void Update()
    {
        if (isPlaying)
            Play();
    }

    #endregion

    #region Network Creation

    public void Create()
    {
        network.NbInputPerceptrons = 3 + nbPlatform * 4 + nbEnemy * 2 + nbKillzone * 2;

        if (network.NbPerceptronsPerHiddenLayer.Count == 0)
        {
            network.NbPerceptronsPerHiddenLayer.Add(16);
            network.NbPerceptronsPerHiddenLayer.Add(10);
        }

        network.NbOutputPerceptrons = 2;

        network.Generate();

        networkCreated = true;
    }

    #endregion

    #region Network Training
    public void Train()
    {
        if (!networkCreated)
            Create();

        if (!dataLoaded)
            ExtractDataFromDataBase();

        for (int i = 0; i < trainingIteration; i++)
            for (int j = 0; j < data.Count; j += 2)
                network.LearningProcess(data[j], data[j + 1]);
    }

    void ExtractDataFromDataBase()
    {
        data = new List<List<float>>();
        List<float> inputs;
        List<float> outputs;

        string content = objectList.text;

        string[] splittedContent = content.Split("\n");

        for (int i = 0; i < splittedContent.Length; i+= 3)
        {
            inputs = new List<float>();
            outputs = new List<float>();

            string[] splittedLine = splittedContent[i].Split(';');

            int platforms = int.Parse(splittedLine[1]);
            int ennemis = int.Parse(splittedLine[2]);
            int killzones = int.Parse(splittedLine[3]);

            bool platformLoaded = false;
            bool enemiLoaded = false;

            splittedLine = splittedContent[i + 1].Split(';');

            for (int j = 0; j < splittedLine.Length; j++)
            {
                inputs.Add(float.Parse(splittedLine[j]));

                if (!platformLoaded && j == 3 + platforms * 4 - 1 && platforms < nbPlatform)
                {
                    for (int k = 0; k < (nbPlatform - platforms) * 4; k++)
                    {
                        inputs.Add(defaultInputValue);
                    }

                    platformLoaded = true;
                }
                
                if (platformLoaded && !enemiLoaded && (ennemis == 0 || (j == 3 + platforms * 4 + ennemis * 2 - 1 && ennemis < nbEnemy)))
                {
                    for (int k = 0; k < (nbEnemy - ennemis) * 2; k++)
                    {
                        inputs.Add(defaultInputValue);
                    }

                    enemiLoaded = true;
                }

                if (enemiLoaded && (killzones == 0 || (j == 3 + platforms * 4 + ennemis * 2 + killzones * 2 - 1 && killzones < nbKillzone)))
                {
                    for (int k = 0; k < (nbKillzone - killzones) * 2; k++)
                    {
                        inputs.Add(defaultInputValue);
                    }
                }
            }

            splittedLine = splittedContent[i + 2].Split(';');

            outputs.Add(int.Parse(splittedLine[0]));
            outputs.Add(int.Parse(splittedLine[1]));

            data.Add(inputs);
            data.Add(outputs);
        }

        dataLoaded = true;
    }
    #endregion

    #region In-Game Behavior
    public void Play()
    {
        if (!networkCreated)
            Create();

        network.FeedForward(ComputeInputs());

        ApplyOutputs(network.GetOutputs());
    }

    List<float> ComputeInputs()
    {
        List<float> inputs = new List<float>();

        inputs.Add(character.isGrounded ? 1 : 0);

        inputs.Add(levelEnd.position.x - transform.position.x);
        inputs.Add(levelEnd.position.y - transform.position.y);

        List<List<GameObject>> gameObjectsInView = vision.objectsInView;

        for (int i = 0; i < gameObjectsInView.Count; i++)
        {
            for (int j = 0; j < gameObjectsInView[i].Count; j++)
            {
                inputs.Add(gameObjectsInView[i][j].transform.position.x - transform.position.x);
                inputs.Add(gameObjectsInView[i][j].transform.position.y - transform.position.y);

                if (i == 0)
                {
                    inputs.Add(gameObjectsInView[i][j].transform.lossyScale.x);
                    inputs.Add(gameObjectsInView[i][j].transform.lossyScale.y);
                }
            }

            if (i == 0)
            {
                for (int j = 0; j < (nbPlatform - gameObjectsInView[i].Count) * 4; j++)
                {
                    inputs.Add(defaultInputValue);
                }
            }
            else if (i == 1)
            {
                for (int j = 0; j < (nbEnemy - gameObjectsInView[i].Count) * 2; j++)
                {
                    inputs.Add(defaultInputValue);
                }
            }
            else if (i == 2)
            {
                for (int j = 0; j < (nbKillzone - gameObjectsInView[i].Count) * 2; j++)
                {
                    inputs.Add(defaultInputValue);
                }
            }
        }

        return inputs;
    }

    void ApplyOutputs(List<float> outputs)
    {
        float movement = outputs[0];
        float jumping = outputs[1];

        if (movement > 0.5f)
            character.Move(1);
        else if (movement < -0.5f)
            character.Move(-1);

        if (jumping > 0.8f)
            character.Jump();
    }
    #endregion

    #region Save & Load

    public void Save()
    {
        string path = Application.dataPath + "/TrainedAI.txt";
        string data = network.Save();

        File.WriteAllText(path, data);
    }

    public void Load()
    {
        network.Load(networkSaved.text);
    }

    #endregion
}
