using System.Collections;
using System.Collections.Generic;
using System.Threading;
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

    public List<SensorController> InRadiusSensors;

    public float DistFromMobileSink;

    public float NetworkMaxRadius = 5.0f;

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
        Network = new List<SensorController>();
        InRadiusSensors = new List<SensorController>();
    }

    void Update()
    {


        ReCalcHandler();
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

            InRadiusSensors.Add(collider.GetComponent<SensorController>());
        }
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
        DEFAULT_MATERIAL = Resources.Load<Material>("Materials/Yellow");
        BLINKING_MATERIAL = Resources.Load<Material>("Materials/Red");
        
        if (!isOnBlinking) {
            blinkingTimer = BLINKING_INTERVAL;
            isOnBlinking = true;
        }

        this.GetComponent<MeshRenderer>().material = BLINKING_MATERIAL;

    }

    public void ConfigSensors(List<SensorController> sensors) {
        Network = sensors;
    }

}
