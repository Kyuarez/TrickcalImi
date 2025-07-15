using UnityEngine;

namespace FSM
{
    /// <summary>
    /// Stage�� �����ϴ� Object���� �Ѱ� State
    /// </summary>
    public abstract class State<T> where T : IngameObject
    {
        public abstract void Enter(T owner);
        public abstract void Excute(T owner);
        public abstract void Exit(T owner);
    }
}
