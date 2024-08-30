using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doum : MonoBehaviour
{
    private bool isGamePaused = false;

    private void Awake()
    {
        PauseGame();
    }

    private void Update()
    {
        // 게임이 멈춘 상태에서 아무 키나 누르면 게임을 다시 시작하고 Doum 오브젝트를 비활성화
        if (isGamePaused && Input.anyKeyDown)
        {
            ResumeGame();
            gameObject.SetActive(false);
        }
    }

    private void PauseGame()
    {
        Time.timeScale = 0f; // 게임 시간을 멈춤
        isGamePaused = true;
        AudioListener.pause = true; // 모든 오디오 멈춤
    }

    private void ResumeGame()
    {
        Time.timeScale = 1f; // 게임 시간을 다시 시작
        isGamePaused = false;
        AudioListener.pause = false; // 오디오 재개
    }
}
