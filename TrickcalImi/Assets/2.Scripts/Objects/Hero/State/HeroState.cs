using UnityEngine;

namespace FSM
{
    public class HeroIdleState : State<HeroManager>
    {
        public override void Enter(HeroManager owner)
        {
            owner.PlayAnim(HeroState.Idle);
        }

        public override void Excute(HeroManager owner)
        {
            
        }

        public override void Exit(HeroManager owner)
        {

        }
    }


    public class HeroWalkState : State<HeroManager>
    {
        public override void Enter(HeroManager owner)
        {
            owner.PlayAnim(HeroState.Walk);
        }

        public override void Excute(HeroManager owner)
        {

        }

        public override void Exit(HeroManager owner)
        {

        }
    }

    public class HeroChaseState : State<HeroManager>
    {
        public override void Enter(HeroManager owner)
        {
            owner.PlayAnim(HeroState.Chase);
        }

        public override void Excute(HeroManager owner)
        {
            owner.TestMove();
        }

        public override void Exit(HeroManager owner)
        {

        }
    }

    public class HeroAttackState : State<HeroManager>
    {
        public override void Enter(HeroManager owner)
        {
            owner.PlayAnim(HeroState.Attack);
        }

        public override void Excute(HeroManager owner)
        {

        }

        public override void Exit(HeroManager owner)
        {

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
}
