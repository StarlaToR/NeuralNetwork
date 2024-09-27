using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DataRecorder : MonoBehaviour
{
    #region Variables
    [SerializeField] CharacterVision vision;
    [SerializeField] Player player;
    [SerializeField] Rigidbody playerRB;
    public Transform end;

    public bool isRecording = false;
    public bool Saved = false;
    public int RecordPerSecond = 60;

    List<List<int>> nbObjectPerFrame = new List<List<int>>();
    List<List<int>> outputs = new List<List<int>>();
    List<List<float>> inputs = new List<List<float>>();

    float timer = 0f;
    bool isJumping = false;
    #endregion

    #region MonoBehaviour
    void Update()
    {
        if (!isRecording) return;

        if (!player.isDead)
        {
            if (timer > 1f / RecordPerSecond)
            {
                Record();
                timer = 0f;

                if (isJumping)
                    isJumping = false;
            }
            else
                timer += Time.deltaTime;
        }

        if (player.isDead && !Saved)
        {
            Saved = true;
            Save();
        }
    }
    #endregion

    #region Methods
    public void SetJumping(InputAction.CallbackContext ctx)
    {
        if (player.isGrounded && ctx.started)
            isJumping = true;
    }
    #endregion

    #region Record & Save
    void Record()
    {
        List<float> input = new List<float>();
        List<int> output = new List<int>();

        output.Add((int)player.movementDirection);
        output.Add(isJumping ? 1 : 0);

        input.Add(player.isGrounded ? 1f : 0f);

        input.Add(end.position.x - transform.position.x);
        input.Add(end.position.y - transform.position.y);

        List<List<GameObject>> objects = vision.objectsInView;
        
        List<int> nbObjects = new List<int>();
        nbObjects.Add(0);

        for (int i = 0; i < objects.Count; i++)
        {
            int count = objects[i].Count;
            nbObjects.Add(count);
            nbObjects[0] += count;

            for (int j = 0; j < count; j++)
            {
                Transform objTransform = objects[i][j].transform;
                input.Add(objTransform.position.x - transform.position.x);
                input.Add(objTransform.position.y - transform.position.y);

                if (i == 0)
                {
                    input.Add(objTransform.lossyScale.x);
                    input.Add(objTransform.lossyScale.y);
                }
            }
        }

        nbObjectPerFrame.Add(nbObjects);
        outputs.Add(output);
        inputs.Add(input);
    }

    void Save()
    {
        string path = Application.dataPath + "/ObjectInputs.txt";

        string data = "";
        for (int i = 0; i < inputs.Count; i++)
        {
            for (int j = 0; j < nbObjectPerFrame[i].Count; j++)
            {
                if (j != 0) data += ';';
                data += nbObjectPerFrame[i][j].ToString();
            }
            data += '\n';
        }

        if (!File.Exists(path))
            File.WriteAllText(path, data);
        else
            File.AppendAllText(path, data);

        path = Application.dataPath + "/DataBase.txt";

        data = "";
        for (int i = 0; i < inputs.Count; i++)
        {
            if (i != 0) data += '\n';

            for (int j = 0; j < nbObjectPerFrame[i].Count; j++)
            {
                if (j != 0) data += ';';
                data += nbObjectPerFrame[i][j].ToString();
            }
            data += '\n';

            for (int j = 0; j < inputs[i].Count; j++)
            {
                if (j != 0) data += ';';
                data += inputs[i][j].ToString();
            }
            data += '\n';

            for (int j = 0; j < outputs[i].Count; j++)
            {
                if (j != 0) data += ';';
                data += outputs[i][j].ToString();
            }
        }

        if (!File.Exists(path))
            File.WriteAllText(path, data);
        else
            File.AppendAllText(path, data);
    }
    #endregion
}
