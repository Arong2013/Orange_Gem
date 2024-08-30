using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindController : MonoBehaviour
{
    // AreaEffector2D 컴포넌트를 참조할 변수
    private AreaEffector2D areaEffector;

    // 바람 방향을 설정할 변수
    public float targetWindAngle = 0f; // 목표 바람 방향
    private float currentWindAngle = 0f; // 현재 바람 방향

    // 바람 세기를 설정할 변수
    public float windStrength = 0f; // 현재 바람 세기
    private float windStrengthRange = 10f; // 바람 세기 범위

    public float windChangeSpeed = 1f; // 바람의 방향이 변경되는 속도

    // 바람 각도 진동 범위 및 상태
    private bool isOscillating = false; // 현재 바람이 진동 중인지 여부

    // Start is called before the first frame update
    void Start()
    {
        // AreaEffector2D 컴포넌트를 가져오기
        areaEffector = GetComponent<AreaEffector2D>();

        // 초기 바람 방향 및 세기 설정
        if (areaEffector != null)
        {
            currentWindAngle = areaEffector.forceAngle;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // 목표 바람의 방향 설정
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            targetWindAngle = 90f; // 위쪽(90도) 방향
            isOscillating = false; // 진동 상태 해제
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            targetWindAngle = 0f; // 오른쪽(0도) 방향
            isOscillating = false; // 진동 상태 해제
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            targetWindAngle = 270f; // 아래쪽(270도) 방향
            isOscillating = false; // 진동 상태 해제
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            targetWindAngle = 180f; // 왼쪽(180도) 방향
            isOscillating = false; // 진동 상태 해제
        }

        // 현재 바람 각도를 목표 각도에 맞게 서서히 변경
        currentWindAngle = Mathf.Lerp(currentWindAngle, targetWindAngle, Time.deltaTime * windChangeSpeed);

        // 바람의 각도가 목표 각도에 도달하면 진동 상태로 전환
        if (!isOscillating && Mathf.Abs(currentWindAngle - targetWindAngle) < 0.1f)
        {
            isOscillating = true;
        }

        // 바람 각도 진동 처리 (-10도에서 +10도 범위)
        if (isOscillating)
        {
            currentWindAngle = targetWindAngle + Mathf.Sin(Time.time * windChangeSpeed) * 10f;
        }

        // 변경된 바람 각도와 세기를 AreaEffector2D에 적용
        if (areaEffector != null)
        {
            areaEffector.forceAngle = currentWindAngle;
            areaEffector.forceMagnitude = windStrength;
        }
    }
}
