using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraParallax : MonoBehaviour
{
    private struct InputData
    {
        public float vertical;
        public float horizontal;
    }

    [SerializeField]
    private Transform m_CameraParent;
    [SerializeField]
    private float smoothSpeed = 5f;
    [SerializeField]
    private Vector2 m_XAngleMinMax = new Vector2(-3f, 3f);
    [SerializeField]
    private Vector2 m_YAngleMinMax = new Vector2(-3f, 3f);


    private void Start()
    {
        Input.gyro.enabled = true;
    }

    private void Update()
    {
        InputData inputData = GetInputData();

        Vector3 cameraParentEulerAngles = FixEulerAngles(m_CameraParent.localEulerAngles);
        cameraParentEulerAngles.y += inputData.horizontal;
        cameraParentEulerAngles.y = Mathf.Clamp(cameraParentEulerAngles.y, m_YAngleMinMax.x, m_YAngleMinMax.y);
        m_CameraParent.localEulerAngles = cameraParentEulerAngles;

        Vector3 thisEulerAngles = FixEulerAngles(transform.localEulerAngles);
        thisEulerAngles.x += inputData.vertical;
        thisEulerAngles.x = Mathf.Clamp(thisEulerAngles.x, m_XAngleMinMax.x, m_XAngleMinMax.y);
        transform.localEulerAngles = thisEulerAngles;
    }

    private InputData GetInputData()
    {
        InputData data = new InputData();

#if UNITY_EDITOR
        data.horizontal = Input.GetAxis("Horizontal") * 5f * smoothSpeed * Time.deltaTime;
        data.vertical = Input.GetAxis("Vertical") * 5f * smoothSpeed * Time.deltaTime;
#else
        data.horizontal = -Input.gyro.rotationRateUnbiased.y * smoothSpeed * Time.deltaTime;
        data.vertical = -Input.gyro.rotationRateUnbiased.x * smoothSpeed * Time.deltaTime;
#endif

        return data;
    }

    private Vector3 FixEulerAngles(Vector3 euler)
    {
        float x = FixAngle(euler.x);
        float y = FixAngle(euler.y);
        float z = FixAngle(euler.z);
        Vector3 fixedEuler = new Vector3(x, y, z);
        return fixedEuler;
    }

    private float FixAngle(float angle)
    {
        angle = (angle > 180) ? angle - 360 : angle;
        return angle;
    }
}
