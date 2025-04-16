using UnityEngine;

namespace FSM
{
    public class StateManager<T> where T : IngameObject
    {
        private T owner;
        private State<T> currentState;

        public void Setup(T owner, State<T> entryState)
        {
            this.owner = owner;
            currentState = null;

            ChangeState(entryState);
        }

        public void Excute()
        {
            if(currentState != null)
            {
                currentState.Excute(owner);
            }
        }

        public void ChangeState(State<T> state)
        {
            if(currentState != null)
            {
                currentState.Exit(owner);
                currentState = null;
            }

            currentState = state;
            currentState.Enter(owner);
        }
    }
}
