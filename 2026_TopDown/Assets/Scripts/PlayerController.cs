using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Spawn Settings")]
    public Vector3 spawnPosition = Vector3.zero; // 인스펙터에서 입력할 시작 좌표 (Z축은 보통 0)

    public float moveSpeed = 5f; // 한 칸 이동할 때의 속도 (수치가 높을수록 휙 움직입니다)

    // 스프라이트 애니메이션 배열들 (기존 설정 유지)
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

        // [수정] Mathf.Round를 제거하여 인스펙터에 입력한 소수점 좌표(9.5, -16.5)가 그대로 유지되도록 합니다.
        targetPosition = new Vector3(spawnPosition.x, spawnPosition.y, spawnPosition.z);
        transform.position = targetPosition;

        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        // [수정] 데이터 매니저 조회를 Awake에서 Start로 옮겼습니다.
        if (GameDataManager.Instance != null)
        {
            moveSpeed = GameDataManager.Instance.GetPlayerMoveSpeed();
            playerHP = GameDataManager.Instance.GetPlayerHP();
            playerAttack = GameDataManager.Instance.GetPlayerAttack();
        }
        else
        {
            Debug.LogError("하이어라키 창에 GameDataManager 오브젝트가 있는지 확인해주세요!");
        }

        // 기존 튜토리얼 코드
        if (GameDataManager.Instance != null && GameDataManager.Instance.isTutorialFinished == 0)
        {
            // 튜토리얼 관련 처리...
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            GameManager.Instance.GameOver();
        }
    }

    private void Update()
    {
        if (isMoving)
        {
            // 움직이는 순간의 애니메이션 처리만 남겨둡니다.
            timer += Time.deltaTime;
            if (timer >= frameTime)
            {
                timer = 0f;
                frameIndex = (frameIndex + 1) % currentSprites.Length;
                sr.sprite = currentSprites[frameIndex];
            }

            // 끊기는 이동 특성상 1프레임만에 목표에 도달하므로 바로 정지 모션으로 바꿉니다.
            isMoving = false;
            frameIndex = 0;
            sr.sprite = currentSprites[0];

            // GameManager에게 턴을 넘깁니다. (턴제 게임일 경우 주석 해제)
            if (GameManager.Instance != null)
            {
                // GameManager.Instance.EndPlayerTurn();
            }
        }
    }

    // New Input System에서 방향키를 누르면 '최초 1번' 실행되는 함수
    public void OnMove(InputValue value)
    {
        if (sr == null || this == null) return;
        if (isMoving) return;

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

            // [핵심 변경 사항] 부드럽게 이동하지 않고, 목표 위치로 즉시 좌표를 꽂아버립니다.
            targetPosition = transform.position + direction;
            transform.position = targetPosition;

            // 이동 상태를 켜서 Update에서 애니메이션 및 턴 처리가 1번 실행되도록 합니다.
            isMoving = true;
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

    public int playerHP = 0;
    public int playerAttack = 0;


}