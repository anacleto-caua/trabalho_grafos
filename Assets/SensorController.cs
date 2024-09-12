using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class SensorController : MonoBehaviour, IComparable<SensorController>
{

    List<SensorController> Network;

    public List<SensorController> ReceivingSensors;
    
    public GameObject AdjSensor;

    public List<SensorController> InRadiusSensors;

    public LineRenderer line;

    #region Params
    public Info Info;

    public int energy;

    public float DistFromMobileSink;
    
    float NetworkMaxRadius = 7.0f;

    public bool isSource = false;
    #endregion Params

    #region BlinkingVars
    //Blinking Interval Const
    public const float  BLINKING_INTERVAL = .3f;
    public float blinkingTimer = 0f;
    public bool isOnBlinking = false;

    public Material DEFAULT_MATERIAL;
    public Material BLINKING_MATERIAL;
    #endregion BlinkingVars

    #region ReCalcNeighborsTimer
    public const float RE_CALC_TIMER = 5f;
    public float ReCalcTimer = RE_CALC_TIMER;
    #endregion ReCalcNeighborsTimer

    void Start()
    {
        //Network = new List<SensorController>();
        //InRadiusSensors = new List<SensorController>();

        DEFAULT_MATERIAL = Resources.Load<Material>("Materials/Yellow");
        BLINKING_MATERIAL = Resources.Load<Material>("Materials/Red");

        line = this.GetComponent<LineRenderer>();
    }

    void Update()
    {
        // Set the number of positions
        line.positionCount = 2;

        // Set the positions (point A and point B)
        line.SetPosition(0, this.transform.position); // Point A
        line.SetPosition(1, AdjSensor.transform.position); // Point B

        // Set the color of the line to red
        line.startColor = Color.red;
        line.endColor = Color.red;

        // Optionally, set the width of the line
        line.startWidth = 0.1f;
        line.endWidth = 0.1f;

        //Debug.DrawLine(transform.position, AdjSensor.transform.position, Color.red);
        //ReCalcHandler();
        
        BlinkHandler();
    }

    #region Handlers
    public void ReCalcHandler()
    {
        if(ReCalcTimer <= 0)
        {
            ReCalcTimer = RE_CALC_TIMER;
            IdentifyNeighborsSensors();
        }
        else
        {
            ReCalcTimer -= Time.deltaTime;
        }
    }
    public void BlinkHandler()
    {
        if (blinkingTimer <= 0f && isOnBlinking)
        {
            blinkingTimer = 0f;
            isOnBlinking = false;

            this.GetComponent<MeshRenderer>().material = DEFAULT_MATERIAL;

            return;
        }

        if (isOnBlinking)
        {
            blinkingTimer -= Time.deltaTime;
        }

    }
    #endregion Handlers

    #region Network
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

    #endregion Network

    public void IdentifyNeighborsSensors()
    {
        InRadiusSensors = new List<SensorController>();

        LayerMask sensorLayer = LayerMask.GetMask("Sensor");

        Collider[] hits = GetObjectsInLayerAroundPoint(transform.position, NetworkMaxRadius, sensorLayer);

        foreach (Collider collider in hits)
        {
            if(collider.gameObject == this.gameObject)
            {
                continue;
            }
            if(collider.TryGetComponent(out MobileSinkSensor sensor))
            {
                AdjSensor = collider.gameObject;
                continue;
            }

            InRadiusSensors.Add(collider.GetComponent<SensorController>());
        }
    }

    public void SetAdjSensor()
    {
        LayerMask sinkLayer = LayerMask.GetMask("MobileSink");

        Collider[] hits = GetObjectsInLayerAroundPoint(transform.position, NetworkMaxRadius, sinkLayer);

        foreach (Collider collider in hits)
        {
            AdjSensor = collider.gameObject;
            return;
        }

        InRadiusSensors.Sort();
        AdjSensor = InRadiusSensors.First().gameObject;
    }

    Collider[] GetObjectsInLayerAroundPoint(Vector3 point, float radius, LayerMask layerMask)
    {
        // Get all colliders within the radius using OverlapSphere
        Collider[] colliders = Physics.OverlapSphere(point, radius);

        // Filter objects by the specified layer
        return System.Array.FindAll(colliders, collider => (layerMask == (layerMask | (1 << collider.gameObject.layer))));
    }

    public void BlinkNeighborSensors()
    {
        foreach (SensorController sensor in InRadiusSensors)
        {
            sensor.SetBlink();
        }
    }

    public void SetBlink()
    {
        if (!isOnBlinking) {
            blinkingTimer = BLINKING_INTERVAL;
            isOnBlinking = true;
        }

        this.GetComponent<MeshRenderer>().material = BLINKING_MATERIAL;
    }

    public void ConfigSensors(List<SensorController> sensors) {
        Network = sensors;
    }

    public void SetAsSource(bool set = true)
    {
        isSource = set;
    }

    public void CalcDistFromMobileSink(Vector3 MobileSinkPos)
    {
        DistFromMobileSink = Vector3.Distance(transform.position, MobileSinkPos);
    }

    public int CompareTo(SensorController other)
    {
        return this.DistFromMobileSink.CompareTo(other.DistFromMobileSink);
    }

}
