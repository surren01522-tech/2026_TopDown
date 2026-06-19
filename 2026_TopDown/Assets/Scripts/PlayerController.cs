using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Spawn Settings")]
    public Vector3 spawnPosition = Vector3.zero; // 인스펙터에서 입력할 시작 좌표 (Z축은 보통 0)
    public float moveSpeed = 5f; // 한 칸 이동할 때의 속도

    [Header("Player Stats")]
    public int playerHP = 100;
    public int playerAttack = 10;

    [Header("Tilemap Reference")]
    public UnityEngine.Tilemaps.Tilemap wallTilemap; // 인스펙터에서 벽 타일맵을 지정하세요 (없으면 자동 수색)

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

        // 소수점 좌표(9.5, -16.5)가 그대로 유지되도록 합니다.
        targetPosition = new Vector3(spawnPosition.x, spawnPosition.y, spawnPosition.z);
        transform.position = targetPosition;

        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        // 1. [안전 장치] 데이터 매니저 조회 및 예외 처리 (에러 원천 차단)
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

        // 2. [자동 연동] 인스펙터에 벽 타일맵이 비어있다면 씬에서 자동으로 찾아 매핑합니다.
        if (wallTilemap == null)
        {
            // 하이어라키 창에 있는 'WallTilemap' 이름을 가진 오브젝트를 찾습니다.
            GameObject wallObj = GameObject.Find("WallTilemap");
            if (wallObj != null)
            {
                wallTilemap = wallObj.GetComponent<UnityEngine.Tilemaps.Tilemap>();
            }
        }

        // 튜토리얼 코드 예외 처리
        if (GameDataManager.Instance != null && GameDataManager.Instance.isTutorialFinished == 0)
        {
            // 튜토리얼 관련 처리...
        }

        // 0.05초 뒤에 현재 위치를 기준으로 맵을 강제 리프레시
        Invoke(nameof(InitialReveal), 0.05f);
    }

    private void InitialReveal()
    {
        FieldOfView fov = FindFirstObjectByType<FieldOfView>();
        if (fov != null)
        {
            fov.RevealMap(transform.position);
        }
        else
        {
            Debug.LogWarning("씬 안에서 FieldOfView 오브젝트를 찾을 수 없습니다.");
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

        // [중요] 벽 타일맵이 없으면 충돌 연산 자체를 건너뛰어 터지는 것을 방지합니다.
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

            // 이동 후 FOV 실시간 연동
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