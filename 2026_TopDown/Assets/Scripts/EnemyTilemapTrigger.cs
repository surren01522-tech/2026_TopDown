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

            Vector3Int playerCell = new Vector3Int(
                Mathf.FloorToInt(playerPos.x),
                Mathf.FloorToInt(playerPos.y),
                0
            );

            TileBase steppedTile = enemyTilemap.GetTile(playerCell);

            // 주변 9칸을 뒤져서 적 타일을 정밀 탐색
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

                // 전투 로직 실행
                ExecuteBattle(enemyName, player);

                // 적 처치 처리: 해당 칸의 적 타일을 삭제합니다.
                enemyTilemap.SetTile(playerCell, null);
            }
        }
    }

    // 🔮 턴제 전투 계산기 (UI 연동)
    private void ExecuteBattle(string enemyName, PlayerController player)
    {
        int enemyHP = 30;
        int enemyAttack = 15; // 적이 플레이어를 때리는 공격력

        if (enemyName.ToLower().Contains("boss"))
        {
            enemyHP = 100;
            enemyAttack = 30;
            // 🔥 [UI 연동] 보스 조우 시 주황색 경고 메시지!
            if (LogUIManager.Instance != null)
            {
                LogUIManager.Instance.AddLog("🚨 강력한 보스 몬스터와 마주쳤습니다!", new Color(1f, 0.5f, 0f));
            }
        }

        // 1. 플레이어가 먼저 적을 공격합니다.
        enemyHP -= player.playerAttack;
        // 🔥 [UI 연동] 플레이어 공격 상황을 흰색 글로 출력!
        if (LogUIManager.Instance != null)
        {
            LogUIManager.Instance.AddLog($"⚔️ 플레이어가 {enemyName}을(를) 공격하여 {player.playerAttack}의 피해를 줬습니다!", Color.white);
        }

        // 2. 적이 살아남았다면 플레이어를 반격합니다.
        if (enemyHP > 0)
        {
            player.playerHP -= enemyAttack;
            // 🔥 [UI 연동] 적의 반격 및 피해 상황을 빨간색 글로 출력!
            if (LogUIManager.Instance != null)
            {
                LogUIManager.Instance.AddLog($"💥 {enemyName}의 반격! {enemyAttack}의 대미지를 입었습니다. (현재 HP: {player.playerHP})", Color.red);
            }

            // 플레이어 사망 체크
            if (player.playerHP <= 0)
            {
                if (LogUIManager.Instance != null)
                {
                    LogUIManager.Instance.AddLog("💀 플레이어가 쓰러졌습니다...", Color.red);
                }
                if (GameManager.Instance != null) GameManager.Instance.GameOver();
            }
        }
        else
        {
            // 🔥 [UI 연동] 적 처치 성공 시 하늘색 글로 출력!
            if (LogUIManager.Instance != null)
            {
                LogUIManager.Instance.AddLog($"✨ {enemyName}을(를) 처치하고 승리했습니다!", Color.cyan);
            }
        }
    }
}