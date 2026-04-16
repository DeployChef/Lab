using DG.Tweening;
using TMPro;
using UnityEngine;

public class DonutMech : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI counterText;

    [Header("Spin Settings")]
    [SerializeField] private float spinPower = 500f;
    [SerializeField] private float friction = 2f;

    [Header("Animation Settings")]
    [SerializeField] private TextMeshProUGUI floatingTextPrefab;
    [SerializeField] private float punchScale = 0.3f;
    [SerializeField] private float floatUpDistance = 80f;
    [SerializeField] private float floatDuration = 0.6f;

    private float _currentSpeed;
    private int _count;

    public void OnDonutClick()
    {
        Debug.Log("Click!");

        _count++;

        _currentSpeed += spinPower;

        if(counterText)
            counterText.text = _count.ToString();

        counterText.transform.DOPunchScale(Vector3.one * punchScale, 0.2f);

        transform.DOPunchScale(Vector3.one * 0.15f, 0.15f);

        ShowFloatingNumber();
    }

    void ShowFloatingNumber()
    {
        if (floatingTextPrefab == null) return;

        // Создаём +1 в позиции счётчика
        TextMeshProUGUI flyingText = Instantiate(floatingTextPrefab,
            counterText.transform.position + new Vector3(100f, 240f, 0),
            Quaternion.identity,
            counterText.transform.parent); // Родитель — Canvas

        flyingText.text = $"+1";
        flyingText.gameObject.SetActive(true);

        // Взлетаем вверх и исчезаем
        flyingText.transform.DOMoveY(flyingText.transform.position.y + floatUpDistance, floatDuration)
            .SetEase(Ease.OutCubic);

        flyingText.DOFade(0f, floatDuration).SetEase(Ease.InQuad);

        // Удаляем после анимации
        Destroy(flyingText.gameObject, floatDuration);
    }

    void Update()
    {
        transform.Rotate(0, 0, _currentSpeed * Time.deltaTime);

        _currentSpeed = Mathf.Lerp(_currentSpeed, 0, Time.deltaTime * friction);
    }
}
