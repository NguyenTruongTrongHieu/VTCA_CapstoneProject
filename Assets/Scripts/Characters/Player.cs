using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 5f;
    public Rigidbody rb;
    public Vector3 startPos;
    public Vector3 startQuaternion;
    public float stopDistanceWithEnemy = 1.5f;
    public float stopDistanceWithBoss = 2f;

    [Header("Animation")]
    public Animator animator;

    [Header("Behaviour Tree")]
    private NodeBehaviourTree rootNode;

    public void SetUpBehaviourTree()
    {
        rootNode = new Selector(new List<NodeBehaviourTree>
        {

            new Sequence(new List<NodeBehaviourTree>
            {
                new CheckIfGameStateIsPlaying(),

                new Selector(new List<NodeBehaviourTree>
                {
                    new Sequence(new List<NodeBehaviourTree>
                    {
                        new CheckDistanceReturnSuccessIfDistanceGreaterThanDistanceToCheck(this.transform,
                            LevelManager.instance.currentLevel.enemiesAtLevel[GameManager.instance.currentEnemyIndex].transform,
                            (true ? stopDistanceWithEnemy : stopDistanceWithBoss)),


                        new RotateToTarget(this.transform,
                        LevelManager.instance.currentLevel.enemiesAtLevel[GameManager.instance.currentEnemyIndex].transform, 3f),

                        new PlayerMoveToTarget(this,
                            LevelManager.instance.currentLevel.enemiesAtLevel[GameManager.instance.currentEnemyIndex].transform,
                            (true ? stopDistanceWithEnemy : stopDistanceWithBoss))
                    }),

                    new PlayerIdle(this),
                }),
            }),

            new PlayerIdle(this),

        }
        );
    }

    private void Start()
    {
        SetUpBehaviourTree();
    }

    private void Update()
    {
        rootNode.Evaluate();
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

public class PlayerMoveToTarget : NodeBehaviourTree
{
    private Player playerSelf;
    private Transform target;
    private float stopDistance;

    public PlayerMoveToTarget(Player player, Transform target, float stopDistance)
    {
        this.target = target;
        this.playerSelf = player;
        this.stopDistance = stopDistance;
    }

    public override NodeState Evaluate()
    {
        //Move player towards target
        Vector3 direction = (target.position - playerSelf.transform.position).normalized;
        if (Vector3.Distance(playerSelf.transform.position, target.position) > stopDistance)
        {
            playerSelf.rb.MovePosition(playerSelf.transform.position + direction * playerSelf.speed * Time.deltaTime);
            state = NodeState.running;
        }
        else
        {
            state = NodeState.success;
        }
        return state;
    }
}

public class PlayerIdle : NodeBehaviourTree
{
    private Player playerSelf;

    public PlayerIdle(Player player)
    {
        this.playerSelf = player;
    }

    public override NodeState Evaluate()
    {
        // Idle logic, e.g., waiting for input or other conditions
        state = NodeState.success; // For now, just return success
        Debug.Log("Idle");
        return state;
    }
}
