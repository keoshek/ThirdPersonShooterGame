using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyCamera : MonoBehaviour
{
    public float rotationSensitivity;
    float RotationMin = -40f;
    float RotationMax = 80f;
    float smoothTime = 0.12f;
    public float positionRate;

    public float yAxis;
    public float xAxis;
    Vector3 targetRotation;
    Vector3 currentVel;

    public Transform target;
    GameObject soldier;
    private bool enableMobileInputs;
    public FixedTouchField fixedTouchField;
    // Start is called before the first frame update
    void Start()
    {
        soldier = GameObject.Find("Soldier");
    }

    // Update is called once per frame
    void LateUpdate()
    {
        enableMobileInputs = soldier.GetComponent<PlayerMovement>().enableMobileInputs;

        if (enableMobileInputs)
        {
            rotationSensitivity = 0.2f;
            yAxis += fixedTouchField.TouchDist.x * rotationSensitivity;
            xAxis -= fixedTouchField.TouchDist.y * rotationSensitivity;
        }
        else
        {
            rotationSensitivity = 8f;
            yAxis += Input.GetAxis("Mouse X") * rotationSensitivity;
            xAxis -= Input.GetAxis("Mouse Y") * rotationSensitivity;
        }

        xAxis = Mathf.Clamp(xAxis, RotationMin, RotationMax);

        targetRotation = Vector3.SmoothDamp(targetRotation, new Vector3(xAxis, yAxis), ref currentVel, smoothTime);
        transform.eulerAngles = targetRotation;
        transform.position = target.position - transform.forward * positionRate;
    }

    public void ChangePositionRate(float rate)
    {
        positionRate = rate;
    }
}
