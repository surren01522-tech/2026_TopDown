using UnityEngine;
using UnityEngine.Tilemaps;

public class FieldOfView : MonoBehaviour
{
    public static FieldOfView Instance;

    [Header("Tilemap References")]
    public Tilemap fogTilemap; // 어둠을 담당하는 타일맵

    [Header("FOV Settings")]
    [Range(1, 10)]
    public int viewRadius = 2; // 플레이어 주변 몇 칸까지 밝힐지 결정 (반지름)

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // 기존 코드가 싱글톤을 덮어쓰지 않도록 확실하게 현재 오브젝트로 고정합니다.
        Instance = this;
    }

    /// <summary>
    /// 플레이어의 현재 월드 위치를 기준으로 주변의 안개를 지웁니다.
    /// </summary>
    public void RevealMap(Vector3 playerWorldPos)
    {
        if (fogTilemap == null) return;

        // 플레이어의 월드 좌표를 타일맵 격자 좌표(Vector3Int)로 변환
        Vector3Int playerCell = fogTilemap.layoutGrid.WorldToCell(playerWorldPos);

        // 지정한 반지름(viewRadius)만큼 루프를 돌며 주변 타일을 지웁니다.
        for (int x = -viewRadius; x <= viewRadius; x++)
        {
            for (int y = -viewRadius; y <= viewRadius; y++)
            {
                // 원형태로 밝히고 싶다면 맨해튼 거리나 유클리드 거리 체크를 추가할 수 있습니다.
                // 여기서는 직관적인 사각형 범위(로그라이크 스타일)로 지웁니다.
                Vector3Int targetCell = new Vector3Int(playerCell.x + x, playerCell.y + y, playerCell.z);

                // 해당 칸의 검은색 타일을 지워버림 (아래 배경 타일이 보이게 됨)
                fogTilemap.SetTile(targetCell, null);
            }
        }
    }

    /// <summary>
    /// 지정된 사각형 영역(방) 전체의 안개를 한 번에 지웁니다.
    /// </summary>
    public void RevealRoom(Vector2 minWorldPos, Vector2 maxWorldPos)
    {
        if (fogTilemap == null) return;

        // 월드 좌표의 최소/최대 영역을 타일맵의 격자 좌표(Vector3Int)로 변환합니다.
        Vector3Int minCell = fogTilemap.layoutGrid.WorldToCell(minWorldPos);
        Vector3Int maxCell = fogTilemap.layoutGrid.WorldToCell(maxWorldPos);

        // 방의 시작점(좌측 하단)부터 끝점(우측 상단)까지 루프를 돌며 안개를 지웁니다.
        for (int x = minCell.x; x <= maxCell.x; x++)
        {
            for (int y = minCell.y; y <= maxCell.y; y++)
            {
                Vector3Int targetCell = new Vector3Int(x, y, minCell.z);
                fogTilemap.SetTile(targetCell, null);
            }
        }
    }
}
