using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseMove : MonoBehaviour
{
    float rotatuionX = 0f;
    float rotatuionY = 0f;
    
    public float sensitivity = 15f;

    public bool Rotate = false;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            rotatuionY += Input.GetAxis("Mouse X") * sensitivity;
            rotatuionX -= Input.GetAxis("Mouse Y") * sensitivity;
            transform.localEulerAngles = new Vector3(rotatuionX, rotatuionY, 0f);
        }

        float horizontalInput = Input.GetAxis("Horizontal"); // Valor de entrada horizontal (A y D)
        float verticalInput = Input.GetAxis("Vertical"); // Valor de entrada vertical (W y S)

        Vector3 movement = new Vector3(horizontalInput, 0f, verticalInput) * 10f;
        transform.Translate(movement);
    }
}
