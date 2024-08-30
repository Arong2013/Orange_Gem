using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Tilemaps;

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

    private void Start()
    {
        // 게임 시작 시 모든 Ground 컴포넌트를 가진 오브젝트를 리스트로 저장
        allGroundObjects = new List<Ground>(FindObjectsOfType<Ground>());

        StartCoroutine(CheckGroundLevels()); // Ground 레벨 확인 코루틴 시작
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

    public void UpdateSprite(SpriteRenderer spriteRenderer, int levelUpCount)
    {
        if (levelSprites.ContainsKey(levelUpCount))
        {
            spriteRenderer.sprite = levelSprites[levelUpCount];
        }
    }
    private IEnumerator CheckGroundLevels()
    {
        while (true)
        {
            yield return new WaitForSeconds(10f); // 10초 대기

            // 레벨이 1 이상인 오브젝트들만 필터링
            List<Ground> filteredGrounds = new List<Ground>();
            foreach (Ground ground in allGroundObjects)
            {
                if (ground.levelUpCount >= 1)
                {
                    filteredGrounds.Add(ground);
                }
            }

            // 레벨이 1인 오브젝트들 중에서 Mindle 오브젝트를 최대 5개 생성
            int maxMindleCount = 5;
            int mindleCount = 0;
            foreach (Ground ground in filteredGrounds)
            {
                if (ground.levelUpCount == 1 && mindleCount < maxMindleCount)
                {
                    if (UnityEngine.Random.value <= 0.01f) // 1% 확률
                    {
                        Instantiate(Mindle.gameObject, ground.transform.position, Quaternion.identity);
                        mindleCount++;
                    }
                }
            }

            int maxSpecificObjectCount = 3; // 생성할 최대 개수
            int specificObjectCount = 0;

            foreach (Ground ground in filteredGrounds)
            {
                if (ground.levelUpCount == 2 && specificObjectCount < maxSpecificObjectCount)
                {
                    if (UnityEngine.Random.value <= 0.01f) // 1% 확률
                    {
                        GameObject randomObject = specificObjectsToSpawn[UnityEngine.Random.Range(0, specificObjectsToSpawn.Count)];
                        Instantiate(randomObject, ground.transform.position, Quaternion.identity);
                        specificObjectCount++;
                    }
                }
            }
            // 디버그 로그 출력
            Debug.Log($"레벨이 1 이상인 Ground 오브젝트 수: {filteredGrounds.Count}, 생성된 Mindle 수: {mindleCount}, 생성된 특정 오브젝트 수: {specificObjectCount}");
        }
    }

}
