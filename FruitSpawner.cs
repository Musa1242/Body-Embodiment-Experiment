using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FruitSpawner : MonoBehaviour
{
    public List<GameObject> ObjectsToSpawn = new List<GameObject>();
    public bool IsTimer;
    public float TimeToSpawn;
    private float CurrentTimeToSpawn;
    public bool isRandomized;
    public float interval = 10;

    // Start is called before the first frame update
    void Start()
    {
        CurrentTimeToSpawn = TimeToSpawn;
    }

    // Update is called once per frame
    void Update()
    {
        if (IsTimer)
        {
            UpdateTimer();
        }
        
    }

    private void UpdateTimer()
    {
        if (CurrentTimeToSpawn > 0)
        {
            CurrentTimeToSpawn -= Time.deltaTime;
        }
        else
        {
            SpawnObject();
            CurrentTimeToSpawn = TimeToSpawn;
        }
        
    }
    public void SpawnObject()
    {
        int index = isRandomized ? Random.Range(0, ObjectsToSpawn.Count) : 0;
        if(ObjectsToSpawn.Count > 0)
        {
            GameObject go = Instantiate(ObjectsToSpawn[index], transform.position, transform.rotation);
            Destroy(go, interval);
            
        }
        
        
    }
}
