using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bug : MonoBehaviour
{
    public float moveRadius = 5f; // 이동할 범위 반경
    public float moveTime = 2f; // 이동에 걸리는 시간
    public float waitTime = 1f; // 각 이동 후 대기 시간
    public float detectionRadius = 2f; // 주변 Ground 탐지 반경

    private Vector3 startPosition;
    private bool facingRight = true; // 현재 바라보고 있는 방향을 저장

    void Start()
    {
        startPosition = transform.position; // 시작 위치 저장
        StartCoroutine(MoveRandomly()); // 랜덤 이동 코루틴 시작
    }

    private IEnumerator MoveRandomly()
    {
        while (true)
        {
            // 무작위 위치 생성
            Vector3 randomPosition = startPosition + new Vector3(
                Random.Range(-moveRadius, moveRadius),
                Random.Range(-moveRadius, moveRadius),
                0f
            );

            // 이동
            yield return StartCoroutine(MoveToPosition(randomPosition));

            // 대기
            yield return new WaitForSeconds(waitTime);
        }
    }

    private IEnumerator MoveToPosition(Vector3 targetPosition)
    {
        Vector3 startPosition = transform.position;
        float elapsedTime = 0f;

        // 좌우 반전 체크
        if ((targetPosition.x > startPosition.x && !facingRight) || (targetPosition.x < startPosition.x && facingRight))
        {
            Flip();
        }

        while (elapsedTime < moveTime)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / moveTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition; // 정확한 위치로 설정
    }

    private void Flip()
    {
        // 스프라이트를 좌우 반전
        facingRight = !facingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.TryGetComponent<Ground>(out Ground component) && component.isGood == false)
        {
            ChangeSurroundingGrounds(component.transform.position);
        }
    }

    private void ChangeSurroundingGrounds(Vector3 position)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, detectionRadius);
        foreach (Collider2D collider in colliders)
        {
            Ground ground = collider.GetComponent<Ground>();
            if (ground != null)
            {
                ground.ChangeGround();
            }
        }
        Destroy(gameObject);
    }
}
