using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
    private float LookSensitivity = 1 / 5f;
    private Quaternion _originalRotation;
    float _mouseX;
    float _mouseY;
    float _minYRot = -60f;
    float _maxYRot = 60f;
    const int NumEffects = 4;
    // Use this for initialization
    void Start ()
    {
        _originalRotation = Quaternion.AngleAxis(180, Vector3.up);
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKey(KeyCode.W))
        {
            transform.position += transform.forward;
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.position -= transform.right;
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.position -= transform.forward;
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.position += transform.right;
        }
	    if (Input.GetMouseButton(1))
	    {
            RotateCamera();
        }
    }

    void RotateCamera()
    {
        _mouseX += Input.GetAxis("Mouse X") * LookSensitivity * 10;
        _mouseY += Input.GetAxis("Mouse Y") * LookSensitivity * 10;

        _mouseY = Mathf.Clamp(_mouseY, _minYRot, _maxYRot);

        Quaternion rotX = Quaternion.AngleAxis(_mouseX, Vector3.up);
        Quaternion rotY = Quaternion.AngleAxis(_mouseY, -Vector3.right);

        transform.localRotation = _originalRotation * rotX * rotY;
    }
}
