using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyTilemapTrigger : MonoBehaviour
{
    private Tilemap enemyTilemap;

    private void Awake()
    {
        enemyTilemap = GetComponent<Tilemap>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player == null) return;

            Vector3 playerPos = collision.transform.position;

            // 🎯 플레이어 중심점 기준 격자 계산
            Vector3Int playerCell = new Vector3Int(
                Mathf.FloorToInt(playerPos.x),
                Mathf.FloorToInt(playerPos.y),
                0
            );

            TileBase steppedTile = enemyTilemap.GetTile(playerCell);

            // 💡 [오차 방어] 혹시 한 발짝 걸쳤다면 주변 9칸 스캔해서 적 타일 정밀 탐색
            if (steppedTile == null)
            {
                for (int x = -1; x <= 1; x++)
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        Vector3Int checkPos = new Vector3Int(playerCell.x + x, playerCell.y + y, 0);
                        TileBase tempTile = enemyTilemap.GetTile(checkPos);
                        if (tempTile != null && tempTile.name.ToLower().Contains("enemy"))
                        {
                            playerCell = checkPos;
                            steppedTile = tempTile;
                            break;
                        }
                    }
                    if (steppedTile != null) break;
                }
            }

            // ⚔️ 적 타일을 확실히 찾았다면 전투 시작!
            if (steppedTile != null)
            {
                string enemyName = steppedTile.name;
                Debug.Log($"⚔️ [전투 발생] {enemyName}와 부딪혔습니다! 격자 좌표: {playerCell}");

                // 💥 전투 로직 실행 (예: 슬라임, 스켈레톤 등 이름에 따라 차등 대미지 가능)
                ExecuteBattle(enemyName, player);

                // 💀 적 처치 처리: 해당 칸의 적 타일을 삭제합니다.
                enemyTilemap.SetTile(playerCell, null);
            }
        }
    }

    // 🔮 간단한 턴제 전투 계산기
    private void ExecuteBattle(string enemyName, PlayerController player)
    {
        // 예시: 적의 기본 체력을 30이라고 가정
        int enemyHP = 30;
        int enemyAttack = 15; // 적이 플레이어를 때리는 공격력

        // 이름별로 스펙 조절 가능
        if (enemyName.ToLower().Contains("boss"))
        {
            enemyHP = 100;
            enemyAttack = 30;
            Debug.Log("🚨 보스 몬스터와 조우했습니다!");
        }

        // 1. 플레이어가 먼저 적을 공격합니다.
        enemyHP -= player.playerAttack;
        Debug.Log($"⚔️ 플레이어가 {player.playerAttack}의 데미지로 공격! (적 잔여 HP: {Mathf.Max(0, enemyHP)})");

        // 2. 적이 살아남았다면 플레이어를 반격합니다.
        if (enemyHP > 0)
        {
            player.playerHP -= enemyAttack;
            Debug.Log($"💥 적의 반격! 플레이어가 {enemyAttack}의 데미지를 입었습니다. (플레이어 현재 HP: {player.playerHP})");

            // 플레이어 사망 체크 (기존 PlayerController에 있던 GameOver 연동)
            if (player.playerHP <= 0)
            {
                Debug.LogError("💀 플레이어의 체력이 0이 되었습니다.");
                if (GameManager.Instance != null) GameManager.Instance.GameOver();
            }
        }
        else
        {
            Debug.Log($"✨ {enemyName}를 무찌르고 승리했습니다!");
        }
    }
}