using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CharacterSpawner : MonoBehaviour
{
    #region Spawner Variables
    [SerializeField] GameObject player;
    [SerializeField] GameObject ai;
    [SerializeField] Transform levelEnd;

    List<GameObject> characters = new List<GameObject>();
    AIController lastSpawnedAI;
    GameObject spawnedPlayer;

    bool PlayerSpawned = false;
    bool AISpawned = false;
    #endregion

    #region UI Variables

    [SerializeField] TMP_InputField inputField;

    #endregion

    #region Spawner
    public void SpawnPlayer()
    {
        if (PlayerSpawned) return;

        spawnedPlayer = Instantiate(player, transform.position, Quaternion.identity);
        spawnedPlayer.GetComponent<DataRecorder>().end = levelEnd;
        spawnedPlayer.GetComponent<Player>().respawnPoint = transform.position;
        characters.Add(spawnedPlayer);

        PlayerSpawned = true;
    }

    public void SpawnAI()
    {
        if (AISpawned) return;

        GameObject gO = Instantiate(ai, transform.position, Quaternion.identity);
        lastSpawnedAI = gO.GetComponent<AIController>();
        lastSpawnedAI.levelEnd = levelEnd;
        gO.GetComponent<Character>().respawnPoint = transform.position;
        characters.Add(gO);

        AISpawned = true;
    }
    #endregion

    #region UI Methods

    public void TrainAI()
    {
        if (!AISpawned) return;

        lastSpawnedAI.trainingIteration = int.Parse(inputField.text);
        lastSpawnedAI.Train();
    }

    public void UpdateAIPlayingState()
    {
        if (!AISpawned) return;

        Character character = lastSpawnedAI.GetComponent<Character>();
        if (character.isDead)
        {
            lastSpawnedAI.isPlaying = true;
            character.isDead = true;
        }
        else
        {
            lastSpawnedAI.isPlaying = !lastSpawnedAI.isPlaying;
        }

    }

    public void CreateAINetwork()
    {
        if (!AISpawned || lastSpawnedAI.networkCreated) return;

        lastSpawnedAI.Create();
    }

    public void LoadTrainedAI()
    {
        if (!AISpawned) return;

        lastSpawnedAI.Load();
    }

    public void SaveTrainedAI()
    {
        if (!AISpawned) return;

        lastSpawnedAI.Save();
    }

    public void RecordPlayerGameplay()
    {
        if (!PlayerSpawned) return;

        spawnedPlayer.GetComponent<DataRecorder>().isRecording = true;
    }

    public void RevivePlayer()
    {
        if (!PlayerSpawned) return;

        Player character = spawnedPlayer.GetComponent<Player>();

        if (character.isDead) character.isDead = false; 
    }

    #endregion

    #region Methods
    public Vector3 GetFurthestCharacterPosition()
    {
        if (characters.Count == 0)
            return Vector3.zero;

        Vector3 position = new Vector3(-20000, 0, 0);

        foreach (var character in characters)
        {
            if (character != null && character.transform.position.x > position.x)
                position = character.transform.position;
        }

        return position;
    }
    #endregion
}
