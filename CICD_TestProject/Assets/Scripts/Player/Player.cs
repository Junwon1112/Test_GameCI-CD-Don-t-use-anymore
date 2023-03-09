using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Animations;
using UnityEngine.UI;

public class Player : MonoBehaviour, IHealth
{
    Player player;



    /// <summary>
    /// 움직임을 위한 인풋 시스템용
    /// </summary>
    public PlayerInput input;

    /// <summary>
    /// 이동 방향 받고 리턴용
    /// </summary>
    Vector3 dir = Vector3.zero;

    float walkSoundVolume = 1.0f;
    float AttackSoundVolume = 0.7f;

    /// <summary>
    /// 애니메이션 용 
    /// </summary>
    Animator anim;

    /// <summary>
    /// 다른 행동중 움직임을 제한하기 위해
    /// </summary>
    bool canMove = true;

    /// <summary>
    /// 체력 관련 변수들
    /// </summary>
    float hp;
    float maxHp = 100;
    Slider hpBar;

    /// <summary>
    /// 경험치 관련 변수들
    /// </summary>
    float exp = 0.0f;
    float maxExp = 100;
    Slider expBar;

    [SerializeField]
    int level = 1;

    /// <summary>
    /// 회전 관련 변수들
    /// </summary>
    float turnToX;
    float turnToY;
    float turnToZ;

    float turnSpeed = 30.0f;

    /// <summary>
    /// 아이템 관련 변수
    /// </summary>
    Item item;
    ItemFactory itemFactory;
    ItemIDCode itemID;
    ItemData_Potion potion;
    public ItemData_Weapon myWeapon;

    float findItemRange = 3.0f;
    Inventory playerInventory;
    InventoryUI playerInventoryUI;

    /// <summary>
    /// 무기 바꿀때 사용하기 위한 변수들
    /// </summary>
    GameObject weaponPrefab;
    CapsuleCollider weaponCollider;
    public Transform weaponHandTransform;
    public bool isFindWeapon = false;

    /// <summary>
    /// 스킬 사용 중인지 체크하기 위해 사용
    /// </summary>
    public bool isSkillUsing = false;

    SkillUse[] skillUses;

    public Transform CharacterTransform
    {
        get { return this.transform; }
    }

    public float HP
    {
        get { return hp; }
        set 
        { 
            hp = value; 

        }
    }

    public float MaxHP
    {
        get { return maxHp; }
    }

    public float Exp
    {
        get { return exp; }
        set
        {
            exp = value;

        }
    }

    public float MaxExp
    {
        get { return maxExp; }
        set
        {
            maxExp = value;
        }
    }

    /// <summary>
    /// private여도 유니티에서 수치바꿀수 있게 해주는 것
    /// </summary>
    [SerializeField]    
    float attackDamage = 10;

    [SerializeField]
    float defence = 5;

    public float AttackDamage
    {
        get { return attackDamage; }
        set { attackDamage = value; }
    }
    public float Defence
    {
        get { return defence; }
        set { defence = value; }
    }


    private void Awake()
    {
        input = new PlayerInput();
        anim = GetComponent<Animator>();
        hpBar = GameObject.Find("HpSlider").GetComponent<Slider>();
        player = GetComponent<Player>();
        playerInventory = GetComponentInChildren<Inventory>();
        playerInventoryUI = FindObjectOfType<InventoryUI>();
        weaponHandTransform = FindObjectOfType<FindWeaponHand>().transform;
        expBar = GameObject.Find("ExpSlider").GetComponent<Slider>();
        skillUses = FindObjectsOfType<SkillUse>();
    }

    /// <summary>
    /// InputSystem에 등록한 단축키들에 해당하는 함수 등록
    /// </summary>
    private void OnEnable()
    {
        input.Player.Enable();
        input.Player.Move.performed += OnMoveInput;
        input.Player.Attack.performed += OnAttackInput;
        input.Player.Look.performed += OnLookInput;
        input.Player.TempItemUse.performed += OnTempItemUse;
        input.Player.TakeItem.performed += OnTakeItem;
        input.Player.TestMakeItem.performed += OnTestMakeItem;
    }


    /// <summary>
    /// InputSystem에 등록한 단축키들에 해당하는 함수 해제
    /// </summary>
    private void OnDisable()
    {
        input.Player.TestMakeItem.performed -= OnTestMakeItem;
        input.Player.TempItemUse.performed -= OnTempItemUse;
        input.Player.Attack.performed -= OnAttackInput;
        input.Player.Move.performed -= OnMoveInput;
        input.Player.Look.performed -= OnLookInput;
        input.Player.Disable();
        input.Player.TakeItem.performed -= OnTakeItem;
    }

    

    private void Start()
    {
        hp = maxHp;
        SetHP();
        potion = new ItemData_Potion();
        SetExp();
        myWeapon = new ItemData_Weapon();
    }

    private void Update()
    {
        transform.Translate(dir * Time.deltaTime * 10, Space.Self);
        if (dir == Vector3.zero)
        {
            anim.SetBool("IsMove", false);
        }
    }

    private void OnMoveInput(InputAction.CallbackContext obj)
    {
        if(canMove)
        {
            //2개의 축만 필요해 2d vector로 만들면 readvalue값을 2d로 받아야만 한다.
            //이후 3d로 변환하는 과정을 거친다.
            Vector3 tempDir;
            tempDir = obj.ReadValue<Vector2>();
            dir.x = tempDir.x;
            dir.z = tempDir.y;

            anim.SetFloat("DirSignal_Front", dir.z);
            anim.SetFloat("DirSignal_Side", dir.x);
            anim.SetBool("IsMove", true);
        }
        
    }


    private void OnAttackInput(InputAction.CallbackContext obj)
    {
        anim.SetBool("IsMove", false);
        anim.SetTrigger("AttackOn");
    }

    //private void OnTriggrEnter(Collider other)
    //{
    //    //플레이어 칼에있는 컬라이더의 트리거
    //    if(other.CompareTag("Monster"))
    //    {
    //        Monster monster;
    //        monster = other.GetComponent<Monster>();
    //        if(monster.playerTriggerOff == false)
    //        {
    //            Attack(monster);
    //            monster.SetHP();
                
    //        }
    //        monster.playerTriggerOff = false;

    //    }
    //}

    private void OnLookInput(InputAction.CallbackContext obj)
    {
        float moveX = obj.ReadValue<Vector2>().x;
        float moveY = obj.ReadValue<Vector2>().y;

        //좌우 회전
        turnToY = turnToY + moveX * turnSpeed * Time.deltaTime; 

        //위아래 쳐다보기, 카메라 스크립트 구현 후 카메라만 움직이게 할 예정
        turnToX = turnToX + moveY * turnSpeed * Time.deltaTime; 
        
        //turnToY = Mathf.Clamp(turnToY, -80, 80);    //최대값 설정
        turnToX = Mathf.Clamp(turnToX, -20, 20);

        transform.eulerAngles = new Vector3(0, turnToY, 0);


    }

    /// <summary>
    /// Keyboard Q
    /// </summary>
    private void OnTempItemUse(InputAction.CallbackContext obj)     
    {

        //아이템 생성 ==> 성공
        //GameObject itemObj = ItemFactory.MakeItem((uint)ItemIDCode.HP_Potion, transform.position, Quaternion.identity);
        //아이템 사용
        //if(playerInventory.FindSameItemSlotForUseItem(potion). != null);
        if(playerInventory.FindSameItemSlotForUseItem(potion).SlotItemData != null)
        {
            int tempID;
            potion.Use(player);
            if(playerInventory.FindSameItemSlotForUseItem(potion).ItemCount == 1)
            {
                tempID = playerInventory.FindSameItemSlotForUseItem(potion).slotID;
                playerInventory.FindSameItemSlotForUseItem(potion).ClearSlotItem();
                playerInventoryUI.slotUIs[tempID].slotUIData = null;
                playerInventoryUI.slotUIs[tempID].slotUICount = 0;
                playerInventoryUI.SetAllSlotWithData();
            }
            else
            {
                tempID = playerInventory.FindSameItemSlotForUseItem(potion).slotID;
                playerInventory.FindSameItemSlotForUseItem(potion).ItemCount--;
                playerInventoryUI.slotUIs[tempID].slotUICount--;
                playerInventoryUI.SetAllSlotWithData();
            }
            
        }
        
        
    }

    /// <summary>
    /// Keyboard F를 눌러 실행
    /// </summary>
    private void OnTakeItem(InputAction.CallbackContext obj)    
    {
        Collider[] findItem = Physics.OverlapSphere(transform.position, findItemRange, LayerMask.GetMask("Item"));
        if(findItem.Length > 0)
        {
            GameObject tempObj = findItem[0].gameObject;
            Item tempItem = tempObj.GetComponent<Item>();

            playerInventory.TakeItem(tempItem.data, 1);
            playerInventoryUI.SetAllSlotWithData();
            Destroy(tempObj);

        }
    }

    /// <summary>
    /// Mouse Right Click (UI꺼져있을 때)
    /// </summary>
    /// <param name="obj"></param>
    private void OnTestMakeItem(InputAction.CallbackContext obj)    
    {
        ItemFactory.MakeItem(ItemIDCode.Basic_Weapon_1, transform.position, Quaternion.identity);
    }

    public void SetHP()
    {
        hpBar.value = HP / MaxHP;
    }

    public void SetExp()
    {
        expBar.value = Exp / MaxExp;
    }

    public void LevelUp()
    {
        level++;
        Exp -= MaxExp;
        MaxExp *= 1.3f;
        SetExp();
    }

    /// <summary>
    /// 바로 아래 위치한 애니메이션으로 attackTrigger조절하는 함수에 collider를 전해주기 위한 함수
    /// </summary>
    public void TakeWeapon()     
    {
        PlayerWeapon tempPlayerWeapon = FindObjectOfType<PlayerWeapon>();
        for(int i = 0; i < skillUses.Length; i++)   //무기 장착시 SkillUse클래스에서도 무기를 받아오도록 함(무기가 시작할 땐 장착되어있지 않아 SkillUse Awake에서 안한다)
        {
            skillUses[i].TakeWeapon();
        }
        if (tempPlayerWeapon != null)
        {
            weaponPrefab = tempPlayerWeapon.gameObject;
            weaponCollider = this.weaponPrefab.GetComponent<CapsuleCollider>();
            weaponCollider.enabled = false;
            isFindWeapon = true;
            Debug.Log("무기찾음");
        }
        else
        {
            isFindWeapon = false;
            Debug.Log("무기못찾음");
        }
        
    }

    public void EquipWeaponAbility()
    {
        AttackDamage += myWeapon.attackDamage;
    }

    public void UnEquipWeaponAbility()
    {
        attackDamage -= myWeapon.attackDamage;
        myWeapon = null;
    }

    /// <summary>
    /// 유니티 애니메이션에서 이벤트로 활성화 할 함수
    /// </summary>
    public void AttackTriggerOn()
    {
        if(isFindWeapon)
        weaponCollider.enabled = true;
    }

    /// <summary>
    /// 유니티 애니메이션에서 이벤트로 활성화 할 함수
    /// </summary>
    public void AttackTriggerOff()
    {
        if(isFindWeapon)
        weaponCollider.enabled = false;
    }

    /// <summary>
    /// 유니티 애니메이션에서 이벤트로 활성화 할 함수
    /// </summary>
    public void IsSkillUseOn()
    {
        isSkillUsing = true;
    }

    /// <summary>
    /// 유니티 애니메이션에서 이벤트로 활성화 할 함수
    /// </summary>
    public void IsSkillUseOff()
    {
        isSkillUsing = false;
    }

    /// <summary>
    /// 유니티 애니메이션에서 이벤트로 활성화 할 함수
    /// </summary>
    public void AttackSoundStart()
    {
        SoundPlayer.Instance?.PlaySound(SoundType.Sound_Attack, AttackSoundVolume);
    }

    /// <summary>
    /// 유니티 애니메이션에서 이벤트로 활성화 할 함수
    /// </summary>
    public void WalkSoundAndEffectStart()
    {
        SoundPlayer.Instance?.PlaySound(SoundType.Sound_Walk, walkSoundVolume);
        ParticlePlayer.Instance?.PlayParticle(ParticleType.ParticleSystem_Walk, transform.position, transform.rotation);
    }

}
