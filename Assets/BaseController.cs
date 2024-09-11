using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;

public class BaseController : MonoBehaviour
{
    List<SensorController> sensorsList = new List<SensorController>();

    public SensorController MobileSink;

    float coneAngle = 45f;  // The total angle of the cone (in degrees)
    int rayCount = 20;      // Number of rays to cast within the cone
    float rayDistance = 10f;  // The maximum distance of each ray
    LayerMask sensorLayer;     // LayerMask to specify what layers to detect

    public List<SensorController> SourceSensors = new List<SensorController>();

    // Start is called before the first frame update
    void Start()
    {
        sensorLayer = LayerMask.GetMask("Sensor");

        sensorsList = new List<SensorController>();
        SourceSensors = new List<SensorController>();

        Collider[] colliders = GetObjectsInLayerAroundPoint(Vector3.zero, 2000f, LayerMask.GetMask("Sensor"));

        foreach (Collider collider in colliders)
        {
            sensorsList.Add(collider.GetComponent<SensorController>());
        }

        foreach (Collider collider in colliders)
        {
            collider.GetComponent<SensorController>().ConfigSensors(sensorsList);
        }

    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.J))
        {
            DefineSources();
        }
    }
    void EdmondsKarp()
    {
        foreach (SensorController sensor in sensorsList)
        { 
        }
    }

    // A source will be every vertex wich have no vertex on the oposite side of the mobile sink
    void DefineSources()
    {
        foreach(SensorController sensor in sensorsList) {
            // Calculate dir from sink to sensor
            Vector3 direction = sensor.transform.position - MobileSink.transform.position;
            direction = direction.normalized;

            for (int i = 0; i < rayCount; i++)
            {
                // Calculate the angle for this ray relative to the forward direction
                float angle = Mathf.Lerp(-coneAngle / 2, coneAngle / 2, (float)i / (rayCount - 1));

                // Calculate the direction of the ray based on the angle
                Quaternion rotation = Quaternion.Euler(0, angle, 0);
                Vector3 rayDirection = rotation * direction;

                // Perform the raycast
                RaycastHit hit;
                if (Physics.Raycast(sensor.transform.position, rayDirection, out hit, rayDistance, sensorLayer))
                {
                    // Do something if the ray hits an object
                    Debug.DrawRay(sensor.transform.position, rayDirection * hit.distance, Color.red);
                    Debug.Log("Hit object: " + hit.collider.name);
                }
                else
                {
                    // Draw the ray in green if nothing was hit
                    sensor.SetAsSource();
                    SourceSensors.Add(sensor);
                }
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

}
