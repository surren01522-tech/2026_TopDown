using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Spawn Settings")]
    public Vector3 spawnPosition = Vector3.zero; // 인스펙터에서 입력할 시작 좌표
    public float moveSpeed = 5f; // 한 칸 이동할 때의 속도

    [Header("Player Stats")]
    public int playerHP = 100;
    public int playerAttack = 10;

    [Header("Tilemap Reference")]
    public UnityEngine.Tilemaps.Tilemap wallTilemap; // 인스펙터 지정용 (없으면 자동 수색)

    [Header("Animations")]
    public Sprite[] spriteUp;
    public Sprite[] spriteDown;
    public Sprite[] spriteLeft;
    public Sprite[] spriteRight;
    public float frameTime = 0.15f;

    private SpriteRenderer sr;
    private Rigidbody2D rb;
    private Sprite[] currentSprites;
    private int frameIndex = 0;
    private float timer = 0f;

    private Vector3 targetPosition; // 이동할 목표 격자 위치
    private bool isMoving = false;  // 현재 한 칸 움직이는 중인가?

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        currentSprites = spriteDown;
        sr.sprite = currentSprites[0];

        targetPosition = new Vector3(spawnPosition.x, spawnPosition.y, spawnPosition.z);
        transform.position = targetPosition;

        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        // 1. 데이터 매니저 조회 및 능력치 세팅
        if (GameDataManager.Instance != null)
        {
            moveSpeed = GameDataManager.Instance.GetPlayerMoveSpeed();
            playerHP = GameDataManager.Instance.GetPlayerHP();
            playerAttack = GameDataManager.Instance.GetPlayerAttack();
        }
        else
        {
            Debug.LogWarning("GameDataManager.Instance를 찾을 수 없습니다. 기본값으로 진행합니다.");
        }

        // 2. 벽 타일맵 실시간 연동 및 초기 안개 밝히기 통합 처리
        RefreshSceneReferences();

        // 0.05초 뒤에 현재 위치를 기준으로 맵을 다시 한번 강제 리프레시 (안전장치)
        Invoke(nameof(InitialReveal), 0.05f);
    }

    // 💡 [추가] 씬 전환 등으로 인해 오브젝트가 활성화되거나 새 무대에 배치될 때 즉시 호출
    private void OnEnable()
    {
        RefreshSceneReferences();
    }

    /// <summary>
    /// 🚪 새 랜덤 맵으로 이동했을 때 씬 안에 물리 타일맵과 안개 시스템을 자동 재정렬합니다.
    /// </summary>
    private void RefreshSceneReferences()
    {
        // 새 씬의 벽 타일맵 탐색 및 자동 연동
        if (wallTilemap == null)
        {
            GameObject wallObj = GameObject.Find("WallTilemap");
            if (wallObj != null)
            {
                wallTilemap = wallObj.GetComponent<UnityEngine.Tilemaps.Tilemap>();
            }
        }

        // 발밑 안개 즉시 오픈
        InitialReveal();
    }

    private void InitialReveal()
    {
        FieldOfView fov = FindFirstObjectByType<FieldOfView>();
        if (fov != null)
        {
            Debug.Log("📌 새 씬의 FieldOfView를 탐색하여 시작 위치 안개를 밝힙니다!");
            fov.RevealMap(transform.position);
        }
        else
        {
            Debug.LogWarning("현재 씬 안에서 FieldOfView 오브젝트를 찾을 수 없습니다.");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            if (GameManager.Instance != null) GameManager.Instance.GameOver();
        }
    }

    private void Update()
    {
        if (isMoving)
        {
            timer += Time.deltaTime;
            if (timer >= frameTime)
            {
                timer = 0f;
                frameIndex = (frameIndex + 1) % currentSprites.Length;
                sr.sprite = currentSprites[frameIndex];
            }

            isMoving = false;
            frameIndex = 0;
            sr.sprite = currentSprites[0];
        }
    }

    public void OnMove(InputValue value)
    {
        if (sr == null || this == null) return;
        if (isMoving) return;

        // 벽 타일맵이 없다면 이동 직전 재탐색 시도
        if (wallTilemap == null)
        {
            GameObject wallObj = GameObject.Find("WallTilemap");
            if (wallObj != null) wallTilemap = wallObj.GetComponent<UnityEngine.Tilemaps.Tilemap>();
        }

        if (wallTilemap == null)
        {
            Debug.LogError("WallTilemap이 할당되지 않아 충돌 검사를 할 수 없습니다!");
            return;
        }

        Vector2 input = value.Get<Vector2>();

        if (input.sqrMagnitude > 0f)
        {
            Vector3 direction = Vector3.zero;

            if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
            {
                direction = input.x > 0 ? Vector3.right : Vector3.left;
                ChangeSprites(input.x > 0 ? spriteRight : spriteLeft);
            }
            else
            {
                direction = input.y > 0 ? Vector3.up : Vector3.down;
                ChangeSprites(input.y > 0 ? spriteUp : spriteDown);
            }

            Vector3 prospectiveTarget = transform.position + direction;

            // 벽 충돌 검사
            Vector3Int cellPos = wallTilemap.layoutGrid.WorldToCell(prospectiveTarget);

            if (wallTilemap.HasTile(cellPos))
            {
                isMoving = false;
                return;
            }

            // 이동 처리
            targetPosition = prospectiveTarget;
            transform.position = targetPosition;
            isMoving = true;

            // 이동 직후 실시간 안개 갱신
            FieldOfView fov = FindFirstObjectByType<FieldOfView>();
            if (fov != null)
            {
                fov.RevealMap(transform.position);
            }
        }
    }

    private void ChangeSprites(Sprite[] newSprites)
    {
        if (currentSprites == newSprites) return;
        currentSprites = newSprites;
        frameIndex = 0;
        timer = 0f;
        sr.sprite = currentSprites[frameIndex];
    }

    private void OnDisable()
    {
        var playerInput = GetComponent<PlayerInput>();
        if (playerInput != null) playerInput.enabled = false;
    }
}