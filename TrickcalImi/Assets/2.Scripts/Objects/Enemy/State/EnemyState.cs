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
        protected float moveSpeed;

        public EnemyWalkState()
        {
            this.moveSpeed = 2.0f; //Default
        }
        public EnemyWalkState(float moveSpeed)
        {
            this.moveSpeed = moveSpeed;
        }   

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

            EnemyBaseWalk(owner);
        }

        public override void Exit(EnemyManager owner)
        {
        }

        protected virtual void EnemyBaseWalk(EnemyManager owner)
        {
            owner.transform.position += Vector3.left * moveSpeed * Time.deltaTime;
        }
    }

    public class EnemyChaseState : State<EnemyManager>
    {
        protected float chaseSpeed;
        public EnemyChaseState()
        {
            this.chaseSpeed = 2.0f; //Default
        }
        public EnemyChaseState(float chaseSpeed)
        {
            this.chaseSpeed = chaseSpeed;
        }
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
                        FXManager.Instance.OnEffect(FXType.Hit_Red, owner.CurrentTarget.transform.position);
                        return;
                    }
                }
                else
                {
                    owner.SetEnemyState(EnemyState.Walk);
                }

                EnemyBaseChase(owner);
            }
            else
            {
                owner.SetEnemyState(EnemyState.Idle);
            }

        }

        public override void Exit(EnemyManager owner)
        {

        }

        protected virtual void EnemyBaseChase(EnemyManager owner)
        {
            //@tk 임시 코드 : 본질적으로 막아야함.
            if(owner.CurrentTarget == null)
            {
                return;
            }

            Transform target = owner.CurrentTarget.transform;
            Vector3 currentPosition = owner.transform.position;
            Vector3 targetPosition = new Vector3(target.position.x, target.position.y, currentPosition.z);
            owner.transform.position = Vector3.Lerp(currentPosition, targetPosition, chaseSpeed * Time.deltaTime);
        }
    }

    public class EnemyAttackState : State<EnemyManager>
    {
        private float elapsedTime = 0f;

        public override void Enter(EnemyManager owner)
        {
            if(owner.IsPossibleAttack == true)
            {
                owner.PlayAnim(EnemyState.Attack);
            }
            else
            {
                owner.SetEnemyState(EnemyState.Idle);
            }
        }

        public override void Excute(EnemyManager owner)
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= owner.AttackDelay)
            {
                if(owner.AttackManager != null  && owner.IsPossibleAttack == true)
                {
                    //Target에 Attack
                    owner.AttackManager.OnNormalAttack(owner.CurrentTarget);
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


    public class EnemyDeadState : State<EnemyManager>
    {
        public override void Enter(EnemyManager owner)
        {
            owner.PlayAnim(EnemyState.Dead);
            owner.OnDead?.Invoke();
        }

        public override void Excute(EnemyManager owner)
        {

        }

        public override void Exit(EnemyManager owner)
        {

        }
    }
}
