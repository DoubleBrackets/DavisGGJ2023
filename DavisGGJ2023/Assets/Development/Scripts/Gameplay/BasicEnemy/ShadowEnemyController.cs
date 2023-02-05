using System;
using MushiSimpleFSM;
using UnityEngine;
using Random = UnityEngine.Random;

public class ShadowEnemyController : MonoBehaviour
{
    [ColorHeader("Invoking", ColorHeaderColor.InvokingChannels)]
    [SerializeField] private PlayVFXFuncChannelSO askPlayVFX;

    //[ColorHeader("Listening", ColorHeaderColor.InvokingChannels, true)]
    
    [ColorHeader("Dependencies", showDivider:true)]
    [SerializeField] private CombatEntity combatEntity;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRen;
    [SerializeField] private HeightBody2D heightBody;
    [SerializeField] private VFXEffectProfile deathVFX;
    [SerializeField] private GameStateSO gameState;

    [ColorHeader("Projectile Config", ColorHeaderColor.Config, true)]
    [SerializeField] private BasicEnemyProjectile projectilePrefab;
    [SerializeField] private float projectileLaunchCooldown;
    [SerializeField] private float projectileLaunchWindupTime;
    [SerializeField] private float projectileLaunchDuration;
    [SerializeField] private float projectileGravityAccel;
    [SerializeField] private float projectileTravelTime;
    [SerializeField] private float projectileMinYVel;
    
    [ColorHeader("Detection Config", ColorHeaderColor.Config, true)]
    [SerializeField] private float detectionDistance;
    [SerializeField] private float aboveDetectionRange;
    [SerializeField] private float belowDetectionRange;

    [ColorHeader("Debug", showDivider: true)]
    [SerializeField, ReadOnly] private EnemyState debugState;
    
    private ProtagBlackboard protagBlackboard => gameState.ProtagBlackboard;

    // Internal state
    private float staggerDuration = 0f;
    private Vector3 launchTargetPositionRaw;

    private AttackProfileSO recentAttacked;
    private AttackInfo recentAttackedInfo;

    private float prevLaunchTime;

    private Vector2 protagHorizontalPos;
    private float protagHeight;
    

    private enum EnemyState
    {
        Idle,
        Staggered,
        Airborne,
        Launching
    }
    
    private EnemyState currentState;
    private QStateMachine stateMachine;
    
    private void OnEnable()
    {
        combatEntity.onAttackReceived += ReceiveAttack;
        heightBody.onHitHazard += OnHazard;
        CreateStateMachine();
    }
    
    private void OnDisable()
    {
        combatEntity.onAttackReceived -= ReceiveAttack;
        heightBody.onHitHazard -= OnHazard;
        stateMachine.ExitStateMachine();
    }

    private void CreateStateMachine()
    {
        stateMachine = new QStateMachine((int)EnemyState.Idle);
        stateMachine.AddNewState(
            (int)EnemyState.Idle,
            IdleUpdate,
            IdleFixedUpdate,
            IdleEnterState,
            IdleExitState,
            IdleSwitchState);

        stateMachine.AddNewState(
            (int)EnemyState.Staggered,
            StaggeredUpdate,
            StaggeredFixedUpdate,
            StaggeredEnterState,
            StaggeredExitState,
            StaggeredSwitchState);

        stateMachine.AddNewState(
            (int)EnemyState.Launching,
            LaunchingUpdate,
            LaunchingFixedUpdate,
            LaunchingEnterState,
            LaunchingExitState,
            LaunchingSwitchState);

        stateMachine.AddNewState(
            (int)EnemyState.Airborne,
            AirborneUpdate,
            AirborneFixedUpdate,
            AirborneEnterState,
            AirborneExitState,
            AirborneSwitchState);
    }

    private void FixedUpdate()
    {
        if (protagBlackboard)
        {
            var protagHBody = protagBlackboard.heightBody;
            protagHorizontalPos = protagHBody.horizontalPos;
            protagHeight = protagHBody.height;
        }
        stateMachine.FixedUpdateStateMachine();
    }

    private bool Detected()
    {
        if (!protagBlackboard ||
            !protagBlackboard.heightBody.gameObject.activeInHierarchy) return false;
        
        Vector2 horizontalPos = heightBody.horizontalPos;
        float verticalPos = heightBody.height;
        bool inHeightRange = 
            verticalPos < protagHeight + aboveDetectionRange && 
            verticalPos > protagHeight - belowDetectionRange;

        Vector3 protagPos = (Vector3)protagHorizontalPos + protagHeight * Vector3.forward;
        Vector3 enemyPos = (Vector3)horizontalPos + verticalPos * Vector3.forward;

        if ((protagPos - enemyPos).sqrMagnitude < detectionDistance * detectionDistance)
        {
            return true;
        }

        return false;
    }

    private void PlayAnim(string name)
    {
        animator.Play($"shadowEnemy_{name}");
    }

    private void Update()
    {
        stateMachine.UpdateStateMachine();
        debugState = currentState;
    }

    private bool ReceiveAttack(AttackProfileSO attackProfile, AttackInfo attack)
    {
        recentAttacked = attackProfile;
        recentAttackedInfo = attack;
        if (currentState == EnemyState.Staggered)
        {
            StaggeredHit();
        }
        else
        {
            stateMachine.SwitchState((int)EnemyState.Staggered);
        }
        return true;
    }
    
    private void OnHazard()
    {
        OnKilled(Vector2.zero, Vector2.up);
    }

    private void OnKilled(Vector2 collisionVel, Vector2 initialVel)
    {
        Vector3 pos = heightBody.TransformPosition;
        pos += initialVel.GetAngle() * Vector3.right;
        var vfx = askPlayVFX.CallFunc(deathVFX, 0, new PlayVFXSettings()
        {
            position = pos,
            rotation = initialVel.GetAngle()
        });
        Destroy(gameObject);
    }

    #region Idle State

    public void IdleUpdate()
    {
        
    }

    public void IdleFixedUpdate()
    {
        heightBody.horizontalVel = Vector2.MoveTowards(
            heightBody.horizontalVel, 
            Vector2.zero, 
            40f * Time.fixedDeltaTime);
    }

    public void IdleEnterState()
    {
        PlayAnim("Idle");
    }

    public void IdleExitState()
    {

    }

    public int IdleSwitchState()
    {
        if (Time.time - prevLaunchTime > projectileLaunchCooldown && Detected())
        {
            return (int)EnemyState.Launching;
        }
        return -1;
    }

    #endregion

    #region Staggered State

    public void StaggeredUpdate()
    {
        staggerDuration -= Time.deltaTime;
        spriteRen.flipX = heightBody.horizontalVel.x < 0f;
    }

    public void StaggeredFixedUpdate()
    {

    }

    public void StaggeredEnterState()
    {
        heightBody.onHorizontalCollide += OnKilled;
        spriteRen.flipX = heightBody.horizontalVel.x < 0f;
        StaggeredHit();
        PlayAnim("Staggered");
    }

    private void StaggeredHit()
    {
        var attackInfo = recentAttackedInfo;
        var attackProfile = recentAttacked;
        
        heightBody.horizontalVel = 
            attackInfo.attackAngle * Vector2.right * (attackProfile.KnockbackVelocity * attackInfo.knockbackRatio);

        heightBody.verticalVelocity = attackProfile.VerticalKnockbackVelocity * attackInfo.knockbackRatio;

        staggerDuration = attackProfile.KnockbackStaggerDuration;
        
    }

    public void StaggeredExitState()
    {
        heightBody.horizontalVel *= 0.5f;
        heightBody.onHorizontalCollide -= OnKilled;
        spriteRen.flipX = false;
    }

    public int StaggeredSwitchState()
    {
        if (staggerDuration < 0f)
        {
            return (int)EnemyState.Idle;
        }
        return -1;
    }

    #endregion

    #region Launching State

    private bool launched = false;
    private float launchStartTime;

    public void LaunchingUpdate()
    {
        if (!launched && Time.time - launchStartTime > projectileLaunchWindupTime)
        {
            launched = true;
            LaunchProjectile();
        }
    }

    public void LaunchingFixedUpdate()
    {
        heightBody.horizontalVel = Vector2.MoveTowards(
            heightBody.horizontalVel, 
            Vector2.zero, 
            40f * Time.fixedDeltaTime);
    }

    public void LaunchingEnterState()
    {
        launchStartTime = Time.time;
        launched = false;
        PlayAnim("Launch");
    }

    private void LaunchProjectile()
    {
        float t = projectileTravelTime;
        float initialHeight = heightBody.height + 0.3f;
        Vector2 initialPosition = heightBody.horizontalPos;

        float targetHeight = protagHeight;
        Vector2 targetPosition = protagHorizontalPos;
        
        var projectile = Instantiate(projectilePrefab);

        // Projectile maffs
        float hDiff = targetHeight - initialHeight;
        float verticalVel = (hDiff +
                      (0.5f * projectileGravityAccel * t * t))
                     / t;

        // Calculate the time needed for the minimum y vel
        if (verticalVel < projectileMinYVel)
        {
            verticalVel = projectileMinYVel;
            float delta = verticalVel * verticalVel - 2 * hDiff * projectileGravityAccel;
            if (delta < 0)
            {
                Debug.LogError("What the fuck");
            }

            t = (verticalVel + delta) / -projectileGravityAccel;
        }

        // Movement prediction (with randomness eeehhee)
        targetPosition += Random.Range(0.1f, 1f) * protagBlackboard.heightBody.horizontalVel / t;

        Vector2 horizontalVel = (targetPosition - initialPosition) / t;
        
        projectile.SetVelocity(horizontalVel, verticalVel, projectileGravityAccel);
        projectile.SetPosition(heightBody.horizontalPos, initialHeight);
    }

    public void LaunchingExitState()
    {
        prevLaunchTime = Time.time;
    }

    public int LaunchingSwitchState()
    {
        if (Time.time - launchStartTime > projectileLaunchDuration)
        {
            return (int)EnemyState.Idle;
        }
        return -1;
    }

    #endregion

    #region Airborne State

    public void AirborneUpdate()
    {
        spriteRen.flipX = heightBody.horizontalVel.x < 0f;
    }

    public void AirborneFixedUpdate()
    {

    }

    public void AirborneEnterState()
    {
        PlayAnim("Staggered");
    }

    public void AirborneExitState()
    {

    }

    public int AirborneSwitchState()
    {
        if (heightBody.isGrounded)
        {
            return (int)EnemyState.Idle;
        }
        return -1;
    }

    #endregion
}
