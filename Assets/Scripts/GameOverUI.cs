using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameOverUI : MonoBehaviour
{
    public TMP_Text gameOverStatsText; // 게임 오버 시 통계를 표시할 TextMeshPro 텍스트
    public string sceneToLoad = "Main"; // 다시 로드할 씬의 이름

    private GameManager gameManager;
    private bool canRestart = false; // 씬을 다시 시작할 수 있는 상태를 추적

    void OnEnable()
    {
        // GameManager 인스턴스 가져오기
        gameManager = GameManager.Instance;

        // 통계 정보를 텍스트로 표시
        StartCoroutine(DisplayGameOverStats());
    }

    private IEnumerator DisplayGameOverStats()
    {
        // GameManager에서 통계 정보를 받아와서 텍스트로 구성
        float goodGroundPercentage = gameManager.GetGoodGroundPercentage();

        string statsText = $"Bugs: Created = {gameManager.totalBugsCreated}, Died = {gameManager.totalBugsDied}\n" +
                           $"Birds: Created = {gameManager.totalBirdsCreated}, Died = {gameManager.totalBirdsDied}\n" +
                           $"Trees: Created = {gameManager.totalTreesCreated}, Died = {gameManager.totalTreesDied}\n" +
                           $"Dandelion: Created = {gameManager.totalOriMinDlesCreated}, Died = {gameManager.totalOriMinDlesDied}\n" +
                           $"Good Ground Percentage: {goodGroundPercentage + 20:F2}%\n\n" +
                           "Press R to restart.";

        gameOverStatsText.text = ""; // 텍스트 초기화

        // 글자를 하나씩 천천히 표시
        foreach (char c in statsText)
        {
            gameOverStatsText.text += c;
            yield return new WaitForSecondsRealtime(0.05f); // 각 글자 사이의 대기 시간 (0.05초), 실시간으로 처리
        }

        // 텍스트가 다 출력되면 R 키로 재시작 가능하게 설정
        canRestart = true;
    }

    void Update()
    {
        // 텍스트가 모두 표시된 후 R 키를 누르면 지정된 씬으로 로드
        if (canRestart && Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(sceneToLoad); // 특정 씬 로드
        }
    }
}
