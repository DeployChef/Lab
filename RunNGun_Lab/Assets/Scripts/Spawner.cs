using System.Collections;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private float rad = 1f;
    [SerializeField] private int minEnemy = 1;
    [SerializeField] private int maxEnemy = 3;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private int waveSeconds = 3;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(Spawn());
    }

    IEnumerator Spawn()
    {
        while (true)
        {
            yield return new WaitForSeconds(waveSeconds);

            var enemiesCount = Random.Range(minEnemy, maxEnemy);

            for (var i = 0; i < enemiesCount; i++)
            {
                var randomInCircle = Random.insideUnitCircle * rad;
                var offset = new Vector3(randomInCircle.x, 0, randomInCircle.y);
                var spawnPosition = playerTransform.position + offset;
                spawnPosition.y = playerTransform.position.y; // фиксируем высоту

                var obj = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
                var enemy = obj.GetComponent<Enemy>();
                enemy.SetPlayerTransform(playerTransform);
            }
        }
    }
}
