using System.Collections.Generic;
using UnityEngine;

public class Sequence : Node
{
    public Sequence() : base()
    {
    }

    public Sequence(List<Node> children) : base(children)
    {
    }

    public override NodeState Evaluate()
    {
        var anyChildRunning = false;
        foreach (var child in children)
        {
            switch (child.Evaluate())
            {
                case NodeState.running:
                    anyChildRunning = true;
                    continue;
                case NodeState.success:
                    continue;
                case NodeState.failure:
                    state = NodeState.failure;
                    return state;
                default:
                    state = NodeState.success;
                    return state;
            }
        }

        state = anyChildRunning ? NodeState.running : NodeState.success;
        return state;
    }
}
