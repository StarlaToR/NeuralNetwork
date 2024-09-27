using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleAI : Character
{
    int inputAxis = -1;

    // Update is called once per frame
    void Update()
    {
        Move(inputAxis);
    }

    private void OnCollisionEnter(Collision collision)
    {
        int layer = collision.gameObject.layer;

        if (layer == 3)
        {
            collision.gameObject.GetComponent<Character>().Die();
        }
        else if (layer == 6 || layer == 7 || layer == 9)
        {
            inputAxis *= -1;
        }
        else if (layer == 0 && transform.position.y < collision.transform.position.y + collision.transform.lossyScale.y / 2f)
        {
            inputAxis *= -1;
        }
    }
}
