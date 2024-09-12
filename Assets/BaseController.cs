using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;

public class BaseController : MonoBehaviour
{
    public List<SensorController> Network = new List<SensorController>();

    public SensorController MobileSink;

    #region SourceRaycastVars
    float coneAngle = 180f;  // The total angle of the cone (in degrees)
    int rayCount = 70;      // Number of rays to cast within the cone
    float rayDistance = 10f;  // The maximum distance of each ray
    LayerMask sensorLayer;     // LayerMask to specify what layers to detect
    #endregion SourceRaycastVars

    public List<SensorController> SourceSensors = new List<SensorController>();

    // Start is called before the first frame update
    void Start()
    {
        sensorLayer = LayerMask.GetMask("Sensor");

        Network = new List<SensorController>();
        SourceSensors = new List<SensorController>();

        
        #region GetSensors
        Collider[] colliders = GetObjectsInLayerAroundPoint(Vector3.zero, 2000f, sensorLayer);

        foreach (Collider collider in colliders)
        {
            Network.Add(collider.GetComponent<SensorController>());
        }

        foreach (Collider collider in colliders)
        {
            collider.GetComponent<SensorController>().ConfigSensors(Network);
        }
        #endregion GetSensors

        DefineSources();
        MakeRoutes();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            MakeRoutes();
        }
    }
    void MakeRoutes()
    {
        foreach (SensorController sensor in Network)
        {
            sensor.IdentifyNeighborsSensors();
            sensor.CalcDistFromMobileSink(MobileSink.transform.position);
            sensor.BlinkNeighborSensors();
        }

        foreach (SensorController sensor in Network)
        {
            sensor.SetAdjSensor();
        }
    }

    // A source will be every vertex wich have no vertex on the oposite side of the mobile sink
    void DefineSources()
    {
        SourceSensors.Clear();
        foreach (SensorController sensor in Network)
        {
            sensor.SetAsSource(false);

            // Calculate dir from sink to sensor
            Vector3 direction = sensor.transform.position - MobileSink.transform.position;
            direction = direction.normalized;

            int hits = 0;

            for (int i = 0; i < rayCount; i++)
            {
                // Calculate the angle for this ray relative to the forward direction
                float angle = Mathf.Lerp(-coneAngle / 2, coneAngle / 2, (float)i / (rayCount - 1));

                // Calculate the direction of the ray based on the angle
                Quaternion rotation = Quaternion.Euler(0, angle, 0);
                Vector3 rayDirection = rotation * direction;

                /*
                 * Debug only line pls ignore
                Debug.DrawLine(
                    sensor.transform.position + (rayDirection * 2f), 
                    sensor.transform.position + (rayDirection * rayDistance), 
                    Color.red);
                */

                // If it doesnt hit anything its a source sensor
                RaycastHit hit;
                if (
                    Physics.Raycast(
                        sensor.transform.position + (rayDirection * 1f), 
                        rayDirection, 
                        out hit, 
                        rayDistance,
                        sensorLayer)
                    ){
                    if(sensor != hit.collider.GetComponent<SensorController>())
                    {
                        hits += 1;
                        // If its a sensor we can just break out code
                        break;
                    }

                }
            }

            if (hits == 0)
            {
                sensor.SetAsSource();
                SourceSensors.Add(sensor);
            }
        }
    }

    Collider[] GetObjectsInLayerAroundPoint(Vector3 point, float radius, LayerMask layerMask)
    {
        // Get all colliders within the radius using OverlapSphere
        Collider[] colliders = Physics.OverlapSphere(point, radius);

        // Filter objects by the specified layer
        return System.Array.FindAll(colliders, collider => (layerMask == (layerMask | (1 << collider.gameObject.layer))));
    }

    private void OnDrawGizmos()
    {
    }
}
