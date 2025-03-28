using UnityEngine;

public class EnemyState 
{
    protected Enemy enemyBase;
    protected EnemyStateMachine stateMachine;
    protected Animator anim;

    protected string animBoolName;  

    public EnemyState(Enemy enemyBase, EnemyStateMachine stateMachine, string animBoolName)
    {
        this.enemyBase = enemyBase;
        this.stateMachine = stateMachine;
        this.animBoolName = animBoolName;
    }

    public virtual void Update()
    {
        Debug.Log("Updating State" + animBoolName);
    }

    public virtual void Enter()
    {
        Debug.Log("Entered State" + animBoolName);
    }

    public virtual void Exit()
    {
        Debug.Log("Exited State" + animBoolName);
    }

}
