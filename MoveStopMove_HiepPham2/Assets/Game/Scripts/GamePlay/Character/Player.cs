using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Player : Character
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float BASE_MOVEMENT = 5f;

    // [SerializeField] private
    // public float Speed => BASE_MOVEMENT;
    private CounterTime counter = new CounterTime();
    private bool isMoving = false;
    private bool IsCanUpdate => GameManager.Ins.IsState(GameState.GamePlay) || GameManager.Ins.IsState(GameState.Setting);
    // private BoosterType weaponBoosterType;
    private SkinType skinType = SkinType.SKIN_Normal;
    private WeaponType weaponType = WeaponType.W_Candy_1;
    private HatType hatType = HatType.HAT_Cap;
    private AccessoryType accessoryType = AccessoryType.ACC_Headphone;
    private PantType pantType = PantType.Pant_1;

    //Booster scale
    List<IBooster> boosters;
    private const float BASE_SPEED = 1.0f;
    private const float BASE_RANGE = 1.0f;
    private const float BASE_ATTACK = 1.0f;
    private const int BASE_SHIELD = 0;
    [SerializeField] private float speedScale = 1.0f;
    [SerializeField] private float rangeScale = 1.0f;
    [SerializeField] private float attackScale = 1.0f;
    [SerializeField] private int shield = 0;

    //Instant Booster or booster in gameplay

    void Update()
    {
        if (IsCanUpdate && !IsDead)
        {

            if (Input.GetMouseButtonDown(0))
            {
                counter.Cancel();
            }

            if (Input.GetMouseButton(0) && JoystickControl.direct != Vector3.zero)
            {
                rb.MovePosition(rb.position + JoystickControl.direct * BASE_MOVEMENT * speedScale * Time.deltaTime);
                TF.position = rb.position;
                TF.forward = JoystickControl.direct;
                ChangeAnim(Constant.ANIM_RUN);
                isMoving = true;
            }
            else
            {
                counter.Execute();
            }

            if (Input.GetMouseButtonUp(0))
            {
                isMoving = false;
                OnMoveStop();
                OnAttack();

            }
        }
    }

    public override void OnInit()
    {
        OnTakeClothsData();
        base.OnInit();
        ApplyBoosters();
        TF.rotation = Quaternion.Euler(Vector3.up * 180);
        SetSize(MIN_SIZE);
        indicator.SetName("Player");
    }

    public override void WearClothes()
    {
        base.WearClothes(); 
        ChangeSkin(skinType);
        ChangeWeapon(weaponType);
        ChangeHat(hatType);
        ChangeAccessory(accessoryType);
        ChangePant(pantType);
    }

    public override void OnMoveStop()
    {
        base.OnMoveStop();
        rb.velocity = Vector3.zero;
        ChangeAnim(Constant.ANIM_IDLE);
    }

    public override void AddTarget(Character target)
    {
        base.AddTarget(target);

        if (!target.IsDead && !IsDead)
        {
            target.SetMask(true);
            if (!counter.IsRunning && !isMoving)
            {
                OnAttack();
            }
        }
    }

    public override void RemoveTarget(Character target)
    {
        base.RemoveTarget(target);
        target.SetMask(false);
    }

    public override void OnAttack()
    {
        base.OnAttack();
        if (target != null && currentSkin.Weapon.IsCanAttack)
        {
            counter.Start(Throw, TIME_DELAY_THROW * this.attackScale);
            ResetAnim();
        }
    }

    protected override void SetSize(float size)
    {
        base.SetSize(size * rangeScale);
        CameraFollow.Ins.SetRateOffset((this.size - MIN_SIZE) / (MAX_SIZE - MIN_SIZE));
    }

    internal void OnRevive()
    {
        ChangeAnim(Constant.ANIM_IDLE);
        IsDead = false;
        ClearTarget();
    }

    public override void OnHit(UnityAction hitAction){
        if(shield > 0) {
            shield --;
            Debug.Log("Shield is used");
        } else if (shield <= 0) {
            base.OnHit(hitAction);
        }
    }

    public override void OnDeath()
    {
        base.OnDeath();
        counter.Cancel();
    }

    public void TryCloth(UISkinShop.ShopType shopType, Enum type)
    {
        switch (shopType)
        {
            case UISkinShop.ShopType.Hat:
                currentSkin.DespawnHat();
                ChangeHat((HatType)type);
                break;

            case UISkinShop.ShopType.Pant:
                ChangePant((PantType)type);
                break;

            case UISkinShop.ShopType.Accessory:
                currentSkin.DespawnAccessory();
                ChangeAccessory((AccessoryType)type);
                break;

            case UISkinShop.ShopType.Skin:
                TakeOffClothes();
                skinType = (SkinType)type;
                WearClothes();
                break;
            case UISkinShop.ShopType.Weapon:
                currentSkin.DespawnWeapon();
                ChangeWeapon((WeaponType)type);
                break;
            default:
                break;
        }

    }

    //take cloth from data
    internal void OnTakeClothsData()
    {
        // take old cloth data
        skinType = UserData.Ins.playerSkin;
        weaponType = UserData.Ins.playerWeapon;
        hatType = UserData.Ins.playerHat;
        accessoryType = UserData.Ins.playerAccessory;
        pantType = UserData.Ins.playerPant;
    }

    //Speed boost
    public void BoostSpeed(float scaleAmount){
        this.speedScale += scaleAmount;
    }

    //Attack Speed boost
    public void BoostAttack(float scaleAmount){
        this.attackScale += scaleAmount;
    }

    //Range boost
    public void BoostRange(float scaleAmount){
        this.rangeScale += scaleAmount;
    }

    //Shield boost
    public void BoostShield(int shield){
        this.shield += shield;
    }

    public void ApplyBoosters(){
        boosters = new List<IBooster>();
        boosters.Add(currentSkin.skinBooster);
        boosters.Add(currentSkin.currentWeapon.weaponBooster);
        if(currentSkin.currentAccessory != null){
            boosters.Add(currentSkin.currentAccessory.accessoryBooster);
            Debug.Log("Accessory not null");
        } else {
            Debug.Log("Accessory is null");
        }
        ApplyBoosterList(boosters);
    }

    public void ApplyBoosterList(List<IBooster> boosters) {
        ResetBooster();
        foreach(IBooster booster in boosters){
            switch(booster.boosterType){
                case BoosterType.BOOST_Attack:
                    BoostAttack(booster.BoostAmount);
                    break;
                case BoosterType.BOOST_Range:
                    BoostRange(booster.BoostAmount);
                    break;
                case BoosterType.BOOST_Speed:
                    BoostSpeed(booster.BoostAmount);
                    break;
                case BoosterType.BOOST_Shield:
                    BoostShield((int)booster.BoostAmount);
                    break;
                default:
                    break;
            }
        }
    }

    private void ResetBooster(){
        this.speedScale = BASE_SPEED;
        this.attackScale = BASE_ATTACK;
        this.rangeScale = BASE_RANGE;
        this.shield = BASE_SHIELD;
    }
    public void ApplyInstantBooster(Booster booster){

    }

}
