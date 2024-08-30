using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using DG.Tweening;

public class GameManager : Singleton<GameManager>
{
    public Vector2 Size; // 맵의 크기
    public GameObject Ground; // 생성할 오브젝트

    public GameObject monther, son, Mindle; // 생성할 오브젝트들
    // Odin Inspector를 사용해 인스펙터에서 설정 가능한 레벨별 스프라이트 딕셔너리
    [OdinSerialize, DictionaryDrawerSettings(KeyLabel = "Level", ValueLabel = "Sprite")]
    public Dictionary<int, Sprite> levelSprites;

    private List<Ground> allGroundObjects; // 게임 시작 시 모든 Ground 오브젝트를 저장할 리스트

    public List<GameObject> specificObjectsToSpawn = new List<GameObject>();

    public GameObject Bird,Tree;

    private void Start()
    {
        // 게임 시작 시 모든 Ground 컴포넌트를 가진 오브젝트를 리스트로 저장
        allGroundObjects = new List<Ground>(FindObjectsOfType<Ground>());


        StartCoroutine(CheckLevel1Grounds()); // 레벨 1 확인 코루틴 시작
        StartCoroutine(CheckLevel2Grounds()); // 레벨 2 확인 코루틴 시작
        StartCoroutine(CheckLevel3Grounds()); // 레벨 3 확인 코루틴 시작

    }

    private IEnumerator CheckLevel1Grounds()
    {
        while (true)
        {
            yield return new WaitForSeconds(10f); // 10초 대기

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
        while (true)
        {
            yield return new WaitForSeconds(15f); // 15초 대기

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
        while (true)
        {
            yield return new WaitForSeconds(30f); // 20초 대기

            int maxSpecificObjectCount = 3; // 생성할 최대 개수
            int specificObjectCount = 0;

            // 레벨이 3 이상인 오브젝트들에서 특정 오브젝트 생성
            foreach (Ground ground in allGroundObjects)
            {
                if (ground.levelUpCount >= 3 && specificObjectCount < maxSpecificObjectCount)
                {
                    if (UnityEngine.Random.value <= 0.01f) // 2% 확률
                    {
                        GameObject randomObject = specificObjectsToSpawn[UnityEngine.Random.Range(0, specificObjectsToSpawn.Count)];
                        Instantiate(Tree.gameObject, ground.transform.position, Quaternion.identity);
                        specificObjectCount++;
                    }

                    // 스프라이트 업데이트
                    UpdateSprite(ground.GetComponent<SpriteRenderer>(), ground.levelUpCount);
                }
            }

            // 디버그 로그 출력
            Debug.Log($"레벨 3 이상에서 생성된 특정 오브젝트 수: {specificObjectCount}");
        }
    }


    public void UpdateSprite(SpriteRenderer spriteRenderer, int levelUpCount)
    {
        if (levelSprites.ContainsKey(levelUpCount))
        {
            spriteRenderer.sprite = levelSprites[levelUpCount];
        }
    }
}
