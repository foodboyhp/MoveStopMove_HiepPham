using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// public enum BoosterType{
//     None,
//     Attack_Speed_20,
//     Speed_20,
//     Shield_1,
//     Range_20
// }
public abstract class IBooster : ScriptableObject {
    public BoosterType boosterType;
    public String boostDecription;
    [SerializeField] private float boostAmount;
    public float BoostAmount => boostAmount;
    public abstract void Apply(Player target);
    public abstract void Unapply(Player target);
}