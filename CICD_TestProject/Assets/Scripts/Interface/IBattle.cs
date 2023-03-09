using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 데미지를 계산하는 클래스에는 반드시 있어야 하는 인터페이스
/// </summary>
public interface IBattle
{
    public float AttackDamage { get; set; }
    //public float Defence { get; set; }
    

    public void Attack(IHealth target);


}
