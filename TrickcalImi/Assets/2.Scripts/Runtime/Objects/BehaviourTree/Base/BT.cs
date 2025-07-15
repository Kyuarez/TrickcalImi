using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TK.BT
{
    /// <summary>
    /// Behaviour Tree
    /// </summary>
    public class BT : MonoBehaviour
    {
        //TODO : Blackboard
        public Blackboard blackBoard { get; private set; }
        public Root root { get; private set; }

        /// <summary>
        /// Tick이 돌고 있으면, BT 중복 실행 방지 변수
        /// </summary>
        private bool _isRunning;
        private Stack<Node> nodeStack = new Stack<Node>();

        private void Awake()
        {
            blackBoard = new Blackboard(gameObject);
            root = new Root(this);
            nodeStack = new Stack<Node>();
            _isRunning = false;
        }

        private void Update()
        {
            if (_isRunning) 
            {
                return;
            }

            _isRunning = true;
            StartCoroutine(CoTick());
        }

        private IEnumerator CoTick()
        {
            nodeStack.Push(root);

            while (nodeStack.Count > 0)
            {
                Node search = nodeStack.Pop();
                Result result = search.Excute();

                if(result == Result.Running)
                {
                    nodeStack.Push(search);
                    yield return null; 
                }
            }

            _isRunning = false;
        }
    }
}
