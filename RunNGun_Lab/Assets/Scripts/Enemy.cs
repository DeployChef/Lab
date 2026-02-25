using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private Transform playerTransform; 
    [SerializeField] private float speed = 3f;
    private CharacterController controller;

    // Для гравитации и прилипания к полу
    private float verticalVelocity = 0f;
    public float gravity = -9.81f;
    public float stickToGroundForce = -2f;
    private bool isGrounded;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
    
        if (playerTransform)
        {
            HandleGravity();

            Vector3 direction = playerTransform.position - transform.position;

            var dir = direction.normalized;
            var move = dir * speed;

            move.y = verticalVelocity;

            controller.Move(move * Time.deltaTime);

            if (dir.magnitude > 0.1f)
            {
                transform.rotation = Quaternion.LookRotation(direction);
            }
        }
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

    public void SetPlayerTransform(Transform pTransform)
    {
        playerTransform = pTransform;
    }
}
