using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Donut : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("References")]
    public RectTransform container;
    public TextMeshProUGUI scoreText;
    public ParticleSystem sprinkles;

    [Header("Settings")]
    public float tiltAmount = 15f;
    public float tiltSmooth = 5f;
    private bool isHovering = false; // Флаг: над пончиком мы или нет
    private int score;

    [Header("Spin Settings")]
    public float spinPower = 500f; // Сила одного клика
    public float friction = 2f;    // Насколько быстро затухает (чем выше, тем быстрее стоп)
    private float currentSpeed = 0f;

    public void OnDonutClick()
    {
        score++;
        scoreText.text = score.ToString();

        currentSpeed -= spinPower;

        if (sprinkles != null) sprinkles.Play();
    }

    // Срабатывает, когда мышка наводится на пончик
    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
    }

    // Срабатывает, когда мышка уходит с пончика
    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
    }

    void Update()
    {
        //Quaternion targetRotation;

        //if (isHovering)
        //{
        //    // Рассчитываем наклон относительно мыши
        //    Vector2 mousePos = Mouse.current.position.ReadValue();
        //    RectTransformUtility.ScreenPointToLocalPointInRectangle(container, mousePos, null, out Vector2 localPoint);

        //    float tiltX = (localPoint.y / container.rect.height) * tiltAmount;
        //    float tiltY = -(localPoint.x / container.rect.width) * tiltAmount;
        //    targetRotation = Quaternion.Euler(tiltX, tiltY, 0);
        //}
        //else
        //{
        //    // Если мышка не над пончиком — возвращаемся в 0
        //    targetRotation = Quaternion.identity;
        //}

        //// Плавный переход к целевому вращению
        //container.localRotation = Quaternion.Slerp(container.localRotation, targetRotation, Time.deltaTime * tiltSmooth);

        // 1. Вращаем объект вокруг оси Z
        // Используем Time.deltaTime, чтобы скорость не зависела от FPS
        transform.Rotate(0, 0, currentSpeed * Time.deltaTime);

        // 2. Плавно гасим скорость (эффект трения)
        // Lerp тянет текущую скорость к нулю со временем
        currentSpeed = Mathf.Lerp(currentSpeed, 0, Time.deltaTime * friction);
    }
}
