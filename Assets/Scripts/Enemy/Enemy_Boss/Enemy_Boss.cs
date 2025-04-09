using UnityEngine;

public class Enemy_Boss : Enemy
{
    public IdleState_Boss idleState { get; private set; }
    public MoveState_Boss moveState { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        idleState = new IdleState_Boss(this, stateMachine, "Idle");
        moveState = new MoveState_Boss(this, stateMachine, "Move");
    }

    protected override void Start()
    {
        base.Start();
        stateMachine.Initialize(idleState);
    }

    protected override void Update()
    {
        base.Update();  
        stateMachine.currentState.Update();
    }
}
