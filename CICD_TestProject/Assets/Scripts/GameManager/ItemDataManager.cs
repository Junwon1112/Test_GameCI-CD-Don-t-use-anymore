using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 아이템과 관련된 데이터를 관리하는 클래스
/// </summary>
public class ItemDataManager : MonoBehaviour
{
    public ItemData[] itemDatas;

    /// <summary>
    /// 인덱서, 프로퍼티를 배열처럼 사용, 프로퍼티 이름을 this로 해서 클래스이름으로 프로퍼티를 
    /// 호출 후, 배열을 써서 해당 배열과 같은 인덱스를 쓰는 프로퍼티 값을 리턴해줌 
    /// </summary>
    /// <param name="i"></param>
    /// <returns></returns>
    public ItemData this[int i] 
    {
        get
        {
            return itemDatas[i];
        }
    }
    //배열처럼 쓰는 프로퍼티

    public ItemData this[ItemIDCode ID ]  //인덱서
    {
        get
        {
            return itemDatas[(int)ID];
        }
    }
    //배열처럼 쓰는 프로퍼티

    
}
