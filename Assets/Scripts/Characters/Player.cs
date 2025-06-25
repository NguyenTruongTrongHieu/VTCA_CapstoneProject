using NUnit.Framework;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 5f;
    public Rigidbody rb;
    public Vector3 startPos;
    public Vector3 startQuaternion;

    [Header("Behaviour Tree")]
    private Node rootNode;

    public void SetUpBehaviourTree()
    { 
        
    }
}

public class  CheckIfCurrentTurnIsPlayer : NodeBehaviourTree
{
    public override NodeState Evaluate()
    {
        state = GameManager.instance.currentTurn == "Player" ? NodeState.success : NodeState.failure;
        return state;
    }
}
