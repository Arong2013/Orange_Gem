using System.Collections;
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

    private bool facingRight = true; // 현재 바라보고 있는 방향을 저장
    private bool canDash = true; // 대쉬 가능 여부

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

        if (isMonther && other.gameObject.TryGetComponent<OriMinDle>(out OriMinDle oriMinDle) && canDash)
        {
            // Rigidbody2D의 mass를 0.8로 설정
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                StartCoroutine(TemporaryMassChange(rb));
            }
        }
    }

    private IEnumerator TemporaryMassChange(Rigidbody2D rb)
    {
        // 대쉬 불가능 설정
        canDash = false;

        // mass를 0.8로 설정
        rb.mass = 0.8f;

        // 2초 대기
        yield return new WaitForSeconds(2f);

        // mass를 1로 복구
        rb.mass = 1f;

        // 10초 후 다시 대쉬 가능
        yield return new WaitForSeconds(5f);
        canDash = true;
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
                if (Random.value <= 0.2f)
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
