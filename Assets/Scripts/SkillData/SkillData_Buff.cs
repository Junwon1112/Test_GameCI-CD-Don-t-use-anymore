using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 버프타입의 스킬데이터를 가지는 스크립터블 오브젝트, SkillData를 상속받음, 버프스킬 현재 미구현
/// </summary>
[CreateAssetMenu(fileName = ("New Skill Data"), menuName = ("Scriptable Object_Skill Data/Skill Data_Buff"), order = 3)]
public class SkillData_Buff : SkillData //버프 스킬
{
    //<부모에 있는 변수들>

    //public string skillName;
    //public uint skillId;
    //public Sprite skillIcon;

    //public int requireLevel;
    //public float skillCooltime;
    //public float skillDamage;

    //public SkillTypeCode skillType;

    //public string skillStateName;
    //public string skillInformation;

    public float buffDuration;

    public override float SetSkillDamage(float attackDamage)    //버프라서 스킬데미지 따로 존재 X
    {
        return 0;
    }
}
