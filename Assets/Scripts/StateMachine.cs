using UnityEngine;

public class StateMachine
{
    public EntityState currentState {  get; private set; }
    public bool canChangeState = true;
    public void Initalize(EntityState startState)//注意是三参
    {
        currentState = startState;
        currentState.Enter();
    }
    public void ChangeState(EntityState newState)//重要，此处传入的是构造函数的多个参数
                                                 //Player player, StateMachine stateMachine, string animBoolName
    {
        if (canChangeState == false)
            return;
        currentState.Exit();//当前状态是三参包括animBoolName
        currentState = newState;
        currentState.Enter();
    }
    //状态机
    //初始化时将新状态赋值给当前状态，然后执行Enter（）
    //想要改变状态时，执行当前状态的Exit离开内容
    //将新状态赋给当前状态，然后执行新状态的Enter（）

    public void UpdataActiveState()
    {
        currentState.Update();//调用公开的Updata
    }
    public void 关闭状态机() => canChangeState = false;
}
