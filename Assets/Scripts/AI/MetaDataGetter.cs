using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetaDataGetter : MonoBehaviour
{
    [SerializeField] TextAsset objectList; // Text file which contains the list of number of each type of object i detected on each frame during playing

    public int NbMaxObject = 0;
    public int NbMaxPlatform = 0;
    public int NbMaxEnemy = 0;
    public int NbMaxKillzone = 0;

    public bool isDone = false;

    // Start is called before the first frame update
    void Start()
    {
        if (objectList == null) return;

        string content = objectList.text;

        string[] splittedContent = content.Split("\n");

        for (int i = 0; i < splittedContent.Length; i++)
        {
            string line = splittedContent[i];

            string[] splittedLine = line.Split(';');

            int objectNb = int.Parse(splittedLine[0]);
            int platformNb = int.Parse(splittedLine[1]);
            int enemyNb = int.Parse(splittedLine[2]);
            int kzNb = int.Parse(splittedLine[3]);

            if (objectNb > NbMaxObject)
                NbMaxObject = objectNb;
            if (platformNb > NbMaxPlatform)
                NbMaxPlatform = platformNb;
            if (enemyNb > NbMaxEnemy)
                NbMaxEnemy = enemyNb;
            if (kzNb > NbMaxKillzone)
                NbMaxKillzone = kzNb;
        }

        isDone = true;
    }
}
