using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Scriptble Object/ItemData")]   // 커스텀 메뉴를 생성하는 속성
public class ItemData : ScriptableObject
{
    public enum ItemType { Melee, Range, Glove, Shoe, Heal }    // enum으로 아이템 타입 배열 생성

    [Header("# Main Info")]
    public ItemType itemType;
    public int itemId;
    public string itemName;
    [TextArea]  // 인스펙터에 텍스트를 여러줄 넣어줄 수 있게 TextArea 속성 부여
    public string itemDesc;
    public Sprite itemIcon;


    [Header("# Level Data")]
    public float baseDamage;    // 0레벨 데미지를 저장할 변수
    public int baseCount;   // 0레벨 관통력 or 근접무기 갯수를 저장할 변수
    public float[] damages;
    public int[] counts;

    [Header("# Weapon")]
    public GameObject projectile;
    public Sprite hand; // 스크립트블 오브젝트에서 손 스프라이트를 담을 속성 추가
}
