using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

public class Ground : SerializedMonoBehaviour
{
    public int levelUpCount;
    public bool isGood;

    [SerializeField] Sprite BadGround;
    private SpriteRenderer spriteRenderer;



    public float levelUpCooldown = 2f; // 레벨업 쿨다운 시간
    private bool canLevelUp = true; // 레벨업 가능 여부

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = BadGround;
    }

    public void ChangeGround()
    {
        if (!canLevelUp) return; // 쿨다운 중이면 레벨업 불가

        if (isGood)
        {
            levelUpCount++;
            GameManager.Instance.UpdateSprite(spriteRenderer,levelUpCount); // 레벨이 올라갈 때 스프라이트 교체 시도
            //StartCoroutine(LevelUpCooldown()); // 쿨다운 시작
        }
        else
        {
            isGood = true;
            levelUpCount++;
            GameManager.Instance.UpdateSprite(spriteRenderer,levelUpCount); 
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
}
