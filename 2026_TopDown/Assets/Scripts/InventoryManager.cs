using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    // 어디서나 접근할 수 있게 만드는 싱글톤 변수
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

        // 타일 이름 분석해서 알맞은 데이터 매칭
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

            // 🔥 [UI 연동] 노란색 글씨로 아이템 획득 메시지를 화면에 띄웁니다!
            if (LogUIManager.Instance != null)
            {
                LogUIManager.Instance.AddLog($"🎒 가방에 추가됨: {newItem.GetDisplayName()}", Color.yellow);
            }
        }
    }

    // 🥤 아이템 사용하기 (인덱스 번호 기반)
    public void UseItem(int index, PlayerController player)
    {
        if (index < 0 || index >= items.Count) return;

        ItemData item = items[index];

        // 1. 아이템 종류별 효과 발동 및 UI 메시지 출력
        switch (item.type)
        {
            case InventoryItemType.Potion:
                player.playerHP += item.value;
                if (LogUIManager.Instance != null)
                {
                    LogUIManager.Instance.AddLog($"❤️ {item.GetDisplayName()}을(를) 마셨습니다! HP +{item.value} (현재 HP: {player.playerHP})", Color.white);
                }
                break;

            case InventoryItemType.Scroll:
                FieldOfView fov = FindFirstObjectByType<FieldOfView>();
                if (fov != null)
                {
                    fov.viewRadius += item.value;
                    fov.RevealMap(player.transform.position);
                    if (LogUIManager.Instance != null)
                    {
                        LogUIManager.Instance.AddLog($"👁️ {item.GetDisplayName()}을(를) 읽었습니다! 시야가 넓어집니다.", Color.white);
                    }
                }
                break;

            case InventoryItemType.Weapon:
                player.playerAttack += item.value;
                if (LogUIManager.Instance != null)
                {
                    LogUIManager.Instance.AddLog($"⚔️ {item.GetDisplayName()}을(를) 장착했습니다! 공격력 +{item.value}", Color.white);
                }
                break;
        }

        // 2. 미식별 아이템이었다면, 사용한 순간 정체가 탄록납니다!
        if (!item.isIdentified)
        {
            item.isIdentified = true;

            // 🔥 [UI 연동] 연두색 글씨로 식별 완료 메시지를 화면에 띄웁니다!
            if (LogUIManager.Instance != null)
            {
                LogUIManager.Instance.AddLog($"💡 [식별 완료!] 진짜 정체는 '{item.realName}' 이었습니다!", Color.green);
            }

            // 같은 판에서 획득한 동일한 종류의 물약들도 전부 자동으로 식별 처리합니다.
            foreach (var inventoryItem in items)
            {
                if (inventoryItem.realName == item.realName)
                {
                    inventoryItem.isIdentified = true;
                }
            }
        }

        // 3. 소모품(물약, 주문서)이라면 가방에서 제거
        if (item.type != InventoryItemType.Weapon)
        {
            items.RemoveAt(index);
        }
    }

    private void Update()
    {
        PlayerController player = FindFirstObjectByType<PlayerController>();
        if (player == null) return;

        if (Input.GetKeyDown(KeyCode.Alpha1)) UseItem(0, player); // 숫자 1키로 1번째 아이템 사용
        if (Input.GetKeyDown(KeyCode.Alpha2)) UseItem(1, player); // 숫자 2키로 2번째 아이템 사용
    }
}