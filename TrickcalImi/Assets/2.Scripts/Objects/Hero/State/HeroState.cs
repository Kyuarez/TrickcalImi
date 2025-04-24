using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace FSM
{
    public class HeroIdleState : State<HeroManager>
    {
        public override void Enter(HeroManager owner)
        {
            owner.ResetTurn();
            owner.PlayAnim(HeroState.Idle);
        }

        public override void Excute(HeroManager owner)
        {
            if (StageManager.Instance.IsPossibleGetTarget() == true)
            {
                if (owner.IsPossibleChase == true)
                {
                    if (owner.IsPossibleAttack == true)
                    {
                        owner.SetHeroState(HeroState.Attack);
                        return;
                    }

                    owner.SetHeroState(HeroState.Chase);
                }
            }
            else
            {
                if(owner.IsReachedDestination == false)
                {
                    owner.SetHeroState(HeroState.Walk); 
                }
            }
        }

        public override void Exit(HeroManager owner)
        {

        }
    }


    public class HeroWalkState : State<HeroManager>
    {
        protected float moveSpeed;

        public HeroWalkState()
        {
            this.moveSpeed = 2.0f; //Default
        }
        public HeroWalkState(float moveSpeed)
        {
            this.moveSpeed = moveSpeed;
        }
        public override void Enter(HeroManager owner)
        {
            owner.PlayAnim(HeroState.Walk);
        }

        public override void Excute(HeroManager owner)
        {
            if (StageManager.Instance.IsPossibleGetTarget() == true)
            {
                if (owner.IsPossibleChase == true)
                {
                    if (owner.IsPossibleAttack == true)
                    {
                        owner.SetHeroState(HeroState.Attack);
                        return;
                    }

                    owner.SetHeroState(HeroState.Chase);
                }
            }
            else
            {
                if(owner.IsReachedDestination == true)
                {
                    owner.SetHeroState(HeroState.Idle);
                }
            }

            HeroBaseWalk(owner);
        }

        public override void Exit(HeroManager owner)
        {

        }
        protected virtual void HeroBaseWalk(HeroManager owner)
        {
            if(owner.Destination != null)
            {
                if (owner.transform.IsFront(owner.Destination) == true)
                {
                    owner.SetTurn(false);
                }
                else
                {
                    owner.SetTurn(true);
                }
                Vector3 currentPosition = owner.transform.position;
                owner.transform.position = Vector3.Lerp(currentPosition, owner.Destination, moveSpeed * Time.deltaTime);

            }
            else
            {
                owner.transform.position += Vector3.right * moveSpeed * Time.deltaTime;
            }
        }
    }

    public class HeroChaseState : State<HeroManager>
    {
        protected float chaseSpeed;
        public HeroChaseState()
        {
            this.chaseSpeed = 2.0f; //Default
        }
        public HeroChaseState(float chaseSpeed)
        {
            this.chaseSpeed = chaseSpeed;
        }
        public override void Enter(HeroManager owner)
        {
            owner.PlayAnim(HeroState.Chase);
        }

        public override void Excute(HeroManager owner)
        {
            if (StageManager.Instance.IsPossibleGetTarget() == true)
            {
                if (owner.IsPossibleChase == true)
                {
                    if (owner.IsPossibleAttack == true)
                    {
                        owner.SetHeroState(HeroState.Attack);
                        return;
                    }

                    HeroBaseChase(owner);
                }
                else
                {
                    owner.SetHeroState(HeroState.Walk);
                }
            }
            else
            {
                owner.SetHeroState(HeroState.Idle);
            }
        }

        public override void Exit(HeroManager owner)
        {

        }
        protected virtual void HeroBaseChase(HeroManager owner)
        {
            Transform target = owner.CurrentTarget.transform;
            Vector3 currentPosition = owner.transform.position;
            Vector3 targetPosition = new Vector3(target.position.x, target.position.y, currentPosition.z);
            owner.transform.position = Vector3.Lerp(currentPosition, targetPosition, chaseSpeed * Time.deltaTime);

            if (owner.transform.IsFront(target) == true)
            {
                owner.SetTurn(false);
            }
            else
            {
                owner.SetTurn(true);
            }
        }
    }

    public class HeroAttackState : State<HeroManager>
    {
        private float elapsedTime = 0f;

        public override void Enter(HeroManager owner)
        {
            if (owner.IsPossibleAttack == true)
            {
                owner.PlayAnim(HeroState.Attack);
                SoundManager.Instance.PlaySFX(owner.SoundWeaponID);
            }
            else
            {
                owner.SetHeroState(HeroState.Idle);
            }
        }

        public override void Excute(HeroManager owner)
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= owner.AttackDelay)
            {
                if (owner.AttackManager != null && owner.IsPossibleAttack == true)
                {
                    //Target¿¡ Attack
                    owner.AttackManager.OnNormalAttack(owner.CurrentTarget);
                }

                owner.SetHeroState(HeroState.Idle);
                elapsedTime = 0f;
            }
        }

        public override void Exit(HeroManager owner)
        {
            elapsedTime = 0f;
        }
    }

    public class HeroHitState : State<HeroManager>
    {
        public override void Enter(HeroManager owner)
        {
            owner.PlayAnim(HeroState.Hit);
        }

        public override void Excute(HeroManager owner)
        {

        }

        public override void Exit(HeroManager owner)
        {

        }
    }

    public class HeroDeadState : State<HeroManager>
    {
        public override void Enter(HeroManager owner)
        {
            owner.PlayAnim(HeroState.Dead);
            owner.OnDead?.Invoke();
            owner.OnDeadAction();
        }

        public override void Excute(HeroManager owner)
        {

        }

        public override void Exit(HeroManager owner)
        {

        }
    }
}
