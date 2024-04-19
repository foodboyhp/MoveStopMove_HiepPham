using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Booster : GameUnit
{
    // public BoosterType boosterType;
    [SerializeField] private IBooster booster;
    private float duration;
    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.TryGetComponent<Player>(out Player player)){
            player.ApplyInstantBooster(this);
        }
    }
}
