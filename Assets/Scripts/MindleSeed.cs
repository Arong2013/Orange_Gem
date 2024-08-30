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
    public float movementRadius = 5f; // 이동할 반경

    private void Start()
    {
        Invoke("RemoveObject", lifeTime);
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
        for (int i = 0; i < spawnCount; i++)
        {
            // 중앙에서 생성
            GameObject newObject = Instantiate(GameManager.Instance.son.gameObject, transform.position, Quaternion.identity);

            // 오브젝트에 이동 코루틴 시작
            StartCoroutine(MoveObject(newObject));
        }
    }

    private IEnumerator MoveObject(GameObject obj)
    {
        // 랜덤한 목표 지점 생성 (일직선으로 이동할 목표 지점)
        Vector3 targetPosition = transform.position + new Vector3(
            Random.Range(-movementRadius, movementRadius),
            Random.Range(-movementRadius, movementRadius),
            0f
        );

        // DOTween을 사용하여 일직선으로 이동
        obj.transform.DOMove(targetPosition, movementDuration)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                // 이동 완료 후 실행할 코드
                Debug.Log("Object finished moving to the target position.");
            });

        yield return null;
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, searchRadius);
    }
}
