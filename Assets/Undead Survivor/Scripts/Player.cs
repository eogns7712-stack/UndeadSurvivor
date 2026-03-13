using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public Vector2 inputVec; // Vector2 = x,y 2개의 축만 존재하는 2차원 벡터
    public Vector2 moveDir; // 플레이어의 실제 이동방향을 저장하는 변수 선언
    public float speed;
    public Scanner scanner;   // Scanner타입 변수 선언
    public Hand[] hands;    // Player의 손 스크립트를 담을 배열변수 선언 및 초기화
    public RuntimeAnimatorController[] animCon;
    Rigidbody2D rigid;
    SpriteRenderer spriter;
    Animator anim;
    void Awake() // Start보다 우선 시작되는 함수 
    {   //변수 초기화
        rigid = GetComponent<Rigidbody2D>(); 
        spriter = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        scanner = GetComponent<Scanner>();  // 직접 만든 스크립트 또한 컴포넌트로 동일하게 취급
        hands = GetComponentsInChildren<Hand>(true);  // 배열이기 때문에 s, Player오브젝트의 자식이기 때문에 InChildren
        // 인자값 true를 넣으면 비활성화된 오브젝트의 컴포넌트도 가져올 수 있음
    }
    void Start()
    {
        
    }

    void OnEnable()
    {
        speed *= Character.Speed;
        anim.runtimeAnimatorController = animCon[GameManager.instance.playerId];    // OnEnable 함수에 애니메이터 변경로직 추가
    }
    void Update()
    {
        if (!GameManager.instance.isLive)
            return;
    //     inputVec.x = Input.GetAxisRaw("Horizontal"); // Input : Unity에서 받는 모든 입력을 관리하는 클래스, 좌우 방향키 입력감지 
    //     inputVec.y = Input.GetAxisRaw("Vertical"); // 상하 방향키 입력감지
    //     // GetAxis => 관성있음, GetAxisRaw => 관성없음

    }
    void FixedUpdate()  //물리 연산 프레임마다 호출되는 생명주기 함수
    { 
    // // 1. 힘을준다
    // rigid.AddForce(inputVec); 
    // // 2. 속도제어 // rigid.velocity = inputVec; 
    //3. 위치이동 
    if (!GameManager.instance.isLive)
            return;

    Vector2 nextVec = inputVec.normalized * speed * Time.fixedDeltaTime;  // normalized가 없으면 대각선 이동시 빨리이동함
    // fixedDeltaTime = 물리 프레임 하나가 소비한 시간 , DeltaTime = void Update()에서 사용
    rigid.MovePosition(rigid.position + nextVec); // 현재 위치에서 입력받은 값으로 이동 
    }

    void OnMove(InputValue value)   // Player의 이동을 구현하는 함수, normalized를 기본으로 사용하고있음
    {
        inputVec = value.Get<Vector2>();    // Get<T> : 프로필에서 설정한 컨트롤타입T 값을 가져오는 함수
    }

    void LateUpdate()   // LateUpdate : 프레임이 종료되기 전 실행되는 생명주기 함수
    {
        if (!GameManager.instance.isLive)
            return;

        anim.SetFloat("Speed",inputVec.magnitude);  // inputVec.magnitude : 입력받은 벡터의 순수한 값
        if (inputVec.x != 0)    // x 좌표가 움직일 때
        {
            spriter.flipX = inputVec.x < 0; // 움직이는 x좌표가 0보다 작으면 SpriteRenderer의 flip값 true
        }
    }

    void OnCollisionStay2D(Collision2D collision)   // OnCollisionStay2D 이벤트 함수 작성
    {
        if (!GameManager.instance.isLive)
            return;
        
        GameManager.instance.health -= Time.deltaTime * 10; // Time.deltaTime을 이용해 적절한 피격시간 계산

        if (GameManager.instance.health < 0)    // Player 사망로직
        {
            for (int index = 2; index < transform.childCount; index++)  // childCount : 자식 오브젝트의 개수
            {
                transform.GetChild(index).gameObject.SetActive(false);   // GetChild : 주어진 인덱스의 자식 오브젝트를 반환하는 함수
            }   

            anim.SetTrigger("Dead");    // 애니메이터 SetTrigger 함수로 Dead 애니메이션 실행
            GameManager.instance.GameOver();    // Player의 사망부분에서 게임오버 함수 호출
        }
    }
}