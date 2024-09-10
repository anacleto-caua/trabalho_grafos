using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class SensorController : MonoBehaviour
{
    List<SensorController> Network;

    public int energy;
    
    public SensorController AdjSensor;

    public List<SensorController> ReceivingSensors;

    public Info Info;

    public List<SensorController> InRadiusSensor;

    public float DistFromMobileSink;

    void Start()
    {
        Network = new List<SensorController>();
        InRadiusSensor = new List<SensorController>();
    }

    void Update()
    {
        
    }

    public void ForwardData(Info outerInfo)
    {
        if(this.Info != null)
        {
            Info = outerInfo;
        }
    }

    public float AverageNetworkEnergy()
    {
        float totalE = 0.0f;
        
        foreach (SensorController sensor in Network)
        {
            totalE += sensor.energy;
        }

        return totalE;
    }
}
