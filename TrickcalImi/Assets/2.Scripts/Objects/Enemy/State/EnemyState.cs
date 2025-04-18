using UnityEngine;

namespace FSM
{
    public class EnemyIdleState : State<EnemyManager>
    {
        public override void Enter(EnemyManager owner)
        {
            owner.PlayAnim(EnemyState.Idle);
        }

        public override void Excute(EnemyManager owner)
        {
            if(StageManager.Instance.IsPossibleGetTarget() == true)
            {
                if (owner.IsPossibleChase == true)
                {
                    if (owner.IsPossibleAttack == true)
                    {
                        owner.SetEnemyState(EnemyState.Attack);
                        return;
                    }

                    owner.SetEnemyState(EnemyState.Chase);
                }
                else
                {
                    owner.SetEnemyState(EnemyState.Walk);
                }
            }
        }

        public override void Exit(EnemyManager owner)
        {

        }
    }


    public class EnemyWalkState : State<EnemyManager>
    {
        public override void Enter(EnemyManager owner)
        {
            owner.PlayAnim(EnemyState.Walk);
        }

        public override void Excute(EnemyManager owner)
        {
            if(StageManager.Instance.IsPossibleGetTarget() == true)
            {
                if (owner.IsPossibleChase == true)
                {
                    if (owner.IsPossibleAttack == true)
                    {
                        owner.SetEnemyState(EnemyState.Attack);
                        return;
                    }

                    owner.SetEnemyState(EnemyState.Chase);
                }
            }
            else
            {
                owner.SetEnemyState(EnemyState.Idle);
            }

            owner.TestMove();           
        }

        public override void Exit(EnemyManager owner)
        {

        }
    }

    public class EnemyChaseState : State<EnemyManager>
    {
        public override void Enter(EnemyManager owner)
        {
            owner.PlayAnim(EnemyState.Chase);
        }

        public override void Excute(EnemyManager owner)
        {
            if(StageManager.Instance.IsPossibleGetTarget() == true)
            {
                if (owner.IsPossibleChase == true)
                {
                    if (owner.IsPossibleAttack == true)
                    {
                        owner.SetEnemyState(EnemyState.Attack);
                        return;
                    }
                }
                else
                {
                    owner.SetEnemyState(EnemyState.Walk);
                }

                owner.TestChase();
            }
            else
            {
                owner.SetEnemyState(EnemyState.Idle);
            }

        }

        public override void Exit(EnemyManager owner)
        {

        }
    }

    public class EnemyAttackState : State<EnemyManager>
    {
        private float elapsedTime;

        public override void Enter(EnemyManager owner)
        {
            owner.PlayAnim(EnemyState.Attack);
        }

        public override void Excute(EnemyManager owner)
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= owner.AttackDelay)
            {
                if(owner.AttackManager != null /* && owner.*/)
                {
                    //Target¿¡ Attack
                }

                owner.SetEnemyState(EnemyState.Idle);
                elapsedTime = 0f;
            }
        }

        public override void Exit(EnemyManager owner)
        {
            elapsedTime = 0f;
        }
    }

    public class EnemyHitState : State<EnemyManager>
    {
        public override void Enter(EnemyManager owner)
        {
            owner.PlayAnim(EnemyState.Hit);
        }

        public override void Excute(EnemyManager owner)
        {

        }

        public override void Exit(EnemyManager owner)
        {

        }
    }
}
