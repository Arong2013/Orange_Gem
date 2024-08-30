using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poos : MonoBehaviour
{
    public float lifetime = 1f; // 몇 초 뒤에 삭제할지 설정
    public float detectionRadius = 3f; // Ground 오브젝트를 탐지할 반경

    void Start()
    {
        // 일정 시간 뒤에 DeleteAndChangeGround 메서드 호출
        Invoke("DeleteAndChangeGround", lifetime);
    }

    private void DeleteAndChangeGround()
    {
        // 주위의 Ground 오브젝트들을 탐지
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectionRadius);

        foreach (Collider2D hit in hits)
        {
            // Ground 컴포넌트를 가지고 있는지 확인
            Ground ground = hit.GetComponent<Ground>();
            if (ground != null && ground.isGood)
            {
                ground.ToBad();
                // 필요 시, 스프라이트 변경이나 추가적인 로직을 여기에 추가
            }
        }

        // 현재 Poos 오브젝트를 삭제
        Destroy(gameObject);
    }

    // Gizmos를 통해 탐지 반경을 시각적으로 표시 (선택 사항)
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
