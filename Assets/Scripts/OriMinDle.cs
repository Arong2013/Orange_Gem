using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OriMinDle : MonoBehaviour
{
    public float delayBeforeSpawn = 5f; // 오브젝트 생성까지의 대기 시간

    private void Start()
    {
        // StartCoroutine을 통해 딜레이 후 오브젝트를 생성하는 코루틴 실행
        StartCoroutine(SpawnObjectAfterDelay());
    }

    private IEnumerator SpawnObjectAfterDelay()
    {
        // 지정된 시간만큼 대기
        yield return new WaitForSeconds(delayBeforeSpawn);
        Instantiate(GameManager.Instance.monther, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
