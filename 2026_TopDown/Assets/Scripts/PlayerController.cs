using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
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

        // 시작할 때 현재 위치를 목표 위치로 잡아둡니다.
        // 정수 단위(타일 칸)에 딱 맞추기 위해 Round(반올림)를 해줍니다.
        targetPosition = new Vector3(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y), transform.position.z);
        transform.position = targetPosition;


        rb = GetComponent<Rigidbody2D>();   
        sr = GetComponent<SpriteRenderer>();

        currentSprites = spriteDown;
        sr.sprite = currentSprites[0];

        moveSpeed = GameDataManager.Instance.GetPlayerMoveSpeed();
        playerHP = GameDataManager.Instance.GetPlayerHP();
        playerAttack = GameDataManager.Instance.GetPlayerAttack();
    }

    void Start()
    {
        if(GameDataManager.Instance.isTutorialFinished == 0)
        {
            // 튜토리얼 안 했을 경우 튜토리얼 오픈
            Debug.Log("튜토리얼 오픈!");
            GameDataManager.Instance.isTutorialFinished = 1;
        }
        else
        {
            //튜토리얼 했을 경우 아무것도 안 함
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
        // 1. 캐릭터를 목표 타일 위치로 부드럽게 슬라이드 이동시킵니다. (눈의 즐거움을 위해)
        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            // 애니메이션 재생
            timer += Time.deltaTime;
            if (timer >= frameTime)
            {
                timer = 0f;
                frameIndex = (frameIndex + 1) % currentSprites.Length;
                sr.sprite = currentSprites[frameIndex];
            }

            // 목표 타일에 완전히 도착했다면?
            if (Vector3.Distance(transform.position, targetPosition) < 0.001f)
            {
                transform.position = targetPosition; // 위치 강제 고정
                isMoving = false;
                frameIndex = 0;
                sr.sprite = currentSprites[0]; // 정지 모션

                // ★ 매우 중요: 내가 한 칸 이동을 마쳤으므로, GameManager에게 턴을 넘깁니다!
                if (GameManager.Instance != null)
                {
                    //GameManager.Instance.EndPlayerTurn();
                }
            }
        }
    }

    // New Input System에서 방향키를 누르면 '최초 1번' 실행되는 함수
    public void OnMove(InputValue value)
    {
        // 에디터 버그 방지 안전장치
        if (sr == null || this == null) return;

        // ★ 현재 플레이어의 턴이 아니거나, 이미 움직이는 중이라면 입력을 무시합니다.
        //if (GameManager.Instance != null && GameManager.Instance.currentState != GameState.PlayerTurn) return;
        if (isMoving) return;

        Vector2 input = value.Get<Vector2>();

        // 대각선 이동을 막고 상하좌우 중 가장 강한 입력 하나만 선택합니다.
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

            // [벽 충돌 검사 영역] 
            // 나중에 여기에 "이동할 칸에 벽 타일이 있는지 확인하는 코드"를 넣을 예정입니다.
            // 지금은 일단 무조건 이동하게 처리합니다.

            // 현재 위치에서 정확히 딱 1칸(direction) 뒤의 위치를 목표로 설정합니다.
            targetPosition = transform.position + direction;
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