using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SensorController))]
public class SensorScriptEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw the default inspector to keep serialized fields visible
        DrawDefaultInspector();

        // Get a reference to the target object (MyScript)
        SensorController sensorCtrl = (SensorController)target;

        // Add a button to increment the number
        if (GUILayout.Button("Blink Neighboors"))
        {
            sensorCtrl.BlinkNeighborSensors(); // Call the method when the button is clicked
        }
    }
}
