using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private CharacterController controller;
    public float moveSpeed = 5f;
    private Vector2 moveInput;
    private PlayerControls controls;

    // Для камеры
    public Transform cameraHolder;
    public float mouseSensitivity = 100f;
    private float xRotation = 0f;
    private Vector2 lookInput;

    // Для гравитации и прилипания к полу
    private float verticalVelocity = 0f;
    public float gravity = -9.81f;
    public float stickToGroundForce = -2f;
    private bool isGrounded;

    public float jumpForce = 5f;

    // Храним расчитанное движение для использования в разных методах
    private Vector3 horizontalMovement;
    private float verticalMovement;
    private Vector3 finalMovement;

    void Awake()
    {
        controls = new PlayerControls();
        controller = GetComponent<CharacterController>();
    }

    void OnEnable()
    {
        controls.Player.Enable();
        controls.Player.Move.performed += OnMovePerformed;
        controls.Player.Move.canceled += OnMoveCanceled;
        controls.Player.Look.performed += OnLookPerformed;
        controls.Player.Look.canceled += OnLookCanceled;
        controls.Player.Jump.performed += OnJumpPerformed;
    }

    void OnDisable()
    {
        controls.Player.Move.performed -= OnMovePerformed;
        controls.Player.Move.canceled -= OnMoveCanceled;
        controls.Player.Look.performed -= OnLookPerformed;
        controls.Player.Look.canceled -= OnLookCanceled;
        controls.Player.Jump.performed -= OnJumpPerformed;
        controls.Player.Disable();
    }

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        moveInput = Vector2.zero;
    }

    private void OnLookPerformed(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    private void OnLookCanceled(InputAction.CallbackContext context)
    {
        lookInput = Vector2.zero;
    }

    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        if (controller.isGrounded)
        {
            verticalVelocity = jumpForce;
        }
    }

    void Update()
    {
        HandleMouseLook();           // 1. Сначала поворачиваем
        HandleMovement();            // 2. Потом рассчитываем движение от WASD
        HandleGravity();             // 3. Потом применяем гравитацию
        ApplyMovement();             // 4. В конце двигаем
    }

    /// <summary>
    /// Поворот игрока и камеры от мыши
    /// </summary>
    private void HandleMouseLook()
    {
        if (lookInput == Vector2.zero) return;

        // Горизонталь - поворачиваем всего игрока
        float mouseX = lookInput.x * mouseSensitivity * Time.deltaTime;
        transform.Rotate(Vector3.up * mouseX);

        // Вертикаль - поворачиваем только камеру
        float mouseY = lookInput.y * mouseSensitivity * Time.deltaTime;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -30f, 30f);

        if (cameraHolder != null)
        {
            cameraHolder.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }
    }

    /// <summary>
    /// Обработка движения от WASD (горизонтальная составляющая)
    /// </summary>
    private void HandleMovement()
    {
        if (moveInput == Vector2.zero)
        {
            horizontalMovement = Vector3.zero;
            return;
        }

        // Двигаемся относительно поворота ИГРОКА
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;

        // Нормализуем диагональ, чтобы скорость была одинаковой
        if (move.magnitude > 1f)
            move.Normalize();

        horizontalMovement = move * moveSpeed;
    }

    /// <summary>
    /// Применение гравитации и прилипание к полу
    /// </summary>
    private void HandleGravity()
    {
        isGrounded = controller.isGrounded;

        if (isGrounded && verticalVelocity < 0)
        {
            // Прижимаем к полу
            verticalVelocity = stickToGroundForce;
        }
        else
        {
            // Свободное падение
            verticalVelocity += gravity * Time.deltaTime;
        }
    }

    /// <summary>
    /// Финальное движение персонажа
    /// </summary>
    private void ApplyMovement()
    {
        // Собираем горизонтальное движение + вертикаль
        finalMovement = horizontalMovement;
        finalMovement.y = verticalVelocity;

        // Двигаем
        controller.Move(finalMovement * Time.deltaTime);
    }
}