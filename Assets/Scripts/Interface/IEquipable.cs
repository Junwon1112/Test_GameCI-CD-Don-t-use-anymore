using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 장비를 장착하는 클래스에 있어야하는 인터페이스
/// </summary>
public interface IEquipable
{
    public void Equip(Player player);
    public void UnEquip(Player player);
}
