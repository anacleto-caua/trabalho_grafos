using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseController : MonoBehaviour
{
    List<SensorController> sensorsList = new List<SensorController>();

    // Start is called before the first frame update
    void Start()
    {
        sensorsList = new List<SensorController>();

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
        
    }

    Collider[] GetObjectsInLayerAroundPoint(Vector3 point, float radius, LayerMask layerMask)
    {
        // Get all colliders within the radius using OverlapSphere
        Collider[] colliders = Physics.OverlapSphere(point, radius);

        // Filter objects by the specified layer
        return System.Array.FindAll(colliders, collider => (layerMask == (layerMask | (1 << collider.gameObject.layer))));
    }

}
