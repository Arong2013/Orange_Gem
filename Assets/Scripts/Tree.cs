using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour
{
    private void Start()
    {
        // GameManager에 나무 생성 증가 보고
        GameManager.Instance.IncrementTreeCreated();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        // 충돌한 오브젝트가 Ground인지 확인
        if (other.gameObject.TryGetComponent<Ground>(out Ground ground))
        {
            // Ground의 isGood이 false일 경우 특정 작업 수행
            if (!ground.isGood)
            {
                ground.ChangeGround(); // 예시: Ground 상태를 변경
                Debug.Log("Tree detected a Ground with isGood = false.");
            }
        }
    }

    private void OnDestroy()
    {
        // GameManager에 나무 사망 증가 보고
        GameManager.Instance.IncrementTreeDied();
    }
}
