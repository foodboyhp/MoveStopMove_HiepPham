using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AttackSpeedBooster", menuName = "Boosters/AttackSpeedBooster", order = 0)]
public class AttackSpeedBooster : IBooster
{
    [SerializeField] private float boostAmount;
    public override void Apply(Player target){
        Debug.Log("AttackSpeedBooster applied");
    }
    public override void Unapply(Player target){
        // target.ResetAttackSpeed();
        Debug.Log("AttackSpeedBooster unapplied");
    }
}
