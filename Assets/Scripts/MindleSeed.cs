using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MindleSeed : MonoBehaviour
{
    public bool isMonther; // 임의로 true 설정 (필요에 따라 변경)
    public float lifeTime = 20f; // 생명 시간
    public float searchRadius = 5f; // 주변을 탐색할 반경
    public int spawnCount = 10; // 생성할 오브젝트의 수
    public float initialDelay = 3f; // 바람의 영향을 받지 않는 시간
    public float movementDuration = 5f; // 이동에 걸리는 시간
    public float movementRadius = 10f; // 이동할 반경


    public bool SpawnMothers;

    private void Start()
    {
        if (!isMonther)
        {
            Invoke("RemoveObject", lifeTime);
        }

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.TryGetComponent<Ground>(out Ground component) && component.isGood == false)
        {
            component.ChangeGround();
        }
    }

    private void RemoveObject()
    {
        if (isMonther)
        {
            SpawnObjects();
        }

        Destroy(gameObject);
    }

    private void SpawnObjects()
    {
        int randIndex = Random.Range(0, spawnCount); // 특정 오브젝트에 추가 행동을 위한 랜덤 인덱스 설정
        for (int i = 0; i < spawnCount; i++)
        {
            GameObject newObject = Instantiate(GameManager.Instance.son.gameObject, transform.position, Quaternion.identity);
            StartCoroutine(MoveObjectAndSpawn(newObject));

            // randIndex와 현재 인덱스가 일치하는 경우 SpawnMothers를 true로 설정
            if (i == randIndex)
            {
                newObject.GetComponent<MindleSeed>().SpawnMothers = true;
            }
            else
            {
                // 20% 확률로 SpawnMothers를 true로 설정
                if (Random.value <= 0.2f) // Random.value는 0.0f에서 1.0f 사이의 값을 반환합니다.
                {
                    newObject.GetComponent<MindleSeed>().SpawnMothers = true;
                }
            }
        }
    }

    private IEnumerator MoveObjectAndSpawn(GameObject obj)
    {
        // 랜덤한 목표 지점 생성 (일직선으로 이동할 목표 지점)
        Vector3 targetPosition = transform.position + new Vector3(
            Random.Range(-movementRadius, movementRadius),
            Random.Range(-movementRadius, movementRadius),
            0f
        );
        obj.transform.DOMove(targetPosition, movementDuration);
        yield return null;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, searchRadius);
    }
}
