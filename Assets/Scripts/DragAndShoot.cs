using DG.Tweening;
using UnityEngine;

public class DragAndShoot : MonoBehaviour
{
    private Vector2 startMousePosition;
    private Vector2 endMousePosition;
    private bool isDragging = false;

    public float forceMultiplier = 1f; // 발사 거리를 조정하는 변수
    public float moveDuration = 0.5f;  // 발사 시 객체가 이동하는 데 걸리는 시간
    public float maxDragDistance = 5f; // 최대 당길 수 있는 거리
    public Transform targetIndicator;  // 도착 지점을 표시할 트랜스폼 (스프라이트를 갖고 있는 객체)
    public LayerMask tileLayerMask;    // 타일을 감지할 레이어 마스크
    public LayerMask draggableLayer;   // 드래그 가능한 오브젝트의 레이어 마스크
    public float effectRadius = 2f;    // 원형 범위의 반지름

    public GameObject objectToShootPrefab; // 날릴 오브젝트의 프리팹

    private GameObject objectToShoot;  // 실제 날아갈 오브젝트 인스턴스

    private bool isDead = false;

    private void Update()
    {
        if (!isDead)
        {
            if (Input.GetMouseButtonDown(0))
            {
                // 마우스 클릭 시 레이캐스트
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, draggableLayer);

                if (hit.collider != null)
                {
                    // 드래그 가능한 오브젝트를 찾음
                    if (hit.collider.gameObject == gameObject)
                    {
                        isDragging = true;
                        startMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    }
                }
            }

            if (Input.GetMouseButton(0) && isDragging)
            {
                endMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                // 드래그 중 도착 지점 계산
                Vector2 shootDirection = startMousePosition - endMousePosition;
                float dragDistance = Mathf.Min(shootDirection.magnitude, maxDragDistance); // 최대 거리를 넘지 않도록 제한
                shootDirection.Normalize();
                Vector2 targetPosition = (Vector2)transform.position + shootDirection * dragDistance * forceMultiplier;

                // 도착 지점에 스프라이트 이동
                if (targetIndicator != null)
                {
                    targetIndicator.position = targetPosition;
                }
            }

            if (Input.GetMouseButtonUp(0) && isDragging)
            {
                isDead = true;
                isDragging = false;
                
                // objectToShoot을 생성
                objectToShoot = Instantiate(objectToShootPrefab, transform.position, Quaternion.identity);

                Vector2 shootDirection = startMousePosition - endMousePosition;
                float dragDistance = Mathf.Min(shootDirection.magnitude, maxDragDistance); // 최대 거리를 넘지 않도록 제한
                shootDirection.Normalize();

                objectToShoot.transform.DOMove((Vector2)objectToShoot.transform.position + shootDirection * dragDistance * forceMultiplier, moveDuration)
                         .SetEase(Ease.OutQuad)
                         .OnComplete(OnMovementComplete);

                // 도착 지점 인디케이터를 비활성화 또는 숨기기 (선택사항)
                if (targetIndicator != null)
                {
                    targetIndicator.gameObject.SetActive(false);
                }
            }
        }
    }

    private void OnMovementComplete()
    {
        // 원형 범위 내에 있는 타일 감지
        Collider2D[] hitTiles = Physics2D.OverlapCircleAll(objectToShoot.transform.position, effectRadius, tileLayerMask);

        foreach (Collider2D tile in hitTiles)
        {
            // 타일에 대해 원하는 작업 수행
            Debug.Log("Tile detected: " + tile.gameObject.name);
        }

        // isDead 플래그를 다시 false로 설정하여 객체가 다시 움직일 수 있게 함
        isDead = false;

        // 필요한 경우 objectToShoot 제거 (선택사항)
        //Destroy(objectToShoot);
    }

    private void OnDrawGizmos()
    {
        // 편집기에서 원형 범위를 시각적으로 표시
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(objectToShoot != null ? objectToShoot.transform.position : transform.position, effectRadius);
    }
}