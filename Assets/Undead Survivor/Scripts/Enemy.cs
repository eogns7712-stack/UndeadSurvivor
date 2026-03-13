using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed;
    public float healthPoint;   // 현재 체력 변수 추가
    public float maxhealthPoint;    // 최대 체력 변수 추가
    public RuntimeAnimatorController[] animCon; // Animator Controller를 여러개 사용하기 위함, 적 종류마다 다른 애니메이션을 적용하기 위함
    public Rigidbody2D target;  // 속도, 목표, 생존여부를 위한 변수
    
    bool isLive;
    
    Rigidbody2D rigid;
    Collider2D coll;    // Collider2D 변수 생성, Capsule Collider 2D이지만 Collider 2D로 통일 가능
    SpriteRenderer spriter;
    Animator anim;
    WaitForFixedUpdate wait;    // 다음 FixedUpdate가 될 때 까지 기다리는 변수 선언

    void Awake()
    {   // 변수 초기화
        rigid = GetComponent<Rigidbody2D>();    
        coll = GetComponent<Collider2D>();
        spriter = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        wait = new WaitForFixedUpdate();
    }

    void FixedUpdate()
    {
        if (!GameManager.instance.isLive)
            return;

        if (!isLive || anim.GetCurrentAnimatorStateInfo(0).IsName("Hit"))    // GetCurrentAnimatorStateInfo : 현재 재생되는 애니메이션 상태 정보를 가져오는 함수, IsName() : 해당 상태의 이름이 지정된 것과 같은지 확인하는 함수
                // Enemy가 살아있지 않다면 or 현재 재생중인 애니메이션 상태의 이름이 'Hit'라면
            return;
        
        Vector2 dirVec = target.position - rigid.position;   // 위치차이 = 타겟(플레이어)의 위치 - 나(Enemy)의 위치
        Vector2 nextVec = dirVec.normalized * speed * Time.fixedDeltaTime; // 방향 = 위치차이 정규화 normalized
        rigid.MovePosition(rigid.position + nextVec);   // 현재 위치에서 플레이어의 키입력 값을 더한 이동
        rigid.velocity = Vector2.zero;    //물리 속도가 이동에 영향을 주지않도록 물리속도 제거
    }
    void LateUpdate()
    {
        if (!GameManager.instance.isLive)
            return;
            
        if (!isLive)    // isLive의 반환값이 flase면
            return;

        spriter.flipX = target.position.x < rigid.position.x;    // 목표의 X축값과 자신의 X축 값을 비교해서 작으면 true가 되도록 설정
    }

    void OnEnable() //Unity에서 제공하는, 스크립트가 활성화 될 때 호출되는 이벤트함수
    {
        target = GameManager.instance.player.GetComponent<Rigidbody2D>();   
        // Enemy가 생성될 때 target 초기화, player의 Rigidbody2D를 따라가도록
        isLive = true; // 여러 로직을 결정하는 isLive 변수를 false로 변경
        coll.enabled = true;   // 컴포넌트 비활성화는 enabled = false
        rigid.simulated = true;    // Rigidbody의 물리적 비활성화는 rigid.simulated = false
        spriter.sortingOrder = 2;   // Dead상태 Enemy의 Order in Layer 1로 변경
        anim.SetBool("Dead",false);  // Animator의 트리거가 bool로 되어있기 때문에 SetBool을 통해 Dead 상태로 변환  // enemy가 생성될 때 isLive 활성화
        healthPoint = maxhealthPoint;   // 현재체력을 최대체력값으로 변경
    }

    public void Init(SpawnData data)  // 매개변수로 소환데이터 하나 지정
    {   // 적 종류 (spriteType)에 따라 애니메이션 컨트롤러를 바꾸는 코드
        anim.runtimeAnimatorController = animCon[data.spriteType];    // 매개변수의 속성을 본스터 속성 변경에 활용
        speed = data.speed;
        maxhealthPoint = data.healthPoint;
        healthPoint = data.healthPoint;
    }

    void OnTriggerEnter2D(Collider2D collision) // Unity에서 제공하는 이벤트함수, Trigger Collider에 다른 Collider가 들어왔을 때 자동실행
    {
        if (!collision.CompareTag("Bullet") || !isLive) // OnTriggerEnter2D 매개변수의 태그를 조건으로 활용
            // 사망로직이 연달아 실행되는 것을 방지하기 위한 조건 추가(영상09 27:58)
            return; // 충돌한 오브젝트의 태그가 'Bullet'이 아니면 즉시 리턴
        
        healthPoint -= collision.GetComponent<Bullet>().damage; // 충돌한 오브젝트의 태그가 'Bullet'이면 healthPoint에 Bullet의 damage만큼 감소
        StartCoroutine(knockBack());    // 코루틴을 시작하는 함수는 StartCoroutine으로 호출

        if (healthPoint > 0)    //남은 체력을 조건으로 피격과 사망으로 로직 나누기
        {   // Hit Action 출력(애니메이션 + 넉백)
            anim.SetTrigger("Hit");
        }
        else
        {
            isLive = false; // 여러 로직을 결정하는 isLive 변수를 false로 변경
            coll.enabled = false;   // 컴포넌트 비활성화는 enabled = false
            rigid.simulated = false;    // Rigidbody의 물리적 비활성화는 rigid.simulated = false
            spriter.sortingOrder = 1;   // Dead상태 Enemy의 Order in Layer 1로 변경
            anim.SetBool("Dead",true);  // Animator의 트리거가 bool로 되어있기 때문에 SetBool을 통해 Dead 상태로 변환
            GameManager.instance.kill++;
            GameManager.instance.GetExp();  // 몬스터 사망 시 킬수 증가와 경험치 증가함수 호출
        }
    }

    // 코루틴(Coroutine) : 생명주기와 비동기처럼 실행되는 함수
    IEnumerator knockBack() // IEnumerator : 코루틴만의 반환형 인터페이스, I가 붙으면 인터페이스라 부름
    {
        yield return wait;    // yield : 코루틴의 반환 키워드
        Vector3 playerPos = GameManager.instance.player.transform.position; // playerPos 변수에 GameManager에 있는 Player의 위치 저장
        Vector3 dirVec = transform.position - playerPos;    // 플레이어 기준의 반대방향 : Enemy의 현재위치 - 플레이어의 위치
        rigid.AddForce(dirVec.normalized * 3, ForceMode2D.Impulse);  // AddForce 함수로 dirVec 방향으로 힘 가하기, 순간적인 힘이므로 ForceMode2D.Impulse 추가
    }

    void Dead()
    {
        gameObject.SetActive(false);
    }
}
