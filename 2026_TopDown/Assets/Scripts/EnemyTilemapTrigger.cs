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

            // 주변 격자 탐색 보정 로직
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

            if (steppedTile != null)
            {
                string enemyName = steppedTile.name;

                // ⚔️ 전투 시스템 시작!
                ExecuteBattle(enemyName, player);

                // 밟은 적 타일 제거
                enemyTilemap.SetTile(playerCell, null);
            }
        }
    }

    private void ExecuteBattle(string enemyName, PlayerController player)
    {
        int enemyHP = 30;
        int enemyAttack = 15;

        // 보스 전용 스펙 조절 및 경고 알림
        if (enemyName.ToLower().Contains("boss"))
        {
            enemyHP = 100;
            enemyAttack = 30;

            if (LogUIManager.Instance != null)
            {
                LogUIManager.Instance.AddLog("[경고] 강력한 보스 몬스터와 마주쳤습니다!", new Color(1f, 0.5f, 0f));
            }
            else
            {
                // UI 매니저가 없을 때를 대비한 디버그 예외 처리
                Debug.Log("[경고] 강력한 보스 몬스터와 마주쳤습니다!");
            }
        }

        // 1. 플레이어 선제 공격
        enemyHP -= player.playerAttack;
        string playerAttackMsg = $"[전투] 플레이어가 {enemyName}을(를) 공격하여 {player.playerAttack}의 피해를 줬습니다!";

        if (LogUIManager.Instance != null) LogUIManager.Instance.AddLog(playerAttackMsg, Color.white);
        else Debug.Log(playerAttackMsg);

        // 2. 적 반격 (살아남았을 경우)
        if (enemyHP > 0)
        {
            player.playerHP -= enemyAttack;
            string enemyCounterMsg = $"[피해] {enemyName}의 반격! {enemyAttack}의 대미지를 입었습니다. (현재 HP: {player.playerHP})";

            if (LogUIManager.Instance != null) LogUIManager.Instance.AddLog(enemyCounterMsg, Color.red);
            else Debug.Log(enemyCounterMsg);

            // 플레이어 사망 판단
            if (player.playerHP <= 0)
            {
                string deadMsg = "[사망] 플레이어가 쓰러졌습니다...";
                if (LogUIManager.Instance != null) LogUIManager.Instance.AddLog(deadMsg, Color.red);
                else Debug.Log(deadMsg);

                if (GameManager.Instance != null) GameManager.Instance.GameOver();
            }
        }
        else
        {
            // 적 처치 성공
            string victoryMsg = $"[승리] {enemyName}을(를) 처치하고 승리했습니다!";
            if (LogUIManager.Instance != null) LogUIManager.Instance.AddLog(victoryMsg, Color.cyan);
            else Debug.Log(victoryMsg);
        }
    }
}