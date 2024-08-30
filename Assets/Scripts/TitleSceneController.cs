using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleSceneController : MonoBehaviour
{
    public Button startButton; // 시작 버튼을 위한 변수
    public RectTransform uiElementToMove; // 이동할 UI 요소 (보통 startButton의 부모나 다른 UI 요소)

    public float moveDuration = 2f; // 이동 시간
    public Vector2 moveDistance = new Vector2(300f, 0f); // 이동 거리 (오른쪽으로 300 유닛 이동)

    private Vector2 initialPosition;

    void Start()
    {
        // 시작 버튼에 OnClickListener 추가
        if (startButton != null)
        {
            startButton.onClick.AddListener(OnStartButtonClicked);
        }

        // UI 요소의 초기 위치 저장
        if (uiElementToMove != null)
        {
            initialPosition = uiElementToMove.anchoredPosition;
        }
    }

    // 시작 버튼 클릭 시 호출될 메서드
    void OnStartButtonClicked()
    {
        // 시작 버튼이 클릭되면 UI 요소를 오른쪽으로 이동시키고 씬 전환 시작
        StartCoroutine(MoveUIAndLoadScene());
    }

    private IEnumerator MoveUIAndLoadScene()
    {
        if (uiElementToMove != null)
        {
            float elapsedTime = 0f;

            while (elapsedTime < moveDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / moveDuration;
                uiElementToMove.anchoredPosition = Vector2.Lerp(initialPosition, initialPosition + moveDistance, t);
                yield return null;
            }

            // 이동 완료 후 정확한 최종 위치 설정
            uiElementToMove.anchoredPosition = initialPosition + moveDistance;
        }
        SceneManager.LoadScene("MainScene"); // "MainScene"은 이동할 씬의 이름입니다.
    }
}
