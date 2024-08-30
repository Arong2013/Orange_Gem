using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Sirenix.OdinInspector;

public class GameManager : Singleton<GameManager>
{
    public int totalBugsCreated = 0; // 총 벌레 생성 수
    public int totalBugsDied = 0; // 총 벌레 사망 수
    public int totalBirdsCreated = 0; // 총 새 생성 수
    public int totalBirdsDied = 0; // 총 새 사망 수
    public int totalTreesCreated = 0; // 총 나무 생성 수
    public int totalTreesDied = 0; // 총 나무 사망 수

    public int totalOriMinDlesCreated = 0; // 총 OriMinDle 생성 수
    public int totalOriMinDlesDied = 0; // 총 OriMinDle 사망 수




    public Vector2 Size; // 맵의 크기
    public GameObject Ground; // 생성할 오브젝트
    public GameObject monther, son, Mindle; // 생성할 오브젝트들
    public Dictionary<int, Sprite> levelSprites; // 레벨별 스프라이트 딕셔너리

    private List<Ground> allGroundObjects; // 게임 시작 시 모든 Ground 오브젝트를 저장할 리스트
    public List<GameObject> specificObjectsToSpawn = new List<GameObject>();
    public GameObject Bird, Tree;

    public TextMeshProUGUI timerText; // 게임 타이머를 표시할 TextMeshPro
    public Image endGameImage; // 게임 종료 후 표시할 이미지
    public float gameTime = 90f; // 게임 시간 (60초)
    private bool isGameOver = false;



    public GameObject gameOverUI;

    private void Start()
    {
        // 게임 시작 시 모든 Ground 컴포넌트를 가진 오브젝트를 리스트로 저장
        allGroundObjects = new List<Ground>(FindObjectsOfType<Ground>());

        StartCoroutine(CheckLevel1Grounds()); // 레벨 1 확인 코루틴 시작
        StartCoroutine(CheckLevel2Grounds()); // 레벨 2 확인 코루틴 시작
        StartCoroutine(CheckLevel3Grounds()); // 레벨 3 확인 코루틴 시작

        StartCoroutine(GameTimer()); // 게임 타이머 코루틴 시작
    }
    [Button("Place Fwang Tiles")] // OdinInspector를 사용한 버튼으로 에디터에서 바로 실행 가능
    private void PlaceFwangTiles()
    {
        // 기존의 Fwang 그룹 오브젝트가 있다면 제거
        GameObject existingParent = GameObject.Find("FwangParent");
        if (existingParent != null)
        {
            DestroyImmediate(existingParent);
        }

        // 새로운 빈 오브젝트를 생성하여 Fwang 오브젝트들의 부모로 설정
        GameObject fwangParent = new GameObject("FwangParent");

        // Size만큼 Fwang 오브젝트 생성
        for (int x = 0; x < Size.x; x++)
        {
            for (int y = 0; y < Size.y; y++)
            {
                Vector2 position = new Vector2(x, y);
                GameObject newFwang = Instantiate(Ground, position, Quaternion.identity);
                newFwang.name = $"Fwang_{x}_{y}"; // 오브젝트의 이름 지정
                newFwang.transform.parent = fwangParent.transform; // FwangParent의 자식으로 설정
            }
        }
    }

    private IEnumerator CheckLevel1Grounds()
    {
        while (!isGameOver)
        {
            yield return new WaitForSeconds(7f); // 7초 대기

            // 레벨이 1 이상인 오브젝트들 중에서 Mindle 오브젝트를 최대 5개 생성
            int maxMindleCount = 5;
            int mindleCount = 0;
            foreach (Ground ground in allGroundObjects)
            {
                if (ground.levelUpCount >= 1 && mindleCount < maxMindleCount)
                {
                    if (UnityEngine.Random.value <= 0.01f) // 1% 확률
                    {
                        Instantiate(Mindle.gameObject, ground.transform.position, Quaternion.identity);
                        mindleCount++;
                    }

                    // 스프라이트 업데이트
                    UpdateSprite(ground.GetComponent<SpriteRenderer>(), ground.levelUpCount);
                }
            }

            // 디버그 로그 출력
            Debug.Log($"레벨 1 이상에서 생성된 Mindle 수: {mindleCount}");
        }
    }

    private IEnumerator CheckLevel2Grounds()
    {
        while (!isGameOver)
        {
            yield return new WaitForSeconds(18f); // 18초 대기

            int maxSpecificObjectCount = 3; // 생성할 최대 개수
            int specificObjectCount = 0;

            // 레벨이 2 이상인 오브젝트들에서 특정 오브젝트 생성
            foreach (Ground ground in allGroundObjects)
            {
                if (ground.levelUpCount >= 2 && specificObjectCount < maxSpecificObjectCount)
                {
                    if (UnityEngine.Random.value <= 0.01f) // 1% 확률
                    {
                        GameObject randomObject = specificObjectsToSpawn[UnityEngine.Random.Range(0, specificObjectsToSpawn.Count)];
                        Instantiate(randomObject, ground.transform.position, Quaternion.identity);
                        specificObjectCount++;
                    }

                    // 스프라이트 업데이트
                    UpdateSprite(ground.GetComponent<SpriteRenderer>(), ground.levelUpCount);
                }
            }

            // 레벨이 2 이상인 Ground 오브젝트 수 확인
            int level2OrHigherCount = 0;
            foreach (Ground ground in allGroundObjects)
            {
                if (ground.levelUpCount >= 2)
                {
                    level2OrHigherCount++;
                }
            }

            // 필드에 있는 Bug 오브젝트의 수 확인
            int bugCount = FindObjectsOfType<Bug>().Length;

            // 조건이 만족되면 새로운 오브젝트 생성
            if (level2OrHigherCount >= 50 && bugCount >= 3)
            {
                Instantiate(Bird.gameObject, new Vector3(50, 50, 0), Quaternion.identity); // 예시로 특정 위치에 생성
                Debug.Log("새 오브젝트가 생성되었습니다!");
            }

            // 디버그 로그 출력
            Debug.Log($"레벨 2 이상에서 생성된 특정 오브젝트 수: {specificObjectCount}");
            Debug.Log($"레벨 2 이상인 Ground 오브젝트 수: {level2OrHigherCount}, 필드에 있는 Bug 오브젝트 수: {bugCount}");
        }
    }

    private IEnumerator CheckLevel3Grounds()
    {
        while (!isGameOver)
        {
            yield return new WaitForSeconds(30f); // 30초 대기

            int maxSpecificObjectCount = 3; // 생성할 최대 개수
            int specificObjectCount = 0;
            float minDistance = 5f; // 나무 간의 최소 거리

            // 레벨이 3 이상인 오브젝트들에서 특정 오브젝트 생성
            foreach (Ground ground in allGroundObjects)
            {
                if (ground.levelUpCount >= 3 && specificObjectCount < maxSpecificObjectCount)
                {
                    if (UnityEngine.Random.value <= 0.001f) // 0.1% 확률
                    {
                        // 주변에 다른 나무가 있는지 확인
                        bool tooClose = false;
                        Collider2D[] nearbyTrees = Physics2D.OverlapCircleAll(ground.transform.position, minDistance);

                        foreach (Collider2D collider in nearbyTrees)
                        {
                            if (collider.GetComponent<Tree>() != null)
                            {
                                tooClose = true;
                                break;
                            }
                        }
                        // 다른 나무와 충분히 떨어져 있을 때만 생성
                        if (!tooClose)
                        {
                            Instantiate(Tree.gameObject, ground.transform.position, Quaternion.identity);
                            specificObjectCount++;
                        }
                    }

                    // 스프라이트 업데이트
                    UpdateSprite(ground.GetComponent<SpriteRenderer>(), ground.levelUpCount);
                }
            }

            // 디버그 로그 출력
            Debug.Log($"레벨 3 이상에서 생성된 특정 오브젝트 수: {specificObjectCount}");
        }
    }

    private IEnumerator GameTimer()
    {
        while (gameTime > 0)
        {
            yield return new WaitForSeconds(1f);
            gameTime--;
            UpdateTimerText();
        }

        EndGame(); // 게임 종료 처리
    }
    

    private void UpdateTimerText()
    {
        int minutes = Mathf.FloorToInt(gameTime / 60);
        int seconds = Mathf.FloorToInt(gameTime % 60);
        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    private void EndGame()
    {
        isGameOver = true;

        // 게임 멈춤
        Time.timeScale = 0;
        GetGoodGroundPercentage();

        // 특정 이미지 표시
        gameOverUI.SetActive(true);

        // 디버그 로그 출력
        Debug.Log("게임 종료");
    }

    public float GetGoodGroundPercentage()
    {
        if (allGroundObjects.Count == 0) return 0f; // Ground 오브젝트가 없으면 0% 반환

        int goodGroundCount = 0;
        foreach (Ground ground in allGroundObjects)
        {
            if (ground.isGood)
            {
                goodGroundCount++;
            }
        }

        return (float)goodGroundCount / allGroundObjects.Count * 100f; // 퍼센티지 계산
    }


    public void UpdateSprite(SpriteRenderer spriteRenderer, int levelUpCount)
    {
        if (levelSprites.ContainsKey(levelUpCount))
        {
            spriteRenderer.sprite = levelSprites[levelUpCount];
        }
    }

    // 벌레 생성 시 호출되는 메서드
    public void IncrementBugCreated()
    {
        totalBugsCreated++;
        Debug.Log($"Total Bugs Created: {totalBugsCreated}");
    }

    // 벌레 사망 시 호출되는 메서드
    public void IncrementBugDied()
    {
        totalBugsDied++;
        Debug.Log($"Total Bugs Died: {totalBugsDied}");
    }
    public void IncrementBirdCreated()
    {
        totalBirdsCreated++;
        Debug.Log($"Total Birds Created: {totalBirdsCreated}");
    }

    // 새 사망 시 호출되는 메서드
    public void IncrementBirdDied()
    {
        totalBirdsDied++;
        Debug.Log($"Total Birds Died: {totalBirdsDied}");
    }


    // 나무 생성 시 호출되는 메서드
    public void IncrementTreeCreated()
    {
        totalTreesCreated++;
        Debug.Log($"Total Trees Created: {totalTreesCreated}");
    }

    // 나무 사망 시 호출되는 메서드
    public void IncrementTreeDied()
    {
        totalTreesDied++;
        Debug.Log($"Total Trees Died: {totalTreesDied}");
    }

    // OriMinDle 생성 시 호출되는 메서드
    public void IncrementOriMinDleCreated()
    {
        totalOriMinDlesCreated++;
        Debug.Log($"Total OriMinDles Created: {totalOriMinDlesCreated}");
    }

    // OriMinDle 사망 시 호출되는 메서드
    public void IncrementOriMinDleDied()
    {
        totalOriMinDlesDied++;
        Debug.Log($"Total OriMinDles Died: {totalOriMinDlesDied}");
    }

}
