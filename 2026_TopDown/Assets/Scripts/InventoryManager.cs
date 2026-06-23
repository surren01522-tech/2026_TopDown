using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    // ✨ [이 부분이 누락되었거나 이름이 틀렸을 확률이 높습니다!] ✨
    // 어디서나 InventoryManager.Instance로 접근할 수 있게 만드는 싱글톤 변수입니다.
    public static InventoryManager Instance { get; private set; }


    // 🎒 플레이어의 가방
    public List<ItemData> items = new List<ItemData>();

    private Dictionary<string, string> potionDic = new Dictionary<string, string>();

    private void Awake()
    {
        // ✨ [싱글톤 초기화 영역] ✨
        // 문지기 역할: 이 세상에 인벤토리 매니저는 단 하나만 존재하도록 강제합니다.
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // 기존 도감 데이터 세팅...
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
            // 무기는 보통 처음부터 식별되어 있는 경우가 많으므로 true 세팅
            newItem = new ItemData("날카로운 검", "낡은 검", InventoryItemType.Weapon, 5, true);
        }

        if (newItem != null)
        {
            items.Add(newItem);
            Debug.Log($"🎒 가방에 추가됨: {newItem.GetDisplayName()} (현재 가방 아이템 수: {items.Count}개)");
            ShowInventoryLog();
        }
    }

    // 🥤 아이템 사용하기 (인덱스 번호 기반)
    public void UseItem(int index, PlayerController player)
    {
        if (index < 0 || index >= items.Count) return;

        ItemData item = items[index];
        Debug.Log($"🔮 {item.GetDisplayName()} 아이템 사용 시도!");

        // 1. 아이템 종류별 효과 발동
        switch (item.type)
        {
            case InventoryItemType.Potion:
                player.playerHP += item.value;
                Debug.Log($"❤️ [포션 복용] {item.GetDisplayName()}을(를) 마셨습니다! HP +{item.value} (현재 HP: {player.playerHP})");
                break;

            case InventoryItemType.Scroll:
                FieldOfView fov = FindFirstObjectByType<FieldOfView>();
                if (fov != null)
                {
                    fov.viewRadius += item.value;
                    fov.RevealMap(player.transform.position);
                    Debug.Log($"👁️ [주문서 낭독] 시야가 확장되었습니다! (현재 시야: {fov.viewRadius})");
                }
                break;

            case InventoryItemType.Weapon:
                player.playerAttack += item.value;
                Debug.Log($"⚔️ [무기 장착] 공격력이 영구히 상승했습니다! +{item.value} (현재 공격력: {player.playerAttack})");
                break;
        }

        // 2. ⭐ [핵심 식별 메커니즘] 미식별 아이템이었다면, 사용한 순간 정체가 탄록납니다!
        if (!item.isIdentified)
        {
            item.isIdentified = true;
            Debug.Log($"💡 [식별 완료!] 이 아이템의 진짜 정체는 '{item.realName}' 이었습니다!");

            // 💥 [정통 로그 시스템] 같은 판에서 획득한 동일한 종류의 물약들도 전부 자동으로 식별 처리합니다.
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

        ShowInventoryLog();
    }

    // 콘솔창에 현재 인벤토리 목록을 보여주는 편리한 디버그용 함수
    public void ShowInventoryLog()
    {
        string currentList = "--- 🎒 현재 가방 목록 --- \n";
        for (int i = 0; i < items.Count; i++)
        {
            currentList += $"[{i + 1}번] {items[i].GetDisplayName()}\n";
        }
        Debug.Log(currentList);
    }

    // ⌨️ 테스트용 키보드 입력 체크 (1번, 2번 키를 누르면 아이템 소비)
    private void Update()
    {
        PlayerController player = FindFirstObjectByType<PlayerController>();
        if (player == null) return;

        if (Input.GetKeyDown(KeyCode.Alpha1)) UseItem(0, player); // 숫자 1키로 1번째 아이템 사용
        if (Input.GetKeyDown(KeyCode.Alpha2)) UseItem(1, player); // 숫자 2키로 2번째 아이템 사용
    }
}
