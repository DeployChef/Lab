using DG.Tweening.Core.Easing;
using System.Collections;
using UnityEngine;

public class Fly : MonoBehaviour
{
    [Header("Настройки движения")]
    public float flySpeed = 2f;
    public float changeDirectionTime = 2f;

    [Header("Зоны (RectTransform)")]
    public RectTransform flyZone;      // Зона где летают мухи
    public RectTransform tableZone;    // Зона стола куда садятся

    [Header("Настройки посадки")]
    public float landingChance = 0.3f;
    public float landingCheckInterval = 3f;
    public float sitDuration = 5f;     // Сколько сидит муха
    public float landingSpeed = 1.5f;   // Скорость снижения при посадке

    private Vector2 randomDirection;
    private float directionTimer;
    private float landingTimer;
    private bool isSitting = false;
    private bool isLanding = false;      // Новый статус - в процессе посадки
    private Vector2 targetLandingPoint;  // Точка на столе куда летим
    private float sitTimer;

    private SpriteRenderer spriteRenderer;
    private Collider2D flyCollider;
    private FlyGameManager gameManager;

    [Header("Gizmos")]
    public bool showGizmos = true;
    public Color flyPathColor = Color.yellow;
    public Color landingPathColor = Color.magenta;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        flyCollider = GetComponent<Collider2D>();
        gameManager = FindObjectOfType<FlyGameManager>();

        SetRandomDirection();
    }

    void Update()
    {
        if (isLanding)
        {
            // Летим к точке посадки
            Vector2 directionToTable = (targetLandingPoint - (Vector2)transform.position).normalized;
            transform.Translate(directionToTable * landingSpeed * Time.deltaTime);

            // Проверяем, долетели ли до стола
            float distanceToTarget = Vector2.Distance(transform.position, targetLandingPoint);
            if (distanceToTarget < 0.1f)
            {
                // Приземлились!
                isLanding = false;
                isSitting = true;
                sitTimer = 0;
                transform.position = targetLandingPoint;

                if (spriteRenderer != null)
                    spriteRenderer.color = Color.red;

                Debug.Log("Муха села на стол");
            }
        }
        else if (!isSitting)
        {
            // Обычный полет
            directionTimer += Time.deltaTime;
            landingTimer += Time.deltaTime;

            if (directionTimer >= changeDirectionTime)
                SetRandomDirection();

            // Движение
            Vector3 newPosition = transform.position + (Vector3)randomDirection * flySpeed * Time.deltaTime;

            // Проверяем границы flyZone
            if (IsPointInRectTransform(newPosition, flyZone))
            {
                transform.position = newPosition;
            }
            else
            {
                // Отскок от границ
                BounceFromBoundary();
            }

            // Проверка на посадку
            if (landingTimer >= landingCheckInterval)
            {
                TryToLand();
                landingTimer = 0;
            }
        }
        else
        {
            // Муха сидит на столе
            sitTimer += Time.deltaTime;

            if (sitTimer >= sitDuration)
            {
                TakeOff();
            }
        }
    }

    bool IsPointInRectTransform(Vector3 point, RectTransform rectTransform)
    {
        if (rectTransform == null) return true;

        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);

        return point.x >= corners[0].x && point.x <= corners[2].x &&
               point.y >= corners[0].y && point.y <= corners[2].y;
    }

    Vector2 GetRandomPointInRectTransform(RectTransform rectTransform)
    {
        if (rectTransform == null) return transform.position;

        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);

        return new Vector2(
            Random.Range(corners[0].x, corners[2].x),
            Random.Range(corners[0].y, corners[2].y)
        );
    }

    void BounceFromBoundary()
    {
        // Меняем направление на противоположное
        randomDirection = -randomDirection;
        directionTimer = 0;
    }

    void SetRandomDirection()
    {
        randomDirection = Random.insideUnitCircle.normalized;
        directionTimer = 0;
    }

    void TryToLand()
    {
        // Проверяем случайный шанс и что муха не слишком высоко
        if (Random.value < landingChance && tableZone != null && !isLanding && !isSitting)
        {
            StartLanding();
        }
    }

    void StartLanding()
    {
        // Выбираем точку на столе
        targetLandingPoint = GetRandomPointInRectTransform(tableZone);

        // Проверяем, что точка действительно на столе
        if (IsPointInRectTransform(targetLandingPoint, tableZone))
        {
            isLanding = true;
            Debug.Log("Муха полетела к столу в точку: " + targetLandingPoint);
        }
    }

    public void TakeOff()
    {
        isSitting = false;
        isLanding = false;
        SetRandomDirection();

        if (spriteRenderer != null)
            spriteRenderer.color = Color.white;

        Debug.Log("Муха улетела со стола");
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("FlySwatter") && isSitting)
        {
            Die();
        }
    }

    void Die()
    {
        if (gameManager != null)
            gameManager.AddKill();
        StartCoroutine(DeathEffect());
    }

    IEnumerator DeathEffect()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.clear;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = Color.clear;
        }

        // Возрождаемся
        TakeOff();

        if (flyZone != null)
        {
            transform.position = GetRandomPointInRectTransform(flyZone);
        }
    }

    // Визуализация
    void OnDrawGizmos()
    {
        if (!showGizmos) return;

        // Текущее направление
        if (Application.isPlaying)
        {
            if (isLanding)
            {
                Gizmos.color = landingPathColor;
                Gizmos.DrawLine(transform.position, targetLandingPoint);
                Gizmos.DrawSphere(targetLandingPoint, 0.2f);
            }
            else
            {
                Gizmos.color = flyPathColor;
                Gizmos.DrawRay(transform.position, randomDirection * 2f);
            }
            Gizmos.DrawWireSphere(transform.position, 0.3f);
        }

        // Показываем зоны
        if (flyZone != null)
        {
            Vector3[] corners = new Vector3[4];
            flyZone.GetWorldCorners(corners);

            Gizmos.color = new Color(0, 1, 1, 0.3f);
            for (int i = 0; i < 4; i++)
            {
                Gizmos.DrawLine(corners[i], corners[(i + 1) % 4]);
            }
        }

        if (tableZone != null)
        {
            Vector3[] corners = new Vector3[4];
            tableZone.GetWorldCorners(corners);

            Gizmos.color = new Color(0, 1, 0, 0.3f);
            for (int i = 0; i < 4; i++)
            {
                Gizmos.DrawLine(corners[i], corners[(i + 1) % 4]);
            }
        }

        // Показываем сидящую муху
        if (isSitting)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position, 0.4f);
        }
    }
}