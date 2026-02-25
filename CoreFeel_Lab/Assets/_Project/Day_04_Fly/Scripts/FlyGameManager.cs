using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FlyGameManager : MonoBehaviour
{
    public static FlyGameManager Instance;

    [Header("UI")]
    public TextMeshProUGUI scoreText;

    private int killCount = 0;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void AddKill()
    {
        killCount++;
        UpdateScoreUI();
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = "Счет: " + killCount;
    }
}