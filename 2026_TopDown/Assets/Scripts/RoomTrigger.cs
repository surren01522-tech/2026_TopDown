using UnityEngine;

public class RoomTrigger : MonoBehaviour
{
    private BoxCollider2D roomCollider;
    private bool isRevealed = false; // 이미 밝혀진 방인지 체크하는 변수

    private void Awake()
    {
        roomCollider = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 이미 밝혀진 방이거나, 부딪힌 오브젝트가 플레이어가 아니라면 무시합니다.
        if (isRevealed || !collision.CompareTag("Player")) return;

        if (FieldOfView.Instance != null && roomCollider != null)
        {
            // BoxCollider2D의 bounds(경계 영역)를 가져와 최소/최대 월드 좌표를 구합니다.
            Vector2 minPos = roomCollider.bounds.min;
            Vector2 maxPos = roomCollider.bounds.max;

            // FOV 스크립트에게 이 영역 전체를 밝히라고 명령합니다.
            FieldOfView.Instance.RevealRoom(minPos, maxPos);

            isRevealed = true; // 다시 밟아도 연산하지 않도록 방 고정
        }
    }
}
