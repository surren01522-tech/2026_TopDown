using UnityEngine;

public class MapTransitionTrigger : MonoBehaviour
{
    [Header("충돌 감지할 플레이어 태그")]
    public string playerTag = "Player";

    // 🚪 물리적인 충돌(Is Trigger가 켜진 상태)이 일어났을 때 자동으로 실행되는 유니티 함수
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 부딪힌 오브젝트의 태그가 플레이어인지 확인합니다.
        if (collision.CompareTag(playerTag))
        {
            Debug.Log("🎯 플레이어가 다음 맵 전환 타일에 도달했습니다!");

            // GameManager에 만들어 둔 랜덤 맵 로드 함수를 호출합니다.
            if (GameManager.Instance != null)
            {
                GameManager.Instance.LoadRandomMapScene();
            }
            else
            {
                Debug.LogWarning("GameManager 인스턴스를 찾을 수 없습니다.");
            }
        }
    }
}