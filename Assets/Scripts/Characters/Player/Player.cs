using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using TMPro;

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
    public int isMovingHash;

    [Header("Behaviour Tree")]
    private NodeBehaviourTree rootNode;

    [Header("Boss SFX")]
    public string[] bossVoice;

    public void SetUpBehaviourTree()
    {
        rootNode = new Selector(new List<NodeBehaviourTree>
        {

            new Sequence(new List<NodeBehaviourTree>
            {
                new CheckIfGameStateIsPlaying(),

                new Sequence(new List<NodeBehaviourTree>//Move to enemy
                {
                    new CheckIfCurrentTurnIsNone(),

                    new Selector(new List<NodeBehaviourTree>
                    {
                        new Sequence(new List<NodeBehaviourTree>
                        {
                            new CheckDistanceReturnSuccessIfDistanceGreaterThanDistanceToCheck(this.transform,
                                LevelManager.instance.currentLevel.enemiesAtLevel[GameManager.instance.currentEnemyIndex].transform,
                                (LevelManager.instance.currentLevel.
                                enemiesAtLevel[GameManager.instance.currentEnemyIndex].GetComponent<EnemyStat>().
                                enemyType != EnemyType.boss ? stopDistanceWithEnemy : stopDistanceWithBoss)),


                            //new RotateToTarget(this.transform,
                            //LevelManager.instance.currentLevel.enemiesAtLevel[GameManager.instance.currentEnemyIndex].transform, 3f),

                            new PlayerMoveToTarget(this,
                                LevelManager.instance.currentLevel.enemiesAtLevel[GameManager.instance.currentEnemyIndex].transform,
                                (LevelManager.instance.currentLevel.
                                enemiesAtLevel[GameManager.instance.currentEnemyIndex].GetComponent<EnemyStat>().
                                enemyType != EnemyType.boss ? stopDistanceWithEnemy : stopDistanceWithBoss), 300f)
                        }),

                        //new Sequence(new List<NodeBehaviourTree>//Attack
                        //{ 
                        //    new CheckIfCurrentTurnIsPlayer(),
                        //}),

                        new PlayerIdle(this),
                    }),
                })
            }),

            new PlayerIdle(this),

        }
        );
    }

    private void Start()
    {
        //Setup Hash
        isMovingHash = Animator.StringToHash("isMoving");
    }

    private void Update()
    {
        rootNode.Evaluate();
    }

    public void ReturnStartPos()
    { 
        rb.isKinematic = true; // Disable physics interactions
        this.transform.position = startPos + new Vector3(0, 0.25f, 0);
        this.transform.rotation = Quaternion.Euler(startQuaternion);
        rb.isKinematic = false; // Re-enable physics interactions
    }

    public void PlayBossSFX()
    { 
        int randomIndex = Random.Range(0, bossVoice.Length);
        AudioManager.instance.PlaySFX(bossVoice[randomIndex]);
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

public class CheckIfCurrentTurnIsNone : NodeBehaviourTree
{
    public override NodeState Evaluate()
    {
        state = GameManager.instance.currentTurn == "None" ? NodeState.success : NodeState.failure;
        return state;
    }
}

public class PlayerMoveToTarget : NodeBehaviourTree
{
    private Player playerSelf;
    private Transform target;
    private float stopDistance;
    private float rotationSpeed; // Speed of rotation towards the target

    public PlayerMoveToTarget(Player player, Transform target, float stopDistance, float rotationSpeed)
    {
        this.target = target;
        this.playerSelf = player;
        this.stopDistance = stopDistance;
        this.rotationSpeed = rotationSpeed;
    }

    public override NodeState Evaluate()
    {
        playerSelf.animator.SetBool(playerSelf.isMovingHash, true);

        //Rotate towards target
        //if ()
        //{
        //    Vector3 direction = target.position - playerSelf.transform.position;
        //    Quaternion lookRotation = Quaternion.LookRotation(direction);

        //    Player player = playerSelf.transform.GetComponent<Player>();

        //    Vector3 rotation = Quaternion.Lerp(playerSelf.transform.rotation, lookRotation, Time.deltaTime * rotationSpeed).eulerAngles;
        //    playerSelf.transform.rotation = Quaternion.Euler(0f, rotation.y, 0f);
        //}

        Vector3 direction = target.position - playerSelf.transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        float dot = Vector3.Dot(playerSelf.transform.forward, direction.normalized);
        if (dot <= 0.99f)
        {
            playerSelf.transform.rotation = Quaternion.RotateTowards(playerSelf.transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
        }

        //Move player towards target
        //Vector3 direction = (target.position - playerSelf.transform.position).normalized;
        if (Vector3.Distance(playerSelf.transform.position, target.position) > stopDistance)
        {
            //playerSelf.rb.MovePosition(playerSelf.transform.position + direction * playerSelf.speed * Time.deltaTime);
            playerSelf.rb.isKinematic = true; // Disable physics interactions for smooth movement
            playerSelf.transform.position = Vector3.MoveTowards(playerSelf.transform.position, target.position, playerSelf.speed * Time.deltaTime);

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

        if (playerSelf.animator.GetBool(playerSelf.isMovingHash))//Gọi ngay khi player vừa di chuyển đến enemy
        {
            LevelManager.instance.currentLevel.enemiesAtLevel
                [GameManager.instance.currentEnemyIndex].GetComponent<EnemyStat>().SetHPSlider(false); // Set enemy HP slider when player is close enough to the enemy
            UIManager.instance.SetCurrentProgress(false); // Hide progress bar when player is close enough to the enemy

            GameManager.instance.currentTurn = "Player"; // Switch turn to Player when player is close enough to the enemy
            playerSelf.rb.isKinematic = false; // Disable physics interactions for smooth movement

            if (LevelManager.instance.currentLevel.
                enemiesAtLevel[GameManager.instance.currentEnemyIndex].GetComponent<EnemyStat>().
                enemyType == EnemyType.boss)
            {
                CameraManager.instance.StartCoroutine(CameraManager.instance.SetVerticalFOV(50f, 0.75f));
                CameraManager.instance.StartCoroutine(CameraManager.instance.SetHardLookAt(1f, 'Z', 1));
                CameraManager.instance.StartCoroutine(CameraManager.instance.SetFollowOffset(0.75f, 'X', 1));
                playerSelf.PlayBossSFX();
            }
        }
        
        playerSelf.animator.SetBool(playerSelf.isMovingHash, false);

        state = NodeState.success;
        return state;
    }
}
