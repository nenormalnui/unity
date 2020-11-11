using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour {

	public AxleInfo [] carAxis = new AxleInfo[2];
	public WheelCollider [] wheelColliders;
	public float carSpeed;
	public float steerAngle;
	public Transform centerOfMass;
	[Range(0,1)]
	public float steerHelpValue = 0;
	public float nitroPower;
	public GameObject nitroEffects;

	[Header("For Smoke From Tires")]
	public float minSpeedForSmoke;
	public float minAngleForSmoke;
	public ParticleSystem [] tireSmokeEffects;

	public Transform helm;

	public Vector3 additionalWheelAngle;

	float horInput;
	float vertInput;

	Rigidbody rb;
	bool onGround;
	float lastYRotation;
	Quaternion startHelmRotation;

	void Start()
	{
		rb = GetComponent<Rigidbody>();
		rb.centerOfMass = centerOfMass.localPosition;
		startHelmRotation = helm.localRotation;
	}

	void FixedUpdate()
	{
		horInput = Input.GetAxis("Horizontal"); 
		vertInput = Input.GetAxis("Vertical");

		CheckOnGround();
		Accelerate();
		ManageNitro();
		ManageHardBreak();
		EmitSmokeFromTires();

		SteerHelpAssist();
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
			if(axle.motor)
			{
				axle.rightWheel.motorTorque = carSpeed * vertInput;
				axle.leftWheel.motorTorque = carSpeed * vertInput;
			}
			VisualWheelsToColliders(axle.rightWheel, axle.visRightWheel);
			VisualWheelsToColliders(axle.leftWheel, axle.visLeftWheel);
		}
		helm.localRotation = startHelmRotation * Quaternion.Euler(Vector3.forward * 180 * horInput);
	}

	void VisualWheelsToColliders(WheelCollider col, Transform visWheel)
	{
		Vector3 position;
		Quaternion rotation;

		col.GetWorldPose(out position, out rotation);

		visWheel.position = position;
		visWheel.rotation = rotation * Quaternion.Euler(additionalWheelAngle);
	}

	void SteerHelpAssist()
	{
		if(!onGround)
			return;

		if(Mathf.Abs(transform.rotation.eulerAngles.y - lastYRotation) < 10f)
		{
			float turnAdjust = (transform.rotation.eulerAngles.y - lastYRotation) * steerHelpValue;
			Quaternion rotateHelp = Quaternion.AngleAxis(turnAdjust, Vector3.up);
			rb.velocity = rotateHelp * rb.velocity;
		}
		lastYRotation = transform.rotation.eulerAngles.y;
	}

	void CheckOnGround()
	{
		onGround = true;
		foreach(WheelCollider wheelCol in wheelColliders)
		{
			if(!wheelCol.isGrounded)
				onGround = false;
		}	
	}

	void ManageNitro()
	{
		if(Input.GetKey(KeyCode.LeftShift) && vertInput > 0.01f)
		{
			rb.AddForce(transform.forward * nitroPower);
			nitroEffects.SetActive(true);
		}
		else
		{
			if(nitroEffects.activeSelf)
				nitroEffects.SetActive(false);
		}
	}

	void ManageHardBreak()
	{
		foreach(AxleInfo axle in carAxis)
		{
			if(Input.GetKey(KeyCode.Space))
			{
				axle.rightWheel.brakeTorque = 50000;
				axle.leftWheel.brakeTorque = 50000;
			}
			else
			{
				axle.rightWheel.brakeTorque = 0;
				axle.leftWheel.brakeTorque = 0;
			}
		}
	}

	void EmitSmokeFromTires()
	{
		if(rb.velocity.magnitude > minSpeedForSmoke)
		{
			float angle = Quaternion.Angle(Quaternion.LookRotation(rb.velocity, Vector3.up), Quaternion.LookRotation(transform.forward, Vector3.up));
			if(angle > minAngleForSmoke && angle < 160 && onGround)
				SwitchSmokeParticles(true);
			else
				SwitchSmokeParticles(false);
		}
		else
			SwitchSmokeParticles(false);
	}

	void SwitchSmokeParticles(bool _enable)
	{
		foreach(ParticleSystem ps in tireSmokeEffects)
		{
			ParticleSystem.EmissionModule psEm = ps.emission;
			psEm.enabled = _enable;
		}
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
