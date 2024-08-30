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

    // Odin Inspector를 사용해 인스펙터에서 설정 가능한 레벨별 스프라이트 딕셔너리
    [OdinSerialize,DictionaryDrawerSettings(KeyLabel = "Level", ValueLabel = "Sprite")]
    public Dictionary<int, Sprite> levelSprites;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = BadGround;
    }

    public void ChangeGround()
    {
        if (isGood)
        {
            levelUpCount++;
            UpdateSprite(); // 레벨이 올라갈 때 스프라이트 교체 시도
        }
        else
        {
            isGood = true;
            spriteRenderer.sprite = GetSpriteForCurrentLevel();
        }
    }

    private void UpdateSprite()
    {
        if (levelSprites.ContainsKey(levelUpCount))
        {
            spriteRenderer.sprite = levelSprites[levelUpCount];
        }
    }

    private Sprite GetSpriteForCurrentLevel()
    {
        return BadGround; // 레벨에 해당하는 스프라이트가 없으면 기본 스프라이트로 설정
    }
}
