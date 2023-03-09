using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 아이템 데이터 중 포션에 대한 데이터, 스크립터블 오브젝트 상속받음
/// </summary>
[CreateAssetMenu(fileName = "New Weapon", menuName = "Scriptable Object_Item Data/ItemData_Weapon", order = 3)]
public class ItemData_Weapon : ItemData, IEquipable
{
    public float attackDamage;
    GameObject makedItem = null;

    /// <summary>
    /// 장착하는 코드(장착하면 prefab을 플레이어의 장비가 장착되는 위치에 생성시키고, 플레이어 능력치에 장비 능력치만큼의 데이터를 추가 시킴)
    /// </summary>
    /// <param name="player"></param>
    public void Equip(Player player)
    {
        
        makedItem = Instantiate(itemPrefab, player.transform.position, player.transform.rotation);
        player.AttackDamage += attackDamage;
    }

    /// <summary>
    /// 해제하는 함수(장착하면 prefab을 플레이어의 장비가 삭제, 플레이어 능력치에 장비 능력치만큼의 데이터를 감소 시킴)
    /// </summary>
    /// <param name="player"></param>
    public void UnEquip(Player player)
    {
        Destroy(makedItem);
        player.AttackDamage -= attackDamage;
    }
}
