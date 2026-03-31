using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LevelUp : MonoBehaviour
{
    RectTransform rect; // UI인 LevelUp 창을 관리하기위한 RectTranform 변수 생성
    Item[] items;   // 아이템의 배열 변수 선언
    bool isSelecting;   // 레벨업 창이 현재 열려있는지 확인하는 변수 선언
    int[] currentChoices = new int[3];

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        items = GetComponentsInChildren<Item>(true);    // Item의 하위오브젝트의 컴포넌트 가져오기, 비활성화 된 오브젝트도 있기때문에 인자값 true
    }
    void Update()
    {
        if (!isSelecting)
            return;

        Keyboard keyboard = Keyboard.current;
        if (keyboard == null)
            return;

        if (keyboard.numpad1Key.wasPressedThisFrame || keyboard.digit1Key.wasPressedThisFrame)
        {
            Select(0);
        }
        else if (keyboard.numpad2Key.wasPressedThisFrame || keyboard.digit2Key.wasPressedThisFrame)
        {
            Select(1);
        }
        else if (keyboard.numpad3Key.wasPressedThisFrame || keyboard.digit3Key.wasPressedThisFrame)
        {
            Select(2);
        }
    }
    public void Show()  // 창을 보이고 숨기는 함수 작성
    {
        Next(); // 창을 보이게 할 때 Next함수 호출
        rect.localScale = Vector3.one;  // (1,1,1)
        GameManager.instance.Stop();    // UI가 출력될 때 게임을 정지하는 함수 호출
        AudioManager.instance.PlaySfx(AudioManager.Sfx.LevelUp); // 레벨업시 효과음 재생
        AudioManager.instance.EffectBgm(true);  // 레벨업UI가 나타날 때 필터를 켜고, 사라지면 끄도록 함수 호출
        isSelecting = true;
    }

    public void Hide()
    {
        rect.localScale = Vector3.zero;
        GameManager.instance.Resume();  // UI가 사라질 때 게임을 재생하는 함수 호출
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Select); // 레벨업 아이템 버튼 클릭시 효과음 재생
        AudioManager.instance.EffectBgm(false);  // 레벨업UI가 나타날 때 필터를 켜고, 사라지면 끄도록 함수 호출
        isSelecting = false;
    }

    public void Select(int slotIndex)   // 버튼을 대신 눌러주는 함수 작성
{
    if (slotIndex < 0 || slotIndex >= currentChoices.Length)
        return;

    int realIndex = currentChoices[slotIndex];

    if (realIndex < 0 || realIndex >= items.Length)
        return;

    if (!items[realIndex].gameObject.activeSelf && isSelecting)
        return;

    items[realIndex].OnClick();

    if (isSelecting)
        Hide();
}
    void Next() // 레벨업 선택창에서 아이템 3개를 랜덤으로 보여주는 함수
    {
        // 1. 모든 아이템 비활성화
        foreach (Item item in items)
        {
            item.gameObject.SetActive(false);
        }
        // 2. 그 중에서 랜덤 3개 아이템 활성화
        while (true)
        {
            currentChoices[0] = Random.Range(0, items.Length);   // 3개 데이터 모두 Random.Range 함수로 임의의 수 생성
            currentChoices[1] = Random.Range(0, items.Length);
            currentChoices[2] = Random.Range(0, items.Length);

            if (currentChoices[0] != currentChoices[1] &&
                currentChoices[1] != currentChoices[2] &&
                currentChoices[0] != currentChoices[2])
                break;
        }

        for (int index = 0; index < currentChoices.Length; index++)
        {
            Item ranItem = items[currentChoices[index]];

        // 3. 만렙 아이템의 경우는 소비아이템으로 대체
            if (ranItem.level == ranItem.data.damages.Length)
            {
                currentChoices[index] = 4;
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
