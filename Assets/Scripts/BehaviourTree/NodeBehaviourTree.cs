using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public enum NodeState
{ 
    running,
    success,
    failure
}

public abstract class NodeBehaviourTree
{
    protected NodeState state;//Trang thai hien tai cua node
    public NodeBehaviourTree parent;//Node cha cua node hien tai
    protected List<NodeBehaviourTree> children = new List<NodeBehaviourTree>();//Danh sach cac node con cua node hien tai

    public NodeBehaviourTree()
    { }

    public NodeBehaviourTree(List<NodeBehaviourTree> children)
    {
        foreach (var node in children)
        {
            Attach(node);
        }
    }

    private void Attach(NodeBehaviourTree child)
    {
        child.parent = this;
        children.Add(child);
    }

    public abstract NodeState Evaluate();
}
