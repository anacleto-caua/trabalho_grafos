using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Info
{
    public int infoCounter;

    public int infoMergesCount;
    
    public List<SensorController> sensorsPassed;

    public Info()
    {
        sensorsPassed = new List<SensorController>();
        infoCounter = 0;
        infoMergesCount = 0;
    }

    public void merge(Info other)
    {
        infoCounter += other.infoCounter;
        infoMergesCount += other.infoMergesCount;
        sensorsPassed.AddRange(other.sensorsPassed);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
