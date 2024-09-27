using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] CharacterSpawner spawner;

    [SerializeField] float minX = -10f;
    [SerializeField] float maxX = 100f;

    // Update is called once per frame
    void Update()
    {
        Vector3 currentPos = spawner.GetFurthestCharacterPosition();
        float currentX = currentPos.x;
        float currentY = currentPos.y;

        if (currentX < minX)
            currentX = minX;
        else if (currentX > maxX)
            currentX = maxX;

        transform.position = new Vector3(currentX, currentY + 3f, transform.position.z);
    }
}
