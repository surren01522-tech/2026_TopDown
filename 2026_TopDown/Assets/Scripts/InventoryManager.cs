using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    // 🎒 플레이어의 가방
    public List<ItemData> items = new List<ItemData>();

    private Dictionary<string, string> potionDic = new Dictionary<string, string>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // 기본 도감 데이터 세팅
        potionDic.Add("potion_red", "치료 물약");
        potionDic.Add("potion_blue", "마나 물약");
        potionDic.Add("scroll_old", "시야 확장 주문서");
    }

    // ➕ 가방에 아이템 넣기
    public void AddItem(string tileName)
    {
        string nameKey = tileName.ToLower();
        ItemData newItem = null;

        if (nameKey.Contains("potion"))
        {
            string real = potionDic.ContainsKey(nameKey) ? potionDic[nameKey] : "미지의 물약";
            newItem = new ItemData(real, "정체불명의 물약", InventoryItemType.Potion, 30);
        }
        else if (nameKey.Contains("scroll"))
        {
            string real = potionDic.ContainsKey(nameKey) ? potionDic[nameKey] : "미지의 주문서";
            newItem = new ItemData(real, "오래된 주문서", InventoryItemType.Scroll, 1);
        }
        else if (nameKey.Contains("weapon") || nameKey.Contains("sword"))
        {
            newItem = new ItemData("날카로운 검", "낡은 검", InventoryItemType.Weapon, 5, true);
        }

        if (newItem != null)
        {
            items.Add(newItem);

            // 💡 [해결책 1 적용] 이모지 대신 [획득] 기호 사용
            if (LogUIManager.Instance != null)
            {
                LogUIManager.Instance.AddLog($"[획득] 가방에 추가됨: {newItem.GetDisplayName()}", Color.yellow);
            }
        }
    }

    // 🥤 아이템 사용하기 (인덱스 번호 기반)
    public void UseItem(int index, PlayerController player)
    {
        if (index < 0 || index >= items.Count) return;

        ItemData item = items[index];

        switch (item.type)
        {
            case InventoryItemType.Potion:
                player.playerHP += item.value;
                // 💡 [해결책 1 적용] 이모지 대신 [회복] 기호 사용
                if (LogUIManager.Instance != null)
                {
                    LogUIManager.Instance.AddLog($"[회복] {item.GetDisplayName()}을(를) 마셨습니다! HP +{item.value} (현재 HP: {player.playerHP})", Color.white);
                }
                break;

            case InventoryItemType.Scroll:
                FieldOfView fov = FindFirstObjectByType<FieldOfView>();
                if (fov != null)
                {
                    fov.viewRadius += item.value;
                    fov.RevealMap(player.transform.position);
                    // 💡 [해결책 1 적용] 이모지 대신 [효과] 기호 사용
                    if (LogUIManager.Instance != null)
                    {
                        LogUIManager.Instance.AddLog($"[효과] {item.GetDisplayName()}을(를) 읽었습니다! 시야가 넓어집니다.", Color.white);
                    }
                }
                break;

            case InventoryItemType.Weapon:
                player.playerAttack += item.value;
                // 💡 [해결책 1 적용] 이모지 대신 [장착] 기호 사용
                if (LogUIManager.Instance != null)
                {
                    LogUIManager.Instance.AddLog($"[장착] {item.GetDisplayName()}을(를) 장착했습니다! 공격력 +{item.value}", Color.white);
                }
                break;
        }

        if (!item.isIdentified)
        {
            item.isIdentified = true;

            // 💡 [해결책 1 적용] 이모지 대신 [식별] 기호 사용
            if (LogUIManager.Instance != null)
            {
                LogUIManager.Instance.AddLog($"[식별] 진짜 정체는 '{item.realName}' 이었습니다!", Color.green);
            }

            foreach (var inventoryItem in items)
            {
                if (inventoryItem.realName == item.realName)
                {
                    inventoryItem.isIdentified = true;
                }
            }
        }

        if (item.type != InventoryItemType.Weapon)
        {
            items.RemoveAt(index);
        }
    }

    private void Update()
    {
        PlayerController player = FindFirstObjectByType<PlayerController>();
        if (player == null) return;

        if (Input.GetKeyDown(KeyCode.Alpha1)) UseItem(0, player);
        if (Input.GetKeyDown(KeyCode.Alpha2)) UseItem(1, player);
    }
}