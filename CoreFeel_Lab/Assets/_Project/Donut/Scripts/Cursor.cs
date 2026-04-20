using DG.Tweening;
using UnityEngine;

public class Cursor : MonoBehaviour
{
    private DonutMech _donut;
    private float _timer = 1f; // Клик раз в секунду

    public void Initialize(DonutMech donut)
    {
        _donut = donut;
    }

    void Update()
    {
        _timer -= Time.deltaTime;
        if (_timer <= 0f)
        {
            _timer = 1f;

            transform.DOPunchScale(Vector3.one * 0.3f, 0.1f);

            _donut.OnDonutClick();
        }
    }
}
