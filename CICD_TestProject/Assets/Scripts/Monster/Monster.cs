using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

/// <summary>
/// 몬스터 행동과 관련된 메서드
/// </summary>
public class Monster : MonoBehaviour, IHealth
{
    //몬스터가 해야할 일들
    //1, 일정 구간 순찰 => 구현
    //2. 순찰 중 일정범위 내 플레이어 존재시 추적 => 구현
    //3. 너무 먼 거리를 오면 제자리로 돌아감 => 구현
    //4. 움직임, 공격, 맞음, 죽음 4가지 상태  => 죽음 제외 3가지 구현
    //5. 체력 필요 => 체력 체크 용 인터페이스 필요 => 구현 해야함

    NavMeshAgent agent;

    /// <summary>
    /// 순찰지점의 위치
    /// </summary>
    Transform[] patrolPoints;   

    public delegate void Action(NavMeshAgent agent);
    int destinationIndex = 0;


    float monsterSearchRadius = 5.0f;
    LayerMask playerLayer;
    int tempLayerMask;

    /// <summary>
    /// 찾았을 때 추적할 플레이어 트랜스폼
    /// </summary>
    Transform playerTransform = null;

    /// <summary>
    /// 플레이어 체력 등 가져오기 위한 플레이어 스크립트
    /// </summary>
    Player player;

    /// <summary>
    /// 몬스터 상태 체크용
    /// </summary>
    bool isMonsterChase = false;
    bool isPatrol = true;
    bool isCombat = false;
    bool isDie = false;

    float hp;
    float maxHP = 100;
    float ratio;

    Slider hpSlider;

    Animator anim;

    public float giveExp = 30.0f;

    float attackDamage = 10;
    float defence = 3;

    float attackDelay = 1.5f;
    float criticalRate = 15.0f; // 15퍼센트 확률로 치명타

    bool isAttackContinue = false;
    public bool playerTriggerOff = false;

    public Transform CharacterTransform
    {
        get { return this.transform; }
    }

    /// <summary>
    /// 공격력과 관련된 프로퍼티
    /// </summary>
    public float AttackDamage
    {
        get
        {
            return attackDamage;
        }
        set
        {
            attackDamage = value;
        }
    }
    /// <summary>
    /// 방어력과 관련된 프로퍼티
    /// </summary>
    public float Defence
    {
        get { return defence; }
        set { defence = value; }
    }

    /// <summary>
    /// 체력과 관련된 프로퍼티, 0이될 경우 아이템 드랍
    /// </summary>
    public float HP
    {
        get { return hp; }
        set 
        {
            hp = value;

            if (hp <= 0 && !isDie)
            {
                anim.SetBool("isDie", true);
                SetMonsterState(MonsterState.die);
                agent.enabled = false;
                DropItem();
                Destroy(transform.parent.gameObject, 3.0f);
            }
        }
    }
    /// <summary>
    /// 최대체력에 대한 프로퍼티
    /// </summary>
    public float MaxHP
    {
        get { return maxHP; }
    }
   



    /// <summary>
    /// 몬스터 상태 체크용 enum
    /// </summary>
    enum MonsterState
    {
        patrol = 0,
        chase,
        combat,
        die
    }

    /// <summary>
    /// enum 인스턴스만들고 기본값을 patrol로 설정
    /// </summary>
    MonsterState monsterState = MonsterState.patrol;


    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        playerLayer = LayerMask.NameToLayer("Player");
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform.GetComponent<Transform>();
        player = GameObject.FindGameObjectWithTag("Player").transform.GetComponent<Player>();
        hpSlider = GetComponentInChildren<Slider>();
        anim = GetComponent<Animator>();
    
    }

    private void Start()
    {
        Transform patrolPoint = transform.parent.GetChild(1);

        patrolPoints = new Transform[patrolPoint.childCount];

        for (int i = 0; i < patrolPoint.childCount; i++)
        {
            patrolPoints[i] = patrolPoint.transform.GetChild(i);
        }

        //비트플래그, 0000 0001 을 playerLayer(7번째 레이어) 만큼 옮겨라 => 0100 0000 이 됨, 플레이어 찾을 떄 플레이어 레이어용 변수로 활용하기 위해 만듬 
        tempLayerMask = (1 << playerLayer); 

        hp = maxHP;

        SetHP();

        anim.SetBool("isPatrol", true);

        agent.SetDestination(patrolPoints[0].transform.position);

        SetMonsterState(monsterState);

    }

    /// <summary>
    /// 매 프레임마다 주변을 체크하고 해당되는 상황의 메서드를 실행
    /// </summary>
    private void Update()
    {
        LookingCameraHPBar();

        if(isPatrol)
        {
            PatrolUpdate(); // 순찰을 하며 주변에 플레이어가 없는지 polling방식으로 계속체크
        }
        else if(isMonsterChase)
        {
            ChaseUpdate();  //플레이어에게 도착할 때까지 추적, 도중에 플레이어가 사라지면 patrol로 돌아가도록 만들어야 됨
        }
        else if(isCombat)
        {
            CombatUpdate(); 
        }
        //else if (isDie)
        //{
        //    DieUpdate();
        //}

    }

    /// <summary>
    /// 순찰시 실제 실행되는 메서드 
    /// </summary>
    private void SetPatrol()
    {
        //0번 경로가 설정되긴 하는데 그뒤 remainingDistance가 0으로 설정돼서 바로 1번 경로로 넘어가 버림,
        //계산 시간이 필요해 그런 듯, 앞으로는 update에서 하지말고 코루틴으로 시간 넉넉하게 돌리고 이번엔 그냥 남은거리 0이면 계산 안 하도록 진행  
        if (agent.remainingDistance <= agent.stoppingDistance && agent.remainingDistance != 0)  
        {
            
            destinationIndex++;
            destinationIndex %= patrolPoints.Length;
            agent.SetDestination(patrolPoints[destinationIndex].transform.position);
        }
        
    }

    /// <summary>
    /// 주변에 플레이어가 있는지 찾는 메서드
    /// </summary>
    private void FindPlayer()  //ontrigger쓰면 되는건데 연습해보고 싶어 사용함
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, monsterSearchRadius, tempLayerMask);
        //여기서 null값 뜨는 중, tempLayerMask 값은 128, player도 7번쨰 layer
        //player에 컬라이더가 없어 생기던 문제였음
        

        if (colliders.Length > 0)  //플레이어는 한명뿐이니 존재하기만 하면 찾은것으로 판단
        {
            monsterState = MonsterState.chase;
            SetMonsterState(monsterState);
            agent.SetDestination(playerTransform.position);

        }
    }

    /// <summary>
    /// 찾은 플레이어가 있을 때 추적하는 메서드
    /// </summary>
    private void ChasePlayer()  //FindPlayer가 찾은 플레이어 트랜스폼으로 추적하는 함수
    {

        if (agent.remainingDistance <= agent.stoppingDistance)  //플레이어에 도착하면 공격태세로 전환
        {
            monsterState = MonsterState.combat;
            SetMonsterState(monsterState);
        }
        else if(agent.remainingDistance > monsterSearchRadius)    //너무 멀어지면 다시 순찰
        {
            monsterState = MonsterState.patrol;
            SetMonsterState(monsterState);
            agent.SetDestination(patrolPoints[destinationIndex].transform.position);
            //상태바뀔때 목적지 재설정
        }
        else
        {
            agent.SetDestination(playerTransform.position);
            //이동하는 플레이어 위치 갱신을 위해 시작할 때 실행
        }

    }

    /// <summary>
    /// 전투 가능 거리에 플레이어가 있을 때 전투를 실행하는 메서드
    /// </summary>
    private void CombatPlayer()
    {
        //agent.remainingDistance썼다가 계속 체크하려면 setDestination을 계속 해야돼서 포기함
        if ((transform.position - playerTransform.position).sqrMagnitude < 2.5f * 2.5f) 
        {
            if (player.HP > 0)
            {
                //transform.rotation = Quaternion.Lerp(Quaternion.identity, Quaternion.LookRotation(playerTransform.position), 0.1f) * Quaternion.Euler(0,180, 0);
                transform.LookAt(Vector3.Lerp(transform.position, playerTransform.position, 0.1f));
                Debug.Log("전투중");
                MonsterAttack();
            }
            else
            {
                monsterState = MonsterState.patrol;
                SetMonsterState(monsterState);
                Debug.Log("몬스터 승리");
            }
        }
        else  //너무 멀어지면 다시 플레이어 추적
        {
            monsterState = MonsterState.chase;
            SetMonsterState(monsterState);
            agent.SetDestination(playerTransform.position);
            //상태바뀔때 목적지 재설정
        }
    }

    /// <summary>
    /// 조건을 만족 시 일정 주기로 공격을 시행하는 메서드
    /// </summary>
    private void MonsterAttack()
    {
        if(!isAttackContinue)   //업데이트에서 여러번 실행되지않도록
        {
            isAttackContinue = true;
            StartCoroutine(MonsterAttackCoroutine(attackDelay));
        }
    }

    /// <summary>
    /// 공격속도에 따라 공격주기가 바뀌고 공격 실행시 실행되는 것들에 대한 메서드
    /// </summary>
    /// <param name="attackSpeed"></param>
    /// <returns></returns>
    IEnumerator MonsterAttackCoroutine(float attackSpeed)
    {
        yield return new WaitForSeconds(attackSpeed);
        //monsterCollider.isTrigger = true; => 애니메이션으로 세팅
        anim.SetTrigger("OnAttack");
        isCriticalAttack(criticalRate);
        isAttackContinue = false;
    }

    /// <summary>
    /// 치명타 공격이 발생하는지 확인하는 메서드
    /// </summary>
    /// <param name="criticalPercent"></param>
    private void isCriticalAttack(float criticalPercent)
    {
        float criticalAttack;
        criticalAttack = Random.Range(0, 100.0f);
        if(criticalAttack < criticalPercent)
        {
            anim.SetTrigger("OnCritical");
        }
    }


    // OntriggerEnter를 무기 스크립트에서 실행하는것으로 변경, 몬스터가 공격했을때 플레이어의 트리거도 발동되어 몬스터 자신도 피해입는 문제를 해결하기 위해
    //private void OnTriggerEnter(Collider other) //공격할때 공격용 컬라이더가 활성되며 트리거를 파악, 플레이어가 들어오면 
    //{
    //    if(other.CompareTag("Player"))
    //    {
    //        playerTriggerOff = true;
    //        Attack(player); //Attack은 매개변수로 IBattle을 받는데 Player클래스는 IBattle을 상속받았으므로 사용할 수 있다.
    //        player.SetHP();
    //        Debug.Log($"{player.HP}");
    //    }
    //}

    /// <summary>
    /// 몬스터의 AI상태를 상황에 따라 바꾸는 함수
    /// </summary>
    /// <param name="mon"></param>
    private void SetMonsterState(MonsterState mon)  //플레이어 상태 세팅해주는 함수
    {

        switch (mon)
        {
            case MonsterState.patrol:
                isPatrol = true;
                isMonsterChase = false;
                isCombat = false;
                isDie = false;
                anim.SetBool("isPatrol", true);
                anim.SetBool("isChase", false);
                anim.SetBool("isCombat", false);
                break;
            case MonsterState.chase:
                isPatrol = false;
                isMonsterChase = true;
                isCombat = false;
                isDie = false;
                anim.SetBool("isPatrol", false);
                anim.SetBool("isChase", true);
                anim.SetBool("isCombat", false);
                break;
            case MonsterState.combat:
                isPatrol = false;
                isMonsterChase = false;
                isCombat = true;
                isDie = false;
                anim.SetBool("isPatrol", false);
                anim.SetBool("isChase", false);
                anim.SetBool("isCombat", true);
                break;
            case MonsterState.die:
                isPatrol = false;
                isMonsterChase = false;
                isCombat = false;
                isDie = true;
                anim.SetBool("isDie", true);

                break;

            default:
                break;
        }
    }

    /// <summary>
    /// 업데이트에서 순찰할 때 사용할 함수들
    /// </summary>
    private void PatrolUpdate()
    {
        SetPatrol();    // 순찰 시키기
        FindPlayer(); // 플레이어 찾기, 참고로 찾은 플레이어
    }

    /// <summary>
    /// 업데이트에서 추적할 때 사용할 함수들
    /// </summary>
    private void ChaseUpdate()
    {
        ChasePlayer();
    }

    /// <summary>
    /// 업데이트에서 전투할 때 사용할 함수들
    /// </summary>
    private void CombatUpdate()
    {
        CombatPlayer();   
    }
    //private void DieUpdate()
    //{
    //    Die();
    //}

    /// <summary>
    /// 체력 계산하는 메서드
    /// </summary>
    public void SetHP()
    {
        hpSlider.value = HP / MaxHP;
    }

    /// <summary>
    /// Hp바가 항상 플레이어를 보도록 하는 함수
    /// </summary>
    private void LookingCameraHPBar()
    {
        hpSlider.transform.LookAt(Camera.main.transform.position);
    }

    /// <summary>
    /// 아이템을 드롭할 때 사용되는 메서드
    /// </summary>
    private void DropItem()
    {
        ItemFactory.MakeItem(ItemIDCode.HP_Potion, transform.position, Quaternion.identity);
    }

}
