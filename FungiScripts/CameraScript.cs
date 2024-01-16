using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    private Camera cam;
    
    void Start()
    {
        cam = Camera.main;
    }
    void Update()
    {
        var x = Input.GetAxis("Horizontal") * Time.deltaTime * 50.0f;
        var y = Input.GetAxis("Vertical") * Time.deltaTime * 50.0f;
        var z = Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * 5000.0f;
        
        var orthographicSize = cam.orthographicSize;
        x *= orthographicSize / 10;
        y *= orthographicSize / 10;

        x = x switch
        {
            > 10 => 10,
            < -10 => -10,
            _ => x
        };
        y = y switch
        {
            > 10 => 10,
            < -10 => -10,
            _ => y
        };

        transform.Translate(x, y, 0);
        if (z > 50) z = 50;
        if (z < -50) z = -50;
        cam.orthographicSize -= z;
        if (cam.orthographicSize < 10)
        {
            cam.orthographicSize = 10;
        }
    }
}
