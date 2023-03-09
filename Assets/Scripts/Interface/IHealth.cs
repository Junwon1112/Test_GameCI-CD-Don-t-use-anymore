using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 체력 및 방어력 관련 수치를 외부에서 적용할 때 필요한 인터페이스
/// </summary>
public interface IHealth
{
    public Transform CharacterTransform { get; }
    public float HP { get; set; }
    public float MaxHP { get; }

    public float Defence { get; set; }

}
