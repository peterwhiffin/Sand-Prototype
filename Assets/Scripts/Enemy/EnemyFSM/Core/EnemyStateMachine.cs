public class EnemyStateMachine
{
    public EnemyState CurrentState { get; private set; }
    public EnemyState NextState { get; private set; }
    public EnemyState PreviousState { get; private set; }

    public void Initialize(EnemyState startingState)
    {
        CurrentState = startingState;
        CurrentState.Enter();
    }

    public void ChangeState(EnemyState newState)
    {
        NextState = newState;
        PreviousState = CurrentState;
        CurrentState.Exit();
        CurrentState = newState;
        CurrentState.Enter();
    }
}
