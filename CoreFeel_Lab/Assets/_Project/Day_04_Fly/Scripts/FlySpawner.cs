using UnityEngine;

public class FlySpawner : MonoBehaviour
{
    [Header("Префабы")]
    public GameObject flyPrefab;

    [Header("Зоны (RectTransform)")]
    public RectTransform flyZone;
    public RectTransform tableZone;

    [Header("Настройки спауна")]
    public int maxFlies = 5;
    public float spawnInterval = 2f;

    private float spawnTimer;

    void Start()
    {
        spawnTimer = spawnInterval;

        if (flyZone == null)
            Debug.LogError("FlyZone не назначена!");
        if (tableZone == null)
            Debug.LogError("TableZone не назначена!");

        // Создаем начальных мух
        for (int i = 0; i < maxFlies / 2; i++)
        {
            SpawnFly();
        }
    }

    void Update()
    {
        if (flyZone == null || tableZone == null) return;

        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnInterval)
        {
            if (GetActiveFlyCount() < maxFlies)
            {
                SpawnFly();
            }
            spawnTimer = 0;
        }
    }

    void SpawnFly()
    {
        Vector2 spawnPos = GetRandomPointInZone(flyZone);
        GameObject newFly = Instantiate(flyPrefab, spawnPos, Quaternion.identity);

        Fly flyScript = newFly.GetComponent<Fly>();
        if (flyScript != null)
        {
            flyScript.flyZone = flyZone;
            flyScript.tableZone = tableZone;
        }
    }

    Vector2 GetRandomPointInZone(RectTransform zone)
    {
        if (zone == null) return Vector2.zero;

        Vector3[] corners = new Vector3[4];
        zone.GetWorldCorners(corners);

        return new Vector2(
            Random.Range(corners[0].x, corners[2].x),
            Random.Range(corners[0].y, corners[2].y)
        );
    }

    int GetActiveFlyCount()
    {
        return GameObject.FindGameObjectsWithTag("Fly").Length;
    }

    void OnDrawGizmosSelected()
    {
        if (flyZone != null)
        {
            Vector3[] corners = new Vector3[4];
            flyZone.GetWorldCorners(corners);

            // Рисуем зону спауна
            Gizmos.color = new Color(0, 1, 1, 0.2f);
            UnityEditor.Handles.DrawSolidRectangleWithOutline(corners,
                new Color(0, 1, 1, 0.1f),
                Color.cyan);
        }
    }
}