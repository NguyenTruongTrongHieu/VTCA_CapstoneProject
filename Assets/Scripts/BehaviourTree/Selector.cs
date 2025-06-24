using System.Collections.Generic;
using UnityEngine;

public class Selector : NodeBehaviourTree
{
    public Selector() : base()
    {
    }

    public Selector(List<NodeBehaviourTree> children) : base(children)
    {
    }

    public override NodeState Evaluate()
    {
        foreach (var child in children)
        {
            switch (child.Evaluate())
            {
                case NodeState.running:
                    state = NodeState.running;
                    return state;
                case NodeState.success:
                    state = NodeState.success;
                    return state;
                case NodeState.failure:
                    continue;
                default:
                    state = NodeState.success;
                    return state;
            }
        }
        state = NodeState.failure;
        return state;
    }
}
