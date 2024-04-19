using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skin : GameUnit
{

    [SerializeField] PantData pantData;

    [SerializeField] Transform head; // hat
    [SerializeField] Transform rightHand; //weapon
    [SerializeField] Transform leftHand; //accessory
    [SerializeField] Renderer pant; // pant

    [SerializeField] bool isCanChange = false;

    public Weapon currentWeapon;
    public Accessory currentAccessory;
    Hat currentHat;
    [SerializeField] Animator anim;
    public Animator Anim => anim;

    public Weapon Weapon => currentWeapon;

    public IBooster skinBooster;

    public void ChangeWeapon(WeaponType weaponType)
    {
        currentWeapon = SimplePool.Spawn<Weapon>((PoolType)weaponType, rightHand);
    }

    public void ChangeAccessory(AccessoryType accessoryType)
    {
        if (isCanChange && accessoryType != AccessoryType.ACC_None)
        {
            currentAccessory = SimplePool.Spawn<Accessory>((PoolType)accessoryType, leftHand);
        }
    }

    public void ChangeHat(HatType hatType)
    {
        if (isCanChange && hatType != HatType.HAT_None)
        {
            currentHat = SimplePool.Spawn<Hat>((PoolType)hatType, head);
        }
    }

    public void ChangePant(PantType pantType)
    {
        pant.material = pantData.GetPant(pantType);
    }

    public void OnDespawn()
    {
        DespawnWeapon();
        DespawnAccessory();
        DespawnHat();
    }

    public void DespawnHat()
    {
        if (currentHat) SimplePool.Despawn(currentHat);
    }
    public void DespawnAccessory()
    {
        if (currentAccessory) {
            SimplePool.Despawn(currentAccessory);
            // currentAccessory = null;
        }
    }

    internal void DespawnWeapon()
    {
        if (currentWeapon) {
            SimplePool.Despawn(currentWeapon);
        }
    }

}