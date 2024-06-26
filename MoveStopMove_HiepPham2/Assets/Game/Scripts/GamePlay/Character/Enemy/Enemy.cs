using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : Character
{
    [SerializeField] private NavMeshAgent agent;
    // public float Speed => agent.speed;
    private IState<Enemy> currentState;
    private Vector3 destination;
    private CounterTime counter = new CounterTime();
    private bool IsCanRunning => (GameManager.Ins.IsState(GameState.GamePlay) || GameManager.Ins.IsState(GameState.Revive) || GameManager.Ins.IsState(GameState.Setting));
    public CounterTime Counter => counter;


    private void Update()
    {
        if (IsCanRunning && currentState != null && !IsDead)
        {
            currentState.OnExecute(this);
        }
    }

    public void ChangeState(IState<Enemy> state)
    {
        if (currentState != null)
        {
            currentState.OnExit(this);
        }
        currentState = state;
        if (currentState != null)
        {
            currentState.OnEnter(this);
        }

    }
    public override void OnInit()
    {
        base.OnInit();

        SetMask(false);
        ResetAnim();

        indicator.SetName(NameUtilities.GetRandomName());
    }

    public override void WearClothes()
    {
        base.WearClothes();
        //change random 
        ChangeSkin(SkinType.SKIN_Normal);
        ChangeWeapon(Utilities.RandomEnumValue<WeaponType>());
        ChangeHat(Utilities.RandomEnumValue<HatType>());
        ChangeAccessory(Utilities.RandomEnumValue<AccessoryType>());
        ChangePant(Utilities.RandomEnumValue<PantType>());
    }

    public override void OnDespawn()
    {
        base.OnDespawn();
        SimplePool.Despawn(this);
        CancelInvoke();
    }

    public override void OnDeath()
    {
        ChangeState(null);
        OnMoveStop();
        base.OnDeath();
        SetMask(false);
        Invoke(nameof(OnDespawn), 2f);
    }

    public override void OnMoveStop()
    {
        base.OnMoveStop();
        agent.enabled = false;
        ChangeAnim(Constant.ANIM_IDLE);
    }

    public bool IsDestination => Vector3.Distance(TF.position, destination) - Mathf.Abs(TF.position.y - destination.y) < 0.1f;

    public void SetDestination(Vector3 point)
    {
        destination = point;
        agent.enabled = true;
        agent.SetDestination(destination);
        ChangeAnim(Constant.ANIM_RUN);
    }

    public override void AddTarget(Character target)
    {
        base.AddTarget(target);

        if (!IsDead && Utilities.Chance(70, 100) && IsCanRunning)
        {
            ChangeState(new AttackState());
        }
    }
}
