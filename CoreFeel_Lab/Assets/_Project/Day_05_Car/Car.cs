using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Car : MonoBehaviour
{
    public float speed = 5f;
    public float turnSpeed = 100f;    // Скорость поворота

    public void Update()
    {
        if (Keyboard.current.wKey.isPressed)
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }
        if (Keyboard.current.sKey.isPressed)
        {
            transform.Translate(Vector3.back * speed * Time.deltaTime);
        }
        if (Keyboard.current.aKey.isPressed)
        {
            transform.Rotate(Vector3.left * turnSpeed * Time.deltaTime);
        }
        if (Keyboard.current.dKey.isPressed)
        {
            transform.Rotate(Vector3.right * turnSpeed * Time.deltaTime);
        }
    }
}
