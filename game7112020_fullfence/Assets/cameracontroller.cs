using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameracontroller : MonoBehaviour
{
    public Transform playerTarget;
    public float moveSpeed;
    public float rotationSpeed;
    Quaternion startRotation;
    Vector3 offset;
    private void FixedUpdate()
    {
        transform.position =Vector3.Lerp(transform.position, playerTarget.position +transform.rotation * offset, moveSpeed*Time.fixedDeltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, playerTarget.rotation * startRotation, rotationSpeed*Time.fixedDeltaTime);

    }
    // Start is called before the first frame update
    void Start()
    {
        offset = transform.position - playerTarget.position;
        startRotation = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
