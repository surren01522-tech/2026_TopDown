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
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 플레이어의 현재 월드 위치를 기준으로 주변의 안개를 지웁니다.
    /// </summary>
    // 🚪 씬 전환 시 자동으로 타일맵을 새로 찾아 매핑하는 로직이 추가된 완성형 함수
    public void RevealMap(Vector3 playerWorldPos)
    {
        // 타일맵 체크 및 자동 탐색 (없으면 실행 차단으로 에러 방지)
        if (!CheckAndFindFogTilemap()) return;

        Vector3Int playerCell = fogTilemap.layoutGrid.WorldToCell(playerWorldPos);

        for (int x = -viewRadius; x <= viewRadius; x++)
        {
            for (int y = -viewRadius; y <= viewRadius; y++)
            {
                Vector3Int targetCell = new Vector3Int(playerCell.x + x, playerCell.y + y, playerCell.z);
                fogTilemap.SetTile(targetCell, null); // ◀ 46번 줄 에러 원천 차단!
            }
        }
    }

    /// <summary>
    /// 🚪 [수정 완료] 지정된 사각형 영역(방) 전체의 안개를 한 번에 지웁니다.
    /// </summary>
    public void RevealRoom(Vector2 minWorldPos, Vector2 maxWorldPos)
    {
        // 타일맵 체크 및 자동 탐색 (없으면 실행 차단으로 에러 방지)
        if (!CheckAndFindFogTilemap()) return;

        // 월드 좌표의 최소/최대 영역을 타일맵의 격자 좌표(Vector3Int)로 변환합니다.
        Vector3Int minCell = fogTilemap.layoutGrid.WorldToCell(minWorldPos);
        Vector3Int maxCell = fogTilemap.layoutGrid.WorldToCell(maxWorldPos);

        // 방의 시작점(좌측 하단)부터 끝점(우측 상단)까지 루프를 돌며 안개를 지웁니다.
        for (int x = minCell.x; x <= maxCell.x; x++)
        {
            for (int y = minCell.y; y <= maxCell.y; y++)
            {
                Vector3Int targetCell = new Vector3Int(x, y, minCell.z);
                fogTilemap.SetTile(targetCell, null); // ◀ 67번 줄 에러 원천 차단!
            }
        }
    }

    private bool CheckAndFindFogTilemap()
    {
        if (fogTilemap == null)
        {
            // 하이어라키 창에 있는 'FogTilemap' 이름의 오브젝트를 찾습니다.
            GameObject fogObj = GameObject.Find("FogTilemap");
            if (fogObj != null)
            {
                fogTilemap = fogObj.GetComponent<UnityEngine.Tilemaps.Tilemap>();
            }
        }
        // 최종적으로 있으면 true, 없으면 false 반환
        return fogTilemap != null;
    }
}
