
public interface IEnemyState
{
    void EnterState(EnemyAI enemy);     // Called when entering the state
    void UpdateState(EnemyAI enemy);    // Called every frame while in the state
    void ExitState(EnemyAI enemy);      // Called when exiting the state
}
