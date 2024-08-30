using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Tilemaps;
public class GameManager : Singleton<GameManager>
{
    public Vector2 Size; // 맵의 크기
    public GameObject Ground; // 생성할 오브

    public GameObject monther,son;
  
    [Button("Place Fwang Tiles")] // OdinInspector를 사용한 버튼으로 에디터에서 바로 실행 가능
    private void PlaceFwangTiles()
    {
        // 기존의 Fwang 그룹 오브젝트가 있다면 제거
        GameObject existingParent = GameObject.Find("FwangParent");
        if (existingParent != null)
        {
            DestroyImmediate(existingParent);
        }

        // 새로운 빈 오브젝트를 생성하여 Fwang 오브젝트들의 부모로 설정
        GameObject fwangParent = new GameObject("FwangParent");

        // Size만큼 Fwang 오브젝트 생성
        for (int x = 0; x < Size.x; x++)
        {
            for (int y = 0; y < Size.y; y++)
            {
                Vector2 position = new Vector2(x, y);
                GameObject newFwang = Instantiate(Ground, position, Quaternion.identity);
                newFwang.name = $"Fwang_{x}_{y}"; // 오브젝트의 이름 지정
                newFwang.transform.parent = fwangParent.transform; // FwangParent의 자식으로 설정
            }
        }
    }
}
