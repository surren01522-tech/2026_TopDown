
using UnityEngine;

public enum GameState { PlayerTurn, EnemyTurn }

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    
    public GameState currentState;                // 현재 게임이 누구의 차례인지 저장하는 변수입니다.

    void Awake()
    {
       
        Instance = this;                          // 전 세계에서 이 게임매니저는 '나 하나뿐'이라고 등록하는 과정입니다.
    }

    void Start()
    {
        
        currentState = GameState.PlayerTurn;     // 게임이 시작되면 가장 먼저 플레이어에게 턴(공격/이동 기회)을 줍니다.
    }

    void Update()
    {
        
        if (GameManager.Instance.currentState != GameState.PlayerTurn)       // 지금이 플레이어 턴이 아니면, 키보드를 눌러도 아무 반응이 없도록 막아버립니다!
        {
            return;
        }

        
        if (Input.GetKeyDown(KeyCode.W))               // 아래에 플레이어 이동 키보드 입력 코드 작성...
        {
            // 위로 이동 후
            GameManager.Instance.EndPlayerTurn();      // 내 턴 끝났다! 라고 외치기
        }
    }


    public void EndPlayerTurn()                  //플레이어가 이동이나 공격을 마치면 '직접' 호출하게 될 함수입니다.
    {
        
        currentState = GameState.EnemyTurn;      // 플레이어의 행동이 끝났으니, 게임 상태를 몬스터 차례(EnemyTurn)로 바꿉니다.

       
        MoveEnemies();                           // 몬스터들을 움직이러 갑니다! (아래 함수 실행)
    }

    void MoveEnemies()
    {
        // (여기에 실제로 몬스터가 플레이어를 향해 한 칸 움직이는 코드가 들어갑니다.)
        // 예: 플레이어가 있는 방향을 찾아서 그쪽으로 1칸 이동하기

        // 모든 몬스터가 이동을 마쳤다면? 
        // 다시 게임 상태를 플레이어 차례로 돌려놓아서 플레이어가 움직일 수 있게 합니다.
        currentState = GameState.PlayerTurn;
    }
}
