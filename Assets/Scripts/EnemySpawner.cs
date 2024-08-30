using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class EnemySpawner : SerializedMonoBehaviour
{
    [System.Serializable]
    public class SpawnSettings
    {
        [LabelText("Spawn Interval")] [Tooltip("적 생성 간격(초)")] 
        public float spawnInterval;

        [LabelText("Enemy Prefabs")] [Tooltip("생성할 적 프리팹 리스트")]
        public List<GameObject> enemyPrefabs;

        [LabelText("Number of Enemies per Wave")] [Tooltip("각 웨이브 당 생성할 적 수")]
        public int enemiesPerWave;
    }

    [LabelText("Spawn Settings per Level")]
    [Tooltip("레벨별 적 생성 설정")]
    [ListDrawerSettings(Expanded = true)]
    public List<SpawnSettings> levelSpawnSettings;

    [LabelText("Top Spawn Point")] [Tooltip("상단에 위치한 스폰 포인트")]
    public Transform topSpawnPoint;

    [LabelText("Bottom Spawn Point")] [Tooltip("하단에 위치한 스폰 포인트")]
    public Transform bottomSpawnPoint;

    [LabelText("Left Spawn Point")] [Tooltip("좌측에 위치한 스폰 포인트")]
    public Transform leftSpawnPoint;

    [LabelText("Right Spawn Point")] [Tooltip("우측에 위치한 스폰 포인트")]
    public Transform rightSpawnPoint;

    private Transform[] spawnPoints; // 네 방향 스폰 포인트를 배열로 저장

    private int currentLevel = 0;

    private void Start()
    {
        // 네 방향 스폰 포인트를 배열에 저장
        spawnPoints = new Transform[] { topSpawnPoint, bottomSpawnPoint, leftSpawnPoint, rightSpawnPoint };

        // 현재 레벨의 스폰 설정을 사용하여 적을 생성하는 코루틴 시작
        StartCoroutine(SpawnEnemies());
    }

    private IEnumerator SpawnEnemies()
    {
        while (currentLevel < levelSpawnSettings.Count)
        {
            var settings = levelSpawnSettings[currentLevel];

            for (int i = 0; i < settings.enemiesPerWave; i++)
            {
                SpawnEnemyAtFixedDirection(settings);
                yield return new WaitForSeconds(settings.spawnInterval);
            }

            // 다음 레벨로 넘어가기 전에 잠시 대기
            yield return new WaitForSeconds(5f);
            currentLevel++;
        }
    }

    private void SpawnEnemyAtFixedDirection(SpawnSettings settings)
    {
        // 랜덤한 스폰 포인트를 선택
        int randomIndex = Random.Range(0, spawnPoints.Length);
        Transform spawnPoint = spawnPoints[randomIndex];

        // 랜덤한 적 프리팹을 선택하여 생성
        var enemyPrefab = settings.enemyPrefabs[Random.Range(0, settings.enemyPrefabs.Count)];
        Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
    }
}
