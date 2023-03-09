using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 몬스터가 공격시 실제 데미지를 계산하는 클래스, 실제 무기는 존재하지 않으나 플레이어와 대칭성을 위한 클래스 이름
/// </summary>
public class MonsterWeapon : MonoBehaviour, IBattle
{
    Monster monster;

    float attackDamage;
    float defence;
    public float AttackDamage { get; set; }
    public float Defence { get; set; }
    private void Awake()
    {
        monster = GameObject.FindGameObjectWithTag("Monster").GetComponent<Monster>();
    }

    private void Start()
    {
        AttackDamage = monster.AttackDamage;
        Defence = monster.Defence;
    }


    public void Attack(IHealth target)
    {
        target.HP -= (AttackDamage - target.Defence);
    }

    private void OnTriggerEnter(Collider other)
    {
        // 무기에있는 컬라이더의 트리거
        if (other.CompareTag("Player"))
        {
            Player player;
            player = other.GetComponent<Player>();


            Attack(player);
            player.SetHP();

        }
    }

}
