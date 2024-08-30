using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;

public class Bird : MonoBehaviour
{
    public float moveSpeed = 5f; // 새의 이동 속도
    public float height = 2f; // 포물선의 높이
    public int requiredToEat = 3; // 새가 특정 오브젝트를 생성하기 위해 필요한 먹이 수

    public Vector2 moveArea = new Vector2(100f, 70f); // 이동할 영역의 크기
    public float generationCooldown = 6f; // 특수 오브젝트 생성 쿨다운
    public float detectionRadius = 10f; // OriMinDle 또는 Bug를 탐지할 반경
    public GameObject poopPrefab; // 똥 프리팹
    public float poopCooldown = 5f; // 똥을 누는 쿨다운 시간
    public float eatTimeout = 30f; // 30초 동안 먹이를 먹지 않으면 삭제

    private Vector3 startPosition;
    private int eatenCount = 0; // 새가 먹은 먹이의 수
    private bool isGenerating = false; // 현재 오브젝트 생성 중인지 여부
    private bool isPooping = false; // 현재 똥을 누고 있는지 여부
    private float lastEatTime; // 마지막으로 먹이를 먹은 시간

    void Start()
    {
        startPosition = transform.position; // 시작 위치 저장
        lastEatTime = Time.time; // 현재 시간을 마지막 먹이 시간으로 초기화
        StartCoroutine(MoveRoutine()); // 포물선 이동 루틴 시작
        StartCoroutine(PoopRoutine()); // 똥 누기 루틴 시작
        StartCoroutine(CheckTimeoutRoutine()); // 먹이 시간 초과 체크 루틴 시작
    }

    private IEnumerator MoveRoutine()
    {
        while (true)
        {
            if (isPooping)
            {
                // 똥을 누는 중이면 이동하지 않음
                yield return null;
                continue;
            }

            // 주위에 Bug나 OriMinDle이 있는지 확인
            Collider2D nearestTarget = FindNearestTarget();

            if (nearestTarget != null)
            {
                // 목표 위치를 가장 가까운 Bug 또는 OriMinDle의 위치로 설정
                MoveInParabola(nearestTarget.transform.position);
            }
            else
            {
                // 목표 위치를 무작위 위치로 설정
                Vector3 randomTargetPosition = new Vector3(
                    Random.Range(0f, moveArea.x),
                    Random.Range(50f, 70f),
                    0f
                );
                MoveInParabola(randomTargetPosition);
            }

            // 이동이 끝날 때까지 대기
            yield return new WaitForSeconds(Vector3.Distance(startPosition, transform.position) / moveSpeed);

            // 잠시 대기 후 다시 이동 시작
            yield return new WaitForSeconds(Random.Range(1f, 3f));
        }
    }

    private void MoveInParabola(Vector3 targetPosition)
    {
        // 시작 위치와 목표 위치 사이에 포물선의 정점 설정
        Vector3 midPoint = (startPosition + targetPosition) / 2 + Vector3.up * height;

        // 목표 위치까지의 거리를 계산하여 이동 시간을 구함
        float distance = Vector3.Distance(startPosition, targetPosition);
        float moveTime = distance / moveSpeed;

        // 좌우 반전 체크
        if ((targetPosition.x > startPosition.x && transform.localScale.x < 0) ||
            (targetPosition.x < startPosition.x && transform.localScale.x > 0))
        {
            Flip();
        }

        // DOTween을 사용하여 포물선 경로로 이동
        transform.DOPath(new Vector3[] { startPosition, midPoint, targetPosition }, moveTime, PathType.CatmullRom)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                // 이동 완료 후 다음 이동을 위해 시작 위치 업데이트
                startPosition = targetPosition;
            });
    }
    private void Flip()
    {
        // 스프라이트를 좌우 반전
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }

    private Collider2D FindNearestTarget()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectionRadius);
        Collider2D nearestTarget = null;
        float nearestDistance = Mathf.Infinity;

        foreach (Collider2D hit in hits)
        {
            if (hit.GetComponent<Bug>() != null || hit.TryGetComponent<MindleSeed>(out MindleSeed component) && !component.isMonther)
            {
                float distance = Vector3.Distance(transform.position, hit.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestTarget = hit;
                }
            }
        }

        return nearestTarget;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 오브젝트 생성 중이 아니어야만 먹이를 먹을 수 있음
        if (!isGenerating)
        {
            // Bug 또는 OriMinDle 컴포넌트를 가지고 있는지 확인
            if (collision.GetComponent<Bug>() != null || collision.GetComponent<OriMinDle>() != null)
            {
                eatenCount++;
                Destroy(collision.gameObject); // 먹은 오브젝트를 제거
                lastEatTime = Time.time; // 마지막으로 먹이를 먹은 시간 업데이트

                // 먹은 개수가 특정 수에 도달하면 오브젝트 생성 시작
                if (eatenCount >= requiredToEat)
                {
                    StartCoroutine(GenerateAndPoopRoutine()); // 오브젝트 생성 코루틴 시작
                    eatenCount = 0; // 먹이 개수 초기화
                }
            }
        }
    }

    private IEnumerator GenerateAndPoopRoutine()
    {
        isGenerating = true;
        float generationDuration = Random.Range(20f, 30f); // 20~30초 동안 생성
        float timePassed = 0f;

        while (timePassed < generationDuration)
        {
            // 특정 오브젝트 생성
            Instantiate(poopPrefab, transform.position, Quaternion.identity);

            // 일정 시간마다 똥 생성
            if (!isPooping)
            {
                isPooping = true;

                // 똥 생성
                Instantiate(poopPrefab, transform.position, Quaternion.identity);

                // 2초 동안 멈춤
                yield return new WaitForSeconds(2f);

                isPooping = false;
            }

            yield return new WaitForSeconds(generationCooldown); // 쿨다운마다 생성
            timePassed += generationCooldown;
        }

        isGenerating = false; // 오브젝트 생성이 끝나면 다시 먹이를 먹을 수 있음
    }

    private IEnumerator PoopRoutine()
    {
        while (true)
        {
            // 일정 시간마다 똥을 누게 함
            yield return new WaitForSeconds(poopCooldown);

            // 똥을 누는 동작 시작
            isPooping = true;

            // 똥 생성
            Instantiate(poopPrefab, transform.position, Quaternion.identity);

            // 2초 동안 멈춤
            yield return new WaitForSeconds(2f);

            // 똥 누는 동작 종료
            isPooping = false;
            yield return false;
        }
    }

    private IEnumerator CheckTimeoutRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f); // 1초마다 체크

            // 마지막으로 먹이를 먹은 시간부터 30초가 지났다면 색을 서서히 투명하게 만들고 새를 삭제
            if (Time.time - lastEatTime > eatTimeout)
            {
                SpriteRenderer[] spriteRenderers = GetComponentsInChildren<SpriteRenderer>();

                // 1초 동안 모든 SpriteRenderer의 색을 서서히 투명하게 만듦
                float fadeDuration = 1f;
                float fadeTime = 0f;

                while (fadeTime < fadeDuration)
                {
                    foreach (SpriteRenderer sr in spriteRenderers)
                    {
                        Color color = sr.color;
                        color.a = Mathf.Lerp(1f, 0f, fadeTime / fadeDuration);
                        sr.color = color;
                    }

                    fadeTime += Time.deltaTime;
                    yield return null;
                }

                // 오브젝트 삭제
                Destroy(gameObject);
                yield break; // 코루틴 종료
            }
        }
    }

}
