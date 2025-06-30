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

public class RotateToTarget : NodeBehaviourTree//success neu quay ve huong cua target, failure neu da quay ve huong cua target roi, running neu van chua quay ve huong cua target
{
    private Transform target;
    private Transform self;
    private float rotationSpeed;

    public RotateToTarget(Transform self, Transform target, float rotationSpeed)
    {
        this.target = target;
        this.self = self;
        this.rotationSpeed = rotationSpeed;
    }

    public override NodeState Evaluate()
    {
        state = NodeState.running;
        Vector3 direction = target.position - self.position;
        Quaternion lookRotation = Quaternion.LookRotation(direction);

        if (self.rotation == lookRotation)
        { 
            state = NodeState.failure;
            return state;
        }

        Player player = self.GetComponent<Player>();
        if (player != null)
        {
            player.animator.SetBool(player.isMovingHash, true);
        }

        Vector3 rotation = Quaternion.Lerp(self.rotation, lookRotation, Time.deltaTime * rotationSpeed).eulerAngles;
        self.rotation = Quaternion.Euler(0f, rotation.y, 0f);

        if (self.rotation == lookRotation)
        {
            state = NodeState.success;
        }

        return state;
    }
}

public class CheckDistanceReturnSuccessIfDistanceLessThanDistanceToCheck : NodeBehaviourTree
{
    private Transform self;
    private Transform target;
    private float distanceToCheck;

    public CheckDistanceReturnSuccessIfDistanceLessThanDistanceToCheck(Transform self, Transform target, float distance)
    {
        this.self = self;
        this.target = target;
        this.distanceToCheck = distance;
    }

    public override NodeState Evaluate()
    {
        if (target == null)
        {
            state = NodeState.failure;
            return state;
        }

        state = Vector3.Distance(self.position, target.position) <= distanceToCheck ? NodeState.success : NodeState.failure;// Neu khoang cach giua self va target nho hon hoac bang distanceToCheck, tra ve success, nguoc lai tra ve failure
        return state;
    }
}

public class CheckDistanceReturnSuccessIfDistanceGreaterThanDistanceToCheck : NodeBehaviourTree
{
    private Transform self;
    private Transform target;
    private float distanceToCheck;

    public CheckDistanceReturnSuccessIfDistanceGreaterThanDistanceToCheck(Transform self, Transform target, float distance)
    {
        this.self = self;
        this.target = target;
        this.distanceToCheck = distance;
    }

    public override NodeState Evaluate()
    {
        if (target == null)
        { 
            state = NodeState.failure;
            return state;
        }

        state = Vector3.Distance(self.position, target.position) > distanceToCheck ? NodeState.success : NodeState.failure;// Neu khoang cach giua self va target lon hon hoac bang distanceToCheck, tra ve success, nguoc lai tra ve failure
        return state;
    }
}

public class CheckIfGameStateIsPlaying : NodeBehaviourTree
{

    public override NodeState Evaluate()
    {
        state = GameManager.instance.currentGameState == GameState.Playing ? NodeState.success : NodeState.failure;// Neu trang thai game la playing, tra ve success, nguoc lai tra ve failure
        if (state == NodeState.success)
        {
            Debug.Log("Current game state is playing");
        }
        return state;
    }
}
