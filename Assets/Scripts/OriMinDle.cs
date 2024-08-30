using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class OriMinDle : MonoBehaviour
{
    public int spawnCount = 10; // 생성할 오브젝트의 수
    public float delayBeforeSpawn = 5f; // 오브젝트 생성까지의 대기 시간
    public float movementRadius = 10f; // 이동할 반경
    public float movementDuration = 5f; // 이동에 걸리는 시간
    public Sprite m1, m2, m3;
    private void Start()
    {
        // StartCoroutine을 통해 딜레이 후 오브젝트를 생성하는 코루틴 실행
        StartCoroutine(SpawnObjectAfterDelay());
    }
    private IEnumerator SpawnObjectAfterDelay()
    {
        // 처음에 오브젝트의 크기를 작게 설정
        transform.localScale = Vector3.zero;

        // 일정 시간 동안 크기를 원래대로 돌려놓기
        transform.DOScale(new Vector3(0.5f, 0.5f, 0.5f), delayBeforeSpawn)
            .OnUpdate(() =>
            {
                // 현재 스케일을 기반으로 스프라이트 변경
                float currentScale = transform.localScale.x;

                if (Mathf.Approximately(currentScale, 0.1f))
                {
                    // 스프라이트를 0.1에 해당하는 것으로 변경
                    GetComponent<SpriteRenderer>().sprite = m1;
                }
                else if (Mathf.Approximately(currentScale, 0.3f))
                {
                    // 스프라이트를 0.3에 해당하는 것으로 변경
                    GetComponent<SpriteRenderer>().sprite = m2;
                }
                else if (Mathf.Approximately(currentScale, 0.5f))
                {
                    // 스프라이트를 0.5에 해당하는 것으로 변경
                    GetComponent<SpriteRenderer>().sprite = m3;
                }
            });

        // 크기가 원래대로 돌아온 후 대기
        yield return new WaitForSeconds(delayBeforeSpawn);

        // 오브젝트 생성
        SpawnObjects();

        // 원래 오브젝트는 파괴
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.TryGetComponent<Ground>(out Ground component) && component.isGood == false)
        {
            component.ChangeGround();
        }
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

}
