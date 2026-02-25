using UnityEngine;
using UnityEngine.InputSystem;

public class HandController : MonoBehaviour
{
    [Header("Movement Boundaries")]
    [SerializeField] private Vector2 pointA = new Vector2(1.52f, -0.98f);  // Левая-верхняя
    [SerializeField] private Vector2 pointB = new Vector2(1.86f, -1.32f); // Правая-нижняя

    [Header("Wobble Settings")]
    [SerializeField] private float wobbleSpeed = 0.5f;
    [SerializeField] private float tiltAmount = 2.5f;     // Макс наклон в градусах

    [Header("Click Settings")]
    [SerializeField] private string clickTriggerName = "Click";
    [SerializeField] private AudioClip[] clickSounds;
    [SerializeField] private AudioSource audioSource;
    [Range(0f, 1f)]
    [SerializeField] private float soundVolume = 0.7f;

    private Animator animator;
    private Keyboard keyboard;
    private float timer;

    void Start()
    {
        animator = GetComponent<Animator>();
        keyboard = Keyboard.current;
        timer = Random.Range(0f, 10f);

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
        if (audioSource == null && clickSounds.Length > 0)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.volume = soundVolume;
        }
    }

    void Update()
    {
        HandleWobble();
        HandleClick();
    }

    void HandleWobble()
    {
        timer += Time.deltaTime * wobbleSpeed;

        // 1. ПЛАВНОЕ ДВИЖЕНИЕ МЕЖДУ ДВУМЯ ТОЧКАМИ
        // Используем синус для плавного перехода между A и B
        float t = (Mathf.Sin(timer) + 1f) * 0.5f; // Конвертируем -1..1 → 0..1

        // Интерполяция позиции
        float currentX = Mathf.Lerp(pointA.x, pointB.x, t);
        float currentY = Mathf.Lerp(pointA.y, pointB.y, t);

        transform.localPosition = new Vector3(currentX, currentY, -1);

        // 2. НАКЛОН В ЗАВИСИМОСТИ ОТ ПОЗИЦИИ
        // Чем правее рука - тем больше наклон, чем левее - обратно
        float normalizedX = (currentX - pointA.x) / (pointB.x - pointA.x); // 0..1
        float tilt = Mathf.Lerp(-tiltAmount, tiltAmount, normalizedX);

        // Добавляем легкое покачивание при движении
        float bounce = Mathf.Sin(timer * 2f) * 0.8f;

        transform.localEulerAngles = new Vector3(0, 0, tilt + bounce);
    }

    void HandleClick()
    {
        if (keyboard != null && keyboard.spaceKey.wasPressedThisFrame)
        {
            animator.SetTrigger(clickTriggerName);
        }
    }

    public void PlayClickSound()
    {
        if (clickSounds.Length == 0 || audioSource == null)
            return;

        int randomIndex = Random.Range(0, clickSounds.Length);
        audioSource.PlayOneShot(clickSounds[randomIndex], soundVolume);
    }

    // Рисуем границы в сцене для наглядности
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(new Vector3(pointA.x, pointA.y, 0), 0.05f);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(new Vector3(pointB.x, pointB.y, 0), 0.05f);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(new Vector3(pointA.x, pointA.y, 0), new Vector3(pointB.x, pointB.y, 0));
    }
}