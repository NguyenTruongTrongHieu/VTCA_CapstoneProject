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

public abstract class Node
{
    protected NodeState state;//Trang thai hien tai cua node
    public Node parent;//Node cha cua node hien tai
    protected List<Node> children = new List<Node>();//Danh sach cac node con cua node hien tai

    public Node()
    { }

    public Node(List<Node> children)
    {
        foreach (var node in children)
        {
            Attach(node);
        }
    }

    private void Attach(Node child)
    {
        child.parent = this;
        children.Add(child);
    }

    public abstract NodeState Evaluate();
}
