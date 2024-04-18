using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpeedBooster", menuName = "Boosters/SpeedBooster", order = 1)]

public class SpeedBooster : IBooster
{
    [SerializeField] private float boostAmount;
    public override void Apply(Player target){
        target.ScaleSpeed(1 + boostAmount/100.0f);
        Debug.Log("SpeedBooster applied");
    }
    public override void Unapply(Player target){
        target.ResetSpeed();
        Debug.Log("SpeedBooster unapplied");
    }
}
