using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SavedData
{
    public float health;
    public float[] position;

    public SavedData (PlayerController playerController)
    {
        health = playerController.playerHealth;

        position = new float[3];
        position[0] = playerController.transform.position.x;
        position[1] = playerController.transform.position.y;
        position[2] = playerController.transform.position.z;
    }
}
