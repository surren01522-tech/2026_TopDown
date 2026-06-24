using UnityEngine;

public class GameClearTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 1. 충돌한 물체가 플레이어인지 확인 (Tag가 Player로 설정되어 있어야 함)
        if (collision.CompareTag("Player"))
        {
            Debug.Log("플레이어가 클리어 지점에 도달했습니다!");

            // 2. 좌측 하단/전체 로그에 클리어 메시지 남기 (기존 시스템 활용)
            if (LogUIManager.Instance != null)
            {
                LogUIManager.Instance.AddLog("축하합니다! 던전을 탈출하여 게임을 클리어했습니다!", Color.yellow);
            }

            // 3. 클리어 UI 띄우기
            GameClearUIManager.Instance.ShowClearPanel();

            // 4. (선택사항) 플레이어 조작 멈추기
            // collision.GetComponent<PlayerController>().enabled = false;
        }
    }
}
