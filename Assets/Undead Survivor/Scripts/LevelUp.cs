using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelUp : MonoBehaviour
{
    RectTransform rect; // UI인 LevelUp 창을 관리하기위한 RectTranform 변수 생성
    Item[] items;   // 아이템의 배열 변수 선언

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        items = GetComponentsInChildren<Item>(true);    // Item의 하위오브젝트의 컴포넌트 가져오기, 비활성화 된 오브젝트도 있기때문에 인자값 true
    }

    public void Show()  // 창을 보이고 숨기는 함수 작성
    {
        Next(); // 창을 보이게 할 때 Next함수 호출
        rect.localScale = Vector3.one;  // (1,1,1)
        GameManager.instance.Stop();    // UI가 출력될 때 게임을 정지하는 함수 호출
    }

    public void Hide()
    {
        rect.localScale = Vector3.zero;
        GameManager.instance.Resume();  // UI가 사라질 때 게임을 재생하는 함수 호출
    }

    public void Select(int index)   // 버튼을 대신 눌러주는 함수 작성
    {
        items[index].OnClick();
    }

    void Next() // 레벨업 선택창에서 아이템 3개를 랜덤으로 보여주는 함수
    {
        // 1. 모든 아이템 비활성화
        foreach (Item item in items)
        {
            item.gameObject.SetActive(false);
        }
        // 2. 그 중에서 랜덤 3개 아이템 활성화
        int[] ran = new int[3];    // 랜덤으로 활성화 할 아이템의 인덱스 3개를 담을 배열 생성
        while (true)
        {
            ran[0] = Random.Range(0, items.Length);   // 3개 데이터 모두 Random.Range 함수로 임의의 수 생성
            ran[1] = Random.Range(0, items.Length);
            ran[2] = Random.Range(0, items.Length);
            
            if (ran[0] != ran[1] && ran[1] != ran[2] && ran[0] != ran[2])   // 서로 비교해 모두 같지 않으면 반복문을 빠져나가도록 작성 (중복 아이템 등장 방지)
                break;
        }

        for (int index = 0; index < ran.Length; index++)
        {
            Item ranItem = items[ran[index]];

        // 3. 만렙 아이템의 경우는 소비아이템으로 대체
            if (ranItem.level == ranItem.data.damages.Length)
            {
                items[4].gameObject.SetActive(true);
                //items[Random.Range(4, 7)].gameObject.SetActive(true); // 만약 소비아이템이 여러개라면 Random.Range를 통해 구현
            }
            else
            {
                ranItem.gameObject.SetActive(true);
            }
        }
    }
}
