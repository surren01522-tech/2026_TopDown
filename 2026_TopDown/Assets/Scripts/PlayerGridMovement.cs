using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerGridMovement : MonoBehaviour
{
    [Header("설정")]
    [SerializeField] private Tilemap groundTilemap; // 바닥 타일맵
    [SerializeField] private Tilemap obstacleTilemap; // 장애물 타일맵 (선택)
    [SerializeField] private float moveSpeed = 5f; // 이동 속도

    private Vector3Int currentCell;
    private bool isMoving = false;

    void Start()
    {
        // 1. 게임 시작 시 현재 월드 좌표를 기준으로 그리드 셀 좌표 계산
        currentCell = groundTilemap.WorldToCell(transform.position);

        // 2. 캐릭터 위치를 타일의 정확한 중앙으로 정렬
        transform.position = groundTilemap.GetCellCenterWorld(currentCell);
    }

    void Update()
    {
        // 이동 중이 아닐 때만 키 입력 허용 (턴제 규칙)
        if (!isMoving)
        {
            CheckInput();
        }
    }

    private void CheckInput()
    {
        Vector3Int direction = Vector3Int.zero;

        // GetAxisRaw 대신 GetKeyDown을 사용하여 1키당 1칸씩만 이동
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) direction = Vector3Int.up;
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) direction = Vector3Int.down;
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) direction = Vector3Int.left;
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) direction = Vector3Int.right;

        // 입력이 있었다면 이동 시도
        if (direction != Vector3Int.zero)
        {
            TryMove(direction);
        }
    }

    private void TryMove(Vector3Int direction)
    {
        Vector3Int targetCell = currentCell + direction;

        // [검증 1] 바닥 타일이 존재하는가?
        if (!groundTilemap.HasTile(targetCell)) return;

        // [검증 2] 장애물 타일이 앞을 가로막고 있는가?
        if (obstacleTilemap != null && obstacleTilemap.HasTile(targetCell)) return;

        // 이동이 가능하므로 코루틴 실행 및 턴 소모 처리
        StartCoroutine(SmoothMove(targetCell));

        // TODO:여기에 적(Enemy)들의 턴을 실행하는 함수를 호출하세요. (예: TurnManager.Instance.OnPlayerTurnEnd();)
    }

    private IEnumerator SmoothMove(Vector3Int targetCell)
    {
        isMoving = true;

        // 목표 타일의 월드 중앙 좌표 가져오기
        Vector3 targetPosition = groundTilemap.GetCellCenterWorld(targetCell);

        // 부드럽게 이동하는 루프
        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }

        // 이동 후 정확한 위치로 고정
        transform.position = targetPosition;
        currentCell = targetCell;

        isMoving = false;
    }
}
