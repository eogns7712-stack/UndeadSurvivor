using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    // 데미지와 관통변수 선언
    public float damage;
    public int per;

    Rigidbody2D rigid;  // Rigidbody2D 변수 생성 및 초기화

    void Awake()
    {   // 변수 초기화
        rigid = GetComponent<Rigidbody2D>();
    }

    public void Init(float damage, int per, Vector3 dir)  // 변수 초기화 함수 선언
    {
        this.damage = damage;   // this : 해당 클래스의 변수로 접근
        this.per = per;

        if (per >= 0)   // 관통(per)이 -1(무한)보다 큰 것에 대해서는 속도적용, 원거리무기의 per값이 0이거나 0보다 크면 속도적용하게 변경 (영상16 6:30)
        {
            rigid.velocity = dir * 15f;   // .velocity : 속도
        }
    }

    void OnTriggerEnter2D(Collider2D collision) // 총알 관통 로직
    {   //충돌한 오브젝트의 태그가 'Enemy'가 아니거나(!,or) 관통값(per)이 -1이라면
        if (!collision.CompareTag("Enemy") || per == -100) // || : or
            return;     // 즉시 반환

        //충돌한 오브젝트의 태그가 'Enemy'거나 관통값(per)이 -1이 아니라면
        per --; // 관통값(per) 감소
        
        if ( per < 0)     // 관통값(per)이 -1이라면 비활성화, 관통 이후 로직을 감싸는 if조건을 안전하게 변경(영상16 6:30)
        {
            rigid.velocity = Vector2.zero;  // 비활성화 이전에 물리속도 초기화, 재활용 하기 위함
            gameObject.SetActive(false);    // Desrtoy는 게임 최적화 문제로 사용x, 재활용 하기위해 비활성화 방식 선택
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Area") || per == -100)
            return;
        
        gameObject.SetActive(false);    // Player의 Area 밖으로 총알이 나가면 오브젝트 비활성화
    }
}
