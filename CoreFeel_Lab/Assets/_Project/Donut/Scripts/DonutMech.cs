using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class DonutMech : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI counterText;
    [SerializeField] private ParticleSystem sprinkles;

    [Header("Spin Settings")]
    [SerializeField] private float spinPower = 500f;
    [SerializeField] private float friction = 2f;

    [Header("Animation Settings")]
    [SerializeField] private TextMeshProUGUI floatingTextPrefab;
    [SerializeField] private float punchScale = 0.3f;
    [SerializeField] private float floatUpDistance = 80f;
    [SerializeField] private float floatDuration = 0.6f;

    [Header("Sound Settings")]
    [SerializeField] private AudioClip clickSound;
    [SerializeField] private AudioClip counterSound;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private float minPitch = 0.8f;
    [SerializeField] private float maxPitch = 1.2f;

    [Header("Cursor Settings")]
    [SerializeField] private GameObject cursorPrefab;
    [SerializeField] private float cursorRadius = 40f;

    [SerializeField] private Button upClickButton;
    [SerializeField] private TextMeshProUGUI upClickText;
    [SerializeField] private Button upCursorButton;

    private float _currentSpeed;
    private int _count;

    private int _clickLvl = 1;
    private int _cursorLvl = 1;

    private int ByClickPrice => 10 * _clickLvl;
    private int CursorPrice => 20 * _cursorLvl;

    public void Start()
    {
        upClickButton.interactable = false;
        upClickButton.onClick.AddListener(OnUpClick);

        upCursorButton.interactable = false;
        upCursorButton.onClick.AddListener(OnUpCursor);
    }

    public void OnDonutClick()
    {
        PlaySound(clickSound);
        Debug.Log("Click!");

        ChangeCount(_clickLvl);

        counterText.transform.DOPunchScale(Vector3.one * punchScale, 0.2f);

        transform.DOPunchScale(Vector3.one * 0.15f, 0.15f);

        if (sprinkles) sprinkles.Play();
    }

    private void ChangeCount(int value)
    {
        _count+= value;
        
        upClickButton.interactable = _count >= ByClickPrice;
        upCursorButton.interactable = _count >= CursorPrice;

        PlaySound(counterSound);

        _currentSpeed += spinPower;

        if(counterText)
            counterText.text = _count.ToString();

        ShowFloatingNumber(value);
    }

    public void OnUpClick()
    {
        ChangeCount(-ByClickPrice);
        _clickLvl++;
        upClickText.text = $"+ клик ({ByClickPrice})";
    }

    public void OnUpCursor()
    {
        if (_count >= CursorPrice)
        {
            ChangeCount(-CursorPrice);
            SpawnCursor();
        }
    }

    private void SpawnCursor()
    {
        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;

        Vector3 offset = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * cursorRadius;
        Vector3 spawnPosition = transform.position + offset;

        GameObject cursor = Instantiate(cursorPrefab, spawnPosition, Quaternion.identity);

        Vector3 directionToCenter = (transform.position - cursor.transform.position).normalized;
        float angleToCenter = Mathf.Atan2(directionToCenter.y, directionToCenter.x) * Mathf.Rad2Deg;
        cursor.transform.rotation = Quaternion.Euler(0, 0, angleToCenter - 90);

        Cursor autoCursor = cursor.GetComponent<Cursor>();
        if (autoCursor != null) autoCursor.Initialize(this);
    }

    private void PlaySound(AudioClip clip)
    {
        if (!clip) return;

        var randomPitch = Random.Range(minPitch, maxPitch);
        audioSource.pitch = randomPitch;

        audioSource.PlayOneShot(clip);
    }

    private void ShowFloatingNumber(int value)
    {
        if (floatingTextPrefab == null) return;

        // Создаём +1 в позиции счётчика
        TextMeshProUGUI flyingText = Instantiate(floatingTextPrefab,
            counterText.transform.position + new Vector3(0, 10f, 0),
            Quaternion.identity,
            counterText.transform); // Родитель — Canvas
        var sign = value > 0 ? "+" : "";
        flyingText.text = $"{sign}{value}";
        flyingText.gameObject.SetActive(true);

        // Взлетаем вверх и исчезаем
        flyingText.transform.DOMoveY(flyingText.transform.position.y + floatUpDistance, floatDuration)
            .SetEase(Ease.OutCubic);

        flyingText.DOFade(0f, floatDuration).SetEase(Ease.InQuad);

        // Удаляем после анимации
        Destroy(flyingText.gameObject, floatDuration);
    }

    private void Update()
    {
        transform.Rotate(0, 0, _currentSpeed * Time.deltaTime);

        _currentSpeed = Mathf.Lerp(_currentSpeed, 0, Time.deltaTime * friction);
    }
}
