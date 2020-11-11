using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class car : MonoBehaviour
{
    public AxleInfo[] carAxis = new AxleInfo[2];
    public float carSpeed;
    public float steerAngle;
    float horInput;
    float vertInput;
    public Transform centerOfMass;
  //  Rigidbody rb;

    void Start()
    {
     /*  rb = GetComponent<Rigidbody>();
        rb.centerOfMass = centerOfMass.localPosition;*/
    }
    private void FixedUpdate()
    {
        horInput = Input.GetAxis("Horizontal");
        vertInput = Input.GetAxis("Vertical");
        Accelerate();
    }
    void Accelerate()
    {
        foreach(AxleInfo axle in carAxis)
        {
            if(axle.steering)
            {
                axle.rightWheel.steerAngle = steerAngle * horInput;
                axle.leftWheel.steerAngle = steerAngle * horInput;
            }
            if (axle.motor)
            {
                axle.rightWheel.motorTorque = carSpeed * vertInput;
                axle.leftWheel.motorTorque = carSpeed * vertInput;
            }
            VisualWheelsToColliders(axle.rightWheel, axle.visRightWheel);
            VisualWheelsToColliders(axle.leftWheel, axle.visLeftWheel);
        }
        void VisualWheelsToColliders(WheelCollider col,Transform visWheel)
        {
            Vector3 position;
            Quaternion rotation;

            col.GetWorldPose(out position, out rotation);

            visWheel.position = position;
            visWheel.rotation = rotation;
        }
    }
    [System.Serializable]
    public class AxleInfo
    {
        public WheelCollider rightWheel;
        public WheelCollider leftWheel;

        public Transform visRightWheel;
        public Transform visLeftWheel;

        public bool steering;
        public bool motor;
    }
}