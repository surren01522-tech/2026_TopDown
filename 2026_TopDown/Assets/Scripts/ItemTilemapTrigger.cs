using UnityEngine;
using UnityEngine.Tilemaps;

public class ItemTilemapTrigger : MonoBehaviour
{
    private Tilemap itemTilemap;

    [Header("아이템 종류별 타일 등록 (인펙터에서 매핑)")]
    public TileBase potionTile;
    public TileBase weaponTile;
    public TileBase scrollTile;

    private void Awake()
    {
        itemTilemap = GetComponent<Tilemap>();
    }

    // 🚪 물리 충돌(Is Trigger)이 일어났을 때 실행
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // 1. 플레이어의 현재 위치를 타일 격자 좌표(Vector3Int)로 변환합니다.
            Vector3Int playerCell = itemTilemap.layoutGrid.WorldToCell(collision.transform.position);

            // 2. 플레이어 발밑에 어떤 타일이 깔려있는지 낚아챕니다.
            TileBase steppedTile = itemTilemap.GetTile(playerCell);

            if (steppedTile != null)
            {
                PlayerController player = collision.GetComponent<PlayerController>();
                if (player != null)
                {
                    // 3. 어떤 아이템 타일인지 식별하고 효과를 줍니다.
                    ApplyTileItemEffect(steppedTile, player);

                    // 4. [중요] 먹었으니 해당 자리의 아이템 타일을 지워서 바닥을 비워줍니다!
                    itemTilemap.SetTile(playerCell, null);
                }
            }
        }
    }

    // 🔮 타일 종류를 판별해서 플레이어의 스탯을 올려주는 함수
    private void ApplyTileItemEffect(TileBase tile, PlayerController player)
    {
        if (tile == potionTile)
        {
            player.playerHP += 20;
            Debug.Log($"❤️ [타일 아이템] 포션 획득! 현재 HP: {player.playerHP}");
        }
        else if (tile == weaponTile)
        {
            player.playerAttack += 5;
            Debug.Log($"⚔️ [타일 아이템] 검 획득! 현재 공격력: {player.playerAttack}");
        }
        else if (tile == scrollTile)
        {
            FieldOfView fov = FindFirstObjectByType<FieldOfView>();
            if (fov != null)
            {
                fov.viewRadius += 1;
                fov.RevealMap(player.transform.position);
                Debug.Log($"👁️ [타일 아이템] 주문서 획득! 현재 시야: {fov.viewRadius}");
            }
        }
    }
}
