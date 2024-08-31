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

    public Transform spriteTransform; // 스프라이트를 회전시킬 자식 객체의 Transform

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
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                StartCoroutine(TemporaryMassChange(rb));
            }
        }
    }

    private IEnumerator TemporaryMassChange(Rigidbody2D rb)
    {
        canDash = false;
        rb.mass = 0.5f;

        // 스프라이트만 빠르게 회전 시작
        Sequence rotationSequence = DOTween.Sequence()
            .Append(spriteTransform.DORotate(new Vector3(0, 0, 360), 0.2f, RotateMode.FastBeyond360).SetLoops(-1, LoopType.Restart));

        yield return new WaitForSeconds(4f);

        rotationSequence.Kill();
        spriteTransform.rotation = Quaternion.identity; // 회전 각도 초기화

        rb.mass = 1f;
        yield return new WaitForSeconds(6f);
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
        int randIndex = Random.Range(0, spawnCount);
        for (int i = 0; i < spawnCount; i++)
        {
            GameObject newObject = Instantiate(GameManager.Instance.son.gameObject, transform.position, Quaternion.identity);
            StartCoroutine(MoveObjectAndSpawn(newObject));

            if (i == randIndex)
            {
                newObject.GetComponent<MindleSeed>().SpawnMothers = true;
            }
            else
            {
                if (Random.value <= 0.2f)
                {
                    newObject.GetComponent<MindleSeed>().SpawnMothers = true;
                }
            }
        }
    }

    private IEnumerator MoveObjectAndSpawn(GameObject obj)
    {
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
