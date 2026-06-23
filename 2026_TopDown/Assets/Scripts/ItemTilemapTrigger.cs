using UnityEngine;
using UnityEngine.Tilemaps;

public class ItemTilemapTrigger : MonoBehaviour
{
    private Tilemap itemTilemap;

    private void Awake()
    {
        itemTilemap = GetComponent<Tilemap>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Vector3 playerPos = collision.transform.position;

            Vector3Int playerCell = new Vector3Int(
                Mathf.FloorToInt(playerPos.x),
                Mathf.FloorToInt(playerPos.y),
                0
            );

            TileBase steppedTile = itemTilemap.GetTile(playerCell);

            // 주변 9칸을 뒤져서 아이템 타일을 찾는 오차 방어선
            if (steppedTile == null)
            {
                for (int x = -1; x <= 1; x++)
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        Vector3Int checkPos = new Vector3Int(playerCell.x + x, playerCell.y + y, 0);
                        TileBase tempTile = itemTilemap.GetTile(checkPos);

                        if (tempTile != null)
                        {
                            string tileName = tempTile.name.ToLower();
                            if (tileName.Contains("potion") || tileName.Contains("weapon") || tileName.Contains("sword") || tileName.Contains("scroll"))
                            {
                                playerCell = checkPos;
                                steppedTile = tempTile;
                                break;
                            }
                        }
                    }
                    if (steppedTile != null) break;
                }
            }

            // 💥 [여기서부터 주목!] 타일을 성공적으로 찾았을 때
            if (steppedTile != null)
            {
                PlayerController player = collision.GetComponent<PlayerController>();
                if (player != null)
                {
                    // ❌ [기존 방식] ApplyTileItemEffectByName(...) 함수를 지웠습니다.
                    // 이제 길 가다 물약을 밟아도 그 자리에서 '꿀꺽' 마시지 않습니다.

                    // 🎒 [새로운 방식] 방금 만든 가방(InventoryManager)에 타일의 이름을 배달합니다!
                    if (InventoryManager.Instance != null)
                    {
                        InventoryManager.Instance.AddItem(steppedTile.name);
                    }

                    // 🧹 바닥의 타일을 지워주는 것은 그대로 유지합니다 (먹었으니까 맵에선 사라져야죠!)
                    itemTilemap.SetTile(playerCell, null);
                }
            }
        }
    }

    // 💡 기존에 있던 ApplyTileItemEffectByName 함수는 이제 필요 없으므로 완전히 지우셔도 됩니다!
}