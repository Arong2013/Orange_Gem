using TMPro;
using UnityEngine;
using System.Collections;

public class GameOverUI : MonoBehaviour
{
    public TMP_Text gameOverStatsText; // 게임 오버 시 통계를 표시할 TextMeshPro 텍스트

    private GameManager gameManager;

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
        string statsText = $"Bugs: Created = {gameManager.totalBugsCreated}, Died = {gameManager.totalBugsDied}\n" +
                           $"Birds: Created = {gameManager.totalBirdsCreated}, Died = {gameManager.totalBirdsDied}\n" +
                           $"Trees: Created = {gameManager.totalTreesCreated}, Died = {gameManager.totalTreesDied}\n" +
                           $"OriMinDles: Created = {gameManager.totalOriMinDlesCreated}, Died = {gameManager.totalOriMinDlesDied}";

        gameOverStatsText.text = ""; // 텍스트 초기화

        // 글자를 하나씩 천천히 표시
        foreach (char c in statsText)
        {
            gameOverStatsText.text += c;
            yield return new WaitForSecondsRealtime(0.05f); // 각 글자 사이의 대기 시간 (0.05초), 실시간으로 처리
        }
    }
}
