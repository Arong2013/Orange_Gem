using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

public class Ground : SerializedMonoBehaviour
{
    public int levelUpCount;
    public bool isGood;
    public int maxLevel = 5; // 최대 레벨 설정

    [SerializeField] Sprite BadGround;
    private SpriteRenderer spriteRenderer;

    public float levelUpCooldown = 2f; // 레벨업 쿨다운 시간
    private bool canLevelUp = true; // 레벨업 가능 여부

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = BadGround;
        StartCoroutine(AutoLevelUp()); // 자동 레벨업 코루틴 시작
    }

    public void ChangeGround()
    {
        if (!canLevelUp || levelUpCount >= maxLevel) return; // 쿨다운 중이거나 최대 레벨에 도달하면 레벨업 불가

        if (isGood)
        {
            levelUpCount++;
            GameManager.Instance.UpdateSprite(spriteRenderer, levelUpCount); // 레벨이 올라갈 때 스프라이트 교체 시도
        }
        else
        {
            isGood = true;
            levelUpCount++;
            GameManager.Instance.UpdateSprite(spriteRenderer, levelUpCount); 
        }

        // 쿨다운 시작
        StartCoroutine(LevelUpCooldown());
    }

    private IEnumerator AutoLevelUp()
    {
        // 레벨이 1 이상이 될 때까지 대기
        while (levelUpCount < 1)
        {
            yield return null;
        }

        // 레벨이 1 이상이 되면 계속해서 레벨업 시도
        while (levelUpCount < maxLevel)
        {
            // 5초에서 15초 사이의 랜덤 시간을 대기
            float randomTime = Random.Range(5f, 15f);
            yield return new WaitForSeconds(randomTime);

            ChangeGround(); // 레벨업 시도
        }
    }

    private Sprite GetSpriteForCurrentLevel()
    {
        return BadGround; // 레벨에 해당하는 스프라이트가 없으면 기본 스프라이트로 설정
    }

    private IEnumerator LevelUpCooldown()
    {
        canLevelUp = false; // 레벨업 불가 상태로 설정
        yield return new WaitForSeconds(levelUpCooldown); // 쿨다운 시간 대기
        canLevelUp = true; // 다시 레벨업 가능 상태로 설정
    }

    public void ToBad()
    {
        levelUpCount = 0;
        isGood = false;
        spriteRenderer.sprite = BadGround;
    }
}
