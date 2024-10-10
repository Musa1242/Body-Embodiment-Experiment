using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnSpeed : MonoBehaviour
{
    public float minVelocityX;
    public float maxVelocityX;
    public float minVelocityY;
    public float maxVelocityY;
    public float minVelocityZ;
    public float maxVelocityZ;
    public float minAngularVelocityX;
    public float maxAngularVelocityX;
    public float minAngularVelocityY;
    public float maxAngularVelocityY;
    public float minAngularVelocityZ;
    public float maxAngularVelocityZ;
    private float randomVelocityX;
    private float randomVelocityY;
    private float randomVelocityZ;
    private float randomAngularVelocityX;
    private float randomAngularVelocityY;
    private float randomAngularVelocityZ;
    


    // Start is called before the first frame update
    void Start()
    {
        randomVelocityX = Random.Range(minVelocityX, maxVelocityX);
        randomVelocityY = Random.Range(minVelocityY, maxVelocityY);
        randomVelocityZ = Random.Range(minVelocityZ, maxVelocityZ);

        randomAngularVelocityX = Random.Range(minAngularVelocityX, maxAngularVelocityX);
        randomAngularVelocityY = Random.Range(minAngularVelocityY, maxAngularVelocityY);
        randomAngularVelocityZ = Random.Range(minAngularVelocityZ, maxAngularVelocityZ);

        GetComponent<Rigidbody>().angularVelocity = new Vector3(randomAngularVelocityX, randomAngularVelocityY, randomAngularVelocityZ);
        GetComponent<Rigidbody>().velocity = new Vector3(randomVelocityX, randomVelocityY, randomVelocityZ);

    }

}
