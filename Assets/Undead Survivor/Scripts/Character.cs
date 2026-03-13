using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public static float Speed   // 함수가 아닌 속성작성
    {
        get { return GameManager.instance.playerId == 0 ? 1.1f : 1f; }
    }
}
