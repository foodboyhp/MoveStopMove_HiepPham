using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Toolbars;
using UnityEngine;
using UnityEngine.UI;

public class UIWeaponShop : UICanvas
{
    public Transform weaponPoint;
    [SerializeField] private WeaponData weaponData;
    [SerializeField] private ButtonState buttonState;
    [SerializeField] private TextMeshProUGUI nameTxt;
    [SerializeField] private TextMeshProUGUI playerCoinTxt;
    [SerializeField] private TextMeshProUGUI descriptionTxt;
    [SerializeField] private Text coinTxt;
    [SerializeField] private Text adsTxt;
    private Weapon currentWeapon;
    private WeaponType weaponType;

    public override void Setup()
    {
        base.Setup();
        ChangeWeapon(UserData.Ins.playerWeapon);
        playerCoinTxt.SetText(UserData.Ins.GetDataState(UserData.Key_Coin).ToString());
    }

    public override void CloseDirectly()
    {
        base.CloseDirectly();

        if (currentWeapon != null)
        {
            SimplePool.Despawn(currentWeapon);
            currentWeapon = null;
        }

        UIManager.Ins.OpenUI<UIMainMenu>();
    }

    public void NextButton()
    {
        ChangeWeapon(weaponData.NextType(weaponType));
    }

    public void PrevButton()
    {
        ChangeWeapon(weaponData.PrevType(weaponType));
    }

    public void BuyButton()
    {
        if (UserData.Ins.GetDataState(UserData.Key_Coin) >= weaponData.GetWeaponItem(weaponType).cost)
        {
            UserData.Ins.SetEnumData(weaponType.ToString(), ShopItem.State.Bought);
            UserData.Ins.SetDataState(UserData.Key_Coin, UserData.Ins.GetDataState(UserData.Key_Coin) - weaponData.GetWeaponItem(weaponType).cost);
            playerCoinTxt.SetText(UserData.Ins.GetDataState(UserData.Key_Coin).ToString());
            ChangeWeapon(weaponType);
        }
    }

    public void AdsButton()
    {
        // int ads = UserData.Ins.GetDataState(weaponType + "Ads", 0);
        // UserData.Ins.SetDataState(weaponType + "Ads", ads + 1);

        // if (ads + 1 >= weaponData.GetWeaponItem(weaponType).ads)
        // {
        //     UserData.Ins.SetDataState(weaponType.ToString(), 1);
        //     ChangeWeapon(weaponType);
        // }
    }

    public void EquipButton()
    {
        UserData.Ins.SetEnumData(weaponType.ToString(), ShopItem.State.Equipped);
        UserData.Ins.SetEnumData(UserData.Ins.playerWeapon.ToString(), ShopItem.State.Bought);
        UserData.Ins.SetEnumData(UserData.Key_Player_Weapon, ref UserData.Ins.playerWeapon, weaponType);
        ChangeWeapon(weaponType);
        LevelManager.Ins.player.TryCloth( UISkinShop.ShopType.Weapon, weaponType);
        LevelManager.Ins.player.ApplyBoosters();
    }

    public void ChangeWeapon(WeaponType weaponType)
    {
        this.weaponType = weaponType;

        if (currentWeapon != null )
        {
            SimplePool.Despawn(currentWeapon);
        }

        currentWeapon = SimplePool.Spawn<Weapon>((PoolType)weaponType, Vector3.zero, Quaternion.identity, weaponPoint);

        ButtonState.State state = (ButtonState.State)UserData.Ins.GetDataState(weaponType.ToString(), 0);
        buttonState.SetState(state);

        WeaponItem item = weaponData.GetWeaponItem(weaponType);
        nameTxt.SetText(item.name);
        descriptionTxt.SetText(currentWeapon.weaponBooster.boostDecription);
        coinTxt.text = item.cost.ToString();
        adsTxt.text = item.ads.ToString();
    }
}
