using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    //Inspector exposed
    [SerializeField] private float mouseSensitivity = 100.0f;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Camera gunCamera;
    [SerializeField] private AudioListener mainAudioListener;

    //private    
    private float xRotation = 0f;

    //public
    public Transform playerBody;  
    
    void Update()
    {      

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        playerBody.Rotate(Vector3.up * mouseX, Space.World);
    }
}
