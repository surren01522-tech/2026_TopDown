using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : MonoBehaviour
{
    public Grid grid;
    public Tilemap tilemap;

    void Update()
    {
        // 마우스 클릭 위치를 월드 좌표로 변환
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // 월드 좌표를 타일 좌표(Vector3Int)로 변환
        Vector3Int cellPos = grid.WorldToCell(worldPos);

        // 타일 좌표를 타일의 월드 중앙 위치로 변환
        Vector3 centerWorldPos = grid.CellToWorld(cellPos);
    }

    
}
