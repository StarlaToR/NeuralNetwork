using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterVision : MonoBehaviour
{
    public List<List<GameObject>> objectsInView;
    public List<GameObject> objects;

    // Start is called before the first frame update
    void Start()
    {
        objectsInView = new List<List<GameObject>>();
        for (int i = 0; i < 3; i++)
        {
            objectsInView.Add(new List<GameObject>());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 0)
            objectsInView[0].Add(other.gameObject);
        else if (other.gameObject.layer == 7)
            objectsInView[1].Add(other.gameObject);
        else if (other.gameObject.layer == 6)
            objectsInView[2].Add(other.gameObject);

        objects.Add(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 0)
            objectsInView[0].Remove(other.gameObject);
        else if (other.gameObject.layer == 7)
            objectsInView[1].Remove(other.gameObject);
        else if (other.gameObject.layer == 6)
            objectsInView[2].Remove(other.gameObject);

        objects.Remove(other.gameObject);
    }
}
