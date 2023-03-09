using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 아이템을 사용하는 클래스에 있어햐 하는 인터페이스
/// </summary>
public interface IConsumable
{
    public void Use(Player player);
}
