using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bird : MonoBehaviour
{
    public float moveTime = 2f; // 이동에 걸리는 시간
    public float height = 2f; // 포물선의 높이
    public int requiredToEat = 3; // 새가 특정 오브젝트를 생성하기 위해 필요한 먹이 수
    public GameObject specialObject; // 일정 개수 먹으면 생성할 오브젝트

    private Vector3 startPosition;
    private Vector3 targetPosition;
    private float elapsedTime;
    private int eatenCount = 0; // 새가 먹은 먹이의 수
    private bool isGenerating = false; // 현재 오브젝트 생성 중인지 여부
    void Start()
    {
        startPosition = transform.position; // 시작 위치 저장
        StartCoroutine(MoveInParabola()); // 포물선 이동 코루틴 시작
    }

    private IEnumerator MoveInParabola()
    {
        while (true)
        {
            // (0,0)부터 (100,100) 사이의 무작위 위치 생성
            targetPosition = new Vector3(
                Random.Range(0f, 100f),
                Random.Range(0f, 100f),
                0f
            );

            elapsedTime = 0f;

            while (elapsedTime < moveTime)
            {
                // 포물선 경로 계산
                float t = elapsedTime / moveTime;
                Vector3 currentPosition = Vector3.Lerp(startPosition, targetPosition, t);
                float parabola = 4 * height * (t - t * t); // 포물선 공식
                currentPosition.y += parabola;

                transform.position = currentPosition;

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            transform.position = targetPosition; // 정확한 위치로 설정

            // 다음 이동을 위한 시작 위치 업데이트
            startPosition = targetPosition;

            // 잠시 대기 후 다시 이동 시작
            yield return new WaitForSeconds(Random.Range(1f, 3f));
        }
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

                // 먹은 개수가 특정 수에 도달하면 오브젝트 생성 시작
                if (eatenCount >= requiredToEat && !isGenerating)
                {
                    StartCoroutine(GenerateObjects()); // 오브젝트 생성 코루틴 시작
                    eatenCount = 0; // 먹이 개수 초기화
                }
            }
        }
    }

    private IEnumerator GenerateObjects()
    {
        isGenerating = true;
        float generationDuration = Random.Range(20f, 30f); // 20~30초 동안 생성
        float timePassed = 0f;

        while (timePassed < generationDuration)
        {
            Instantiate(specialObject, transform.position, Quaternion.identity);
            yield return new WaitForSeconds(6f); // 6초마다 생성
            timePassed += 6f;
        }

        isGenerating = false; // 오브젝트 생성이 끝나면 다시 먹이를 먹을 수 있음
    }
}
