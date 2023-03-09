using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 지속 공격타입의 스킬데이터를 가지는 스크립터블 오브젝트, SkillData를 상속받음
/// </summary>
[CreateAssetMenu(fileName = ("New Skill Data"), menuName = ("Scriptable Object_Skill Data/Skill Data_Duration"), order = 2)]
public class SkillData_Duration : SkillData     //지속시간이 있는 공격 스킬
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
    public float skillDuration;

    public override float SetSkillDamage(float attackDamage)
    {
        float finalSkillDamage = 0;

        finalSkillDamage = this.skillDamage + (attackDamage * 0.2f);


        return finalSkillDamage;
    }
}
