using UnityEngine;

public class StateMachine
{
    public EntityState currentState { get; private set; }
    public bool canChangeState = true;
    public void Initialize(EntityState startState)//初始化
    {
        currentState = startState;
        currentState.Enter();
    }
    public void ChangeState(EntityState newState)
    {
        if (canChangeState == false)
            return;
        currentState.Exit();//当前状态是三参包括animBoolName
        currentState = newState;
        currentState.Enter();
    }
    public void UpdataActiveState()
    {
        currentState.Update();//调用公开的Updata
    }
    public void FixedUpdateForPlayerState()
    {
        currentState.FixedUpdate();//调用公开的Updata
    }
    public void DisableStateMachine()
    {
        canChangeState = false;
    }
}
