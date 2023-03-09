using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 아이템 만드는 메소드의 집합(MakeItem)
/// </summary>
public class ItemFactory : MonoBehaviour
{
    /// <summary>
    /// 아이템을 하나 만들때 마다 늘릴 아이템 갯수
    /// </summary>
    static int itemCount = 0;

    /// <summary>
    /// 아이템을 동적으로 생성하는 메서드
    /// </summary>
    /// <param name="itemIDCode">어떤 아이템을 만들지 ItemIdCode로 받음, 다른 방식은 오버로딩을 통해 구현</param>
    /// <param name="position">생성할 위치</param>
    /// <param name="rotation">생성시 회전의 정도</param>
    /// <returns></returns>
    public static GameObject MakeItem(ItemIDCode itemIDCode, Vector3 position, Quaternion rotation)
    {
        GameObject obj = Instantiate(GameManager.Instance.ItemManager[itemIDCode].itemPrefab, position, rotation);
        Item item = obj.AddComponent<Item>();   //오브젝트에 아이템 컴포넌트 추가, 이를 통해 아이템이 됨
        if (itemIDCode == ItemIDCode.Basic_Weapon_1 || itemIDCode == ItemIDCode.Basic_Weapon_2)
        {
            obj.AddComponent<PlayerWeapon>();
            CapsuleCollider capsuleCollider = obj.AddComponent<CapsuleCollider>();
            capsuleCollider.radius = 0.1f;
            capsuleCollider.height = 1.4f;
            capsuleCollider.isTrigger = true;
        }
        else if(itemIDCode == ItemIDCode.HP_Potion)
        {
            SphereCollider sphereCollider = obj.AddComponent<SphereCollider>();
            sphereCollider.radius = 0.5f;
            sphereCollider.isTrigger = true;
        }
        //GameObject obj = new GameObject();      //새로운 오브젝트 만들고
        

        item.data = GameManager.Instance.ItemManager[itemIDCode];   //추가된 컴포넌트의 데이터는 ItemIdCode의 데이터에 따라간다.
        obj.name = $"{item.data.name}_{itemCount}";                 //이름 설정
        obj.layer = LayerMask.NameToLayer("Item");                  //레이어 설정
        
        itemCount++;    //현재 아이템 갯수 한개 추가

        //강사님은 여기에 미니맵표시 추가를 함, 나중에 미니맵 만들면 추가 요망

        return obj;
    }

    /// <summary>
    /// id로 받은 값을 내부적으로 enum타입으로 형변환해서 만들어주는 함수
    /// </summary>
    /// <param name="id"></param>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    /// <returns></returns>
    public static GameObject MakeItem(uint id, Vector3 position, Quaternion rotation)
    {
        GameObject obj = MakeItem((ItemIDCode)id, position, rotation);  
        return obj;
    }

    //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    /// <summary>
    /// 아이템 만들시 위치값을 미세하게 다르게 나오도록 만드는 메소드
    /// </summary>
    /// <param name="itemIDCode"></param>
    /// <param name="itemPosition"></param>
    /// <param name="rotation"></param>
    /// <param name="randomNoise">bool randomNoise = false 는 만약에 파라미터로 randomNoise만 따로 안적으면 false로 취급한다는 뜻이다</param>
    /// <returns></returns>

    public static GameObject MakeItem(ItemIDCode itemIDCode, Vector3 itemPosition, Quaternion rotation, bool randomNoise = false)   
    {
        GameObject obj = MakeItem(itemIDCode, itemPosition, rotation);

        if(randomNoise)
        {
            Vector2 noise = Random.insideUnitCircle * 0.5f;
            itemPosition.x += noise.x;
            itemPosition.y += noise.y;
        }

        obj.transform.position = itemPosition;

        return obj;
    }
    /// <summary>
    /// 살짝랜덤으로 위치를 만드는데 id로 받음
    /// </summary>
    /// <param name="id"></param>
    /// <param name="itemPosition"></param>
    /// <param name="rotation"></param>
    /// <param name="randomNoise"></param>
    /// <returns></returns>
    public static GameObject MakeItem(uint id, Vector3 itemPosition, Quaternion rotation, bool randomNoise = false)
    {
        return (MakeItem((ItemIDCode)id, itemPosition, rotation, randomNoise));
    }


    //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------


    /// <summary>
    /// 얘는 한번에 여러개의 아이템을 만드는 코드로 리턴값이 void이다
    /// </summary>
    /// <param name="itemIDCode"></param>
    /// <param name="itemPosition"></param>
    /// <param name="rotation"></param>
    /// <param name="count"></param>
    public static void MakeItem(ItemIDCode itemIDCode, Vector3 itemPosition, Quaternion rotation,uint count)
    {
        //GameObject obj = MakeItem(itemIDCode);

        for(int i=0; i < count; i++)
        {
            MakeItem(itemIDCode, itemPosition, rotation, true);
        } 
    }

    /// <summary>
    /// 얘는 한번에 여러개의 아이템을 만드는 코드로 id로 받음(오버로딩)
    /// </summary>
    /// <param name="id"></param>
    /// <param name="itemPosition"></param>
    /// <param name="rotation"></param>
    /// <param name="count"></param>
    public static void MakeItem(uint id, Vector3 itemPosition, Quaternion rotation, uint count)
    {
        MakeItem((ItemIDCode)id, itemPosition, rotation, count);
    }







}
