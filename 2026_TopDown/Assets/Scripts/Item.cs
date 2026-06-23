using UnityEngine;

public class Item : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public enum ItemType
    {
        Potion,   // 물약 (체력 회복 등)
        Weapon,   // 무기 (공격력 상승)
        Scroll,   // 주문서 (시야 확장, 순간이동 등)
        Gold      // 골드/재화
    }
}
