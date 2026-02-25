using UnityEngine;
using UnityEngine.InputSystem;

public class FlySwatter : MonoBehaviour
{
    [Header("Настройки")]
    public float rotationSpeed = 1000f;
    public float hitAngle = 45f;
    public float returnSpeed = 5f;

    private Camera mainCamera;
    private Quaternion targetRotation;
    private bool isHitting = false;

    // Для новой Input System
    private Mouse mouse;

    void Start()
    {
        mainCamera = Camera.main;
        targetRotation = Quaternion.identity;

        // Получаем ссылку на мышь
        mouse = Mouse.current;
    }

    void Update()
    {
        // Получение позиции мыши через новую систему
        if (mouse != null)
        {
            Vector2 mouseScreenPos = mouse.position.ReadValue();
            Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(
                new Vector3(mouseScreenPos.x, mouseScreenPos.y, 10)
            );
            mouseWorldPos.z = 0;
            transform.position = mouseWorldPos;

            // Проверка нажатия левой кнопки
            if (mouse.leftButton.wasPressedThisFrame)
            {
                StartHit();
            }
        }

        // Плавный поворот
        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            targetRotation,
            returnSpeed * Time.deltaTime
        );
    }

    void StartHit()
    {
        if (!isHitting)
        {
            isHitting = true;
            targetRotation = Quaternion.Euler(0, 0, hitAngle);
            Invoke("EndHit", 0.1f);
        }
    }

    void EndHit()
    {
        isHitting = false;
        targetRotation = Quaternion.identity;
    }
}