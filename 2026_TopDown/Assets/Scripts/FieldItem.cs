using UnityEngine;
using static Item;

public class FieldItem : MonoBehaviour
{
    [Header("Item Settings")]
    public string itemName;      // 아이템 이름
    public ItemType itemType;    // 아이템 종류
    public int value = 20;       // 효과 수치 (회복량이나 공격력 증가치 등)

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 플레이어가 아이템을 밟았을 때
        if (collision.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null)
            {
                ApplyItemEffect(player);

                // 효과 적용 후 필드의 아이템 오브젝트는 삭제
                Destroy(gameObject);
            }
        }
    }

    // 🔮 아이템 종류에 따라 플레이어에게 효과를 주는 핵심 함수
    private void ApplyItemEffect(PlayerController player)
    {
        Debug.Log($"🎒 아이템 획득: {itemName}");

        switch (itemType)
        {
            case ItemType.Potion:
                // 플레이어 HP 회복 (기존 체력 + 포션 수치)
                player.playerHP += value;
                Debug.Log($"❤️ 체력 회복! 현재 HP: {player.playerHP}");
                break;

            case ItemType.Weapon:
                // 플레이어 공격력 영구 상승
                player.playerAttack += value;
                Debug.Log($"⚔️ 공격력 상승! 현재 공격력: {player.playerAttack}");
                break;

            case ItemType.Scroll:
                // 로그라이크 특수 효과 예시: 시야 반지름 확장
                FieldOfView fov = FindFirstObjectByType<FieldOfView>();
                if (fov != null)
                {
                    fov.viewRadius += 1;
                    Debug.Log($"👁️ 시야가 넓어졌습니다! 현재 시야: {fov.viewRadius}");
                    // 변경된 시야 즉시 갱신
                    fov.RevealMap(player.transform.position);
                }
                break;

            case ItemType.Gold:
                // 데이터 매니저가 있다면 골드 추가
                if (GameDataManager.Instance != null)
                {
                    // GameDataManager에 골드 변수와 함수가 있다면 주석을 풀고 연동하세요.
                    // GameDataManager.Instance.AddGold(value);
                }
                break;
        }
    }
}
