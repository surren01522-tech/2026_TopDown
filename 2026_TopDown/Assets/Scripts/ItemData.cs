using UnityEngine;

// 아이템 대분류
public enum InventoryItemType { Potion, Weapon, Scroll }

[System.Serializable]
public class ItemData
{
    public string realName;       // 진짜 이름 (예: "치료 물약", "강화 주문서")
    public string unknownName;    // 식별 전 이름 (예: "붉은 물약", "흐릿한 주문서")
    public InventoryItemType type;
    public int value;             // 효과 수치
    public bool isIdentified;     // 💡 핵심: 현재 이 아이템이 식별되었는가?

    // 인스펙터나 코드에서 쉽게 생성하기 위한 생성자
    public ItemData(string realName, string unknownName, InventoryItemType type, int value, bool isIdentified = false)
    {
        this.realName = realName;
        this.unknownName = unknownName;
        this.type = type;
        this.value = value;
        this.isIdentified = isIdentified;
    }

    // 👁️ 현재 식별 상태에 따라 유저에게 보여줄 이름을 반환하는 함수
    public string GetDisplayName()
    {
        return isIdentified ? realName : unknownName;
    }
}
