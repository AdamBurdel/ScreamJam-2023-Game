﻿using CandiceAIforGames.AI;
using CandiceAIforGames.AI.Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CandiceAIforGames.AI
{
    #region ENUMS
    public enum PatrolType
    {
        PatrolPoints,
        Waypoints,
    }
    public enum AttackType
    {
        Melee,
        Range,
    }
    public enum MovementType
    {
        STATIC,
        DYNAMIC,
        TILE_BASED
    }
    public enum PathfindSource
    {
        None,
        Candice,
        UnityNavmesh,
    }
    public enum SensorType
    {
        Sphere,
    }
    public enum AnimationType
    {
        TransitionBased,
        CodeBased,
    }
    #endregion
    //New inheritance chain from CandiceAnimationManager, which in turns inherits from MonoBehaviour. 
    //Scene Manager will override all aspects on future releases
    public class CandiceAIController : CandiceAnimationManager
    {

        #region Member Variables
        /*
         * General Variables
         */
        object col;
        [SerializeField]
        private int agentID;
        [SerializeField]
        private float maxHitPoints = 100f;
        [SerializeField]
        public float hitPoints = 100f;
        [SerializeField]
        private float halfHeight = 100f;
        [SerializeField]
        private bool is3D = true;


        /*
         * New Animation Variables
         */
        [SerializeField]
        private Camera mainCamera;
        [SerializeField]
        private GameObject rig;

        [SerializeField]
        private bool enableRagdoll = false;

        //New Animation System fields
        [SerializeField]
        public Animator templateAnimator;            

        /*
         * End Of New Animation Variables
         */

        [SerializeField]
        private List<GameObject> enemies = new List<GameObject>();
        [SerializeField]
        private List<GameObject> allies = new List<GameObject>();
        [SerializeField]
        private List<GameObject> players = new List<GameObject>();
        [SerializeField]
        private GameObject player;
        [SerializeField]
        private GameObject mainTarget;
        [SerializeField]
        private GameObject fleeTarget;
        [SerializeField]
        private GameObject wanderTarget;

        [SerializeField]
        public CandiceBehaviorTree behaviorTree;
        public CandiceGOAPState agentState = new CandiceGOAPState(new Dictionary<string, int>
        {
            {"wood", 0},
            {"axe", 0}
        }, "Game State");

        /*
         * Detection Variables
         */
        [SerializeField]
        private float detectionRadius = 3f;
        [SerializeField]
        private int detectionLines = 10;
        [SerializeField]
        private float lineOfSight = 3f;
        [SerializeField]
        private float detectionHeight = 3f;
        [SerializeField]
        private SensorType sensorType = SensorType.Sphere;
        [SerializeField]
        private bool objectDetected = false;
        [SerializeField]
        private bool resourceDetected = false;
        [SerializeField]
        private bool playerDetected = false;
        [SerializeField]
        private bool allyDetected = false;
        [SerializeField]
        private bool enemyDetected = false;
        [SerializeField]
        private List<string> objectTags = new List<string>();
        [SerializeField]
        public List<string> enemyTags = new List<string>();
        [SerializeField]
        public List<string> resourceTags = new List<string>();
        [SerializeField]
        private List<string> allyTags = new List<string>();
        [SerializeField]
        private LayerMask perceptionMask;
        [SerializeField]
        private float obstacleAvoidaceDistance = 3f;
        [SerializeField]
        private float obstacleAvoidanceAOE = 0.5f;
        [SerializeField]
        private Vector3 lookPoint;

        /*
         * Movement Variables
         */
        [SerializeField]
        private Vector3 movePoint;
        [SerializeField]
        public float moveSpeed = 7f;
        [SerializeField]
        private float rotationSpeed = 12f;
        [SerializeField]
        private bool isMoving = false;
        [SerializeField]
        private bool enableHeadLook = false;
        [SerializeField]
        private GameObject headLookTarget;
        [SerializeField]
        private float headLookIntensity = 1f;
        [SerializeField]
        private CandiceWaypoint waypoint;

        /*
         * Pathfinding Variables
         */
        Path _path;//The path that the Agent will use to follow.
        [SerializeField]
        public float _minPathUpdateTime = .2f;//Minimum time it will take for the agent before attempting to request a new updated path from Candice.
        [SerializeField]
        public float _pathUpdateMoveThreshold = .5f;// Minimum distance the target can move by before requesting a new Updated path from Candice.
        [SerializeField]
        public float _turnSpeed;//The speed the agent will turn between waypoints by when pathfinding.
        [SerializeField]
        public float _turnDist;//The ditance the agent will start to turn while moving to the next node.
        [SerializeField]
        public float _stoppingDist;//How far away from the target the agent will start to slow down and stop.
        bool _followingPath;//Whether or not the agent is following a path.
        private bool switchWanderTarget = true;

        [SerializeField]
        private bool isCalculatingPath = false;
        [SerializeField]
        private bool isFollowingPath = false;
        [SerializeField]
        float sqrMoveThreshold;
        [SerializeField]
        int pathIndex = 0;
        [SerializeField]
        private bool drawAgentPath = false;
        Vector3 targetPosOld;

        /*
         * Combat Variables
         */
        [SerializeField]
        private GameObject attackTarget;
        [SerializeField]
        private GameObject resourceTarget;
        [SerializeField]
        public float attackDamage = 3f;
        [SerializeField]
        private float attacksPerSecond = 1f;
        [SerializeField]
        private float attackRange = 1f;
        [SerializeField]
        private float damageAngle = 45f;
        [SerializeField]
        private AttackType attackType = AttackType.Melee;
        [SerializeField]
        public GameObject projectile;
        [SerializeField]
        private Transform projectileSpawnPos;
        [SerializeField]
        private bool hasAttackAnimation = false;
        [SerializeField]
        public bool isAttacking = false;

        /*
         * Modules
         */
        public CandiceModuleMovement movementModule;
        public CandiceModuleDetection detectionModule;
        public CandiceModuleCombat combatModule;


        /*
         * Properties
         */
        public int AgentID { get => agentID; set => agentID = value; }
        
        public List<GameObject> Enemies { get => enemies; set => enemies = value; }
        public List<GameObject> GameResources { get => enemies; set => enemies = value; }
        public List<GameObject> Allies { get => allies; set => allies = value; }
        public List<GameObject> Players { get => players; set => players = value; }
        public GameObject Player { get => player; set => player = value; }
        public bool ObjectDetected { get => objectDetected; set => objectDetected = value; }
        public bool ResourceDetected { get => resourceDetected; set => resourceDetected = value; }
        public bool PlayerDetected { get => playerDetected; set => playerDetected = value; }
        public bool AllyDetected { get => allyDetected; set => allyDetected = value; }
        public bool EnemyDetected { get => enemyDetected; set => enemyDetected = value; }
        public GameObject MainTarget { get => mainTarget; set => mainTarget = value; }
        public float MoveSpeed { get => moveSpeed; set => moveSpeed = value; }
        public bool IsMoving { get => isMoving; set => isMoving = value; }
        public SensorType SensorType { get => sensorType; set => sensorType = value; }
        public float DetectionRadius { get => detectionRadius; set => detectionRadius = value; }
        public float LineOfSight { get => lineOfSight; set => lineOfSight = value; }
        public float DetectionHeight { get => detectionHeight; set => detectionHeight = value; }
        public bool Is3D { get => is3D; set => is3D = value; }
        public float AttackDamage { get => attackDamage; set => attackDamage = value; }
        public float AttackSpeed { get => 1f / attacksPerSecond;}
        public float AttacksPerSecond { get => attacksPerSecond; set => attacksPerSecond = value; }
        public LayerMask PerceptionMask { get => perceptionMask; set => perceptionMask = value; }
        public float ObstacleAvoidaceDistance { get => obstacleAvoidaceDistance; set => obstacleAvoidaceDistance = value; }
        public float ObstacleAvoidanceAOE { get => obstacleAvoidanceAOE; set => obstacleAvoidanceAOE = value; }
        public List<string> EnemyTags { get => enemyTags; set => enemyTags = value; }
        public List<string> ResourceTags { get => resourceTags; set => resourceTags = value; }
        public List<string> AllyTags { get => allyTags; set => allyTags = value; }
        public List<string> ObjectTags { get => objectTags; set => objectTags = value; }
        public float AttackRange { get => attackRange; set => attackRange = value; }
        public bool EnableHeadLook { get => enableHeadLook; set => enableHeadLook = value; }
        public GameObject HeadLookTarget { get => headLookTarget; set => headLookTarget = value; }
        public float HeadLookIntensity { get => headLookIntensity; set => headLookIntensity = value; }
        public GameObject AttackTarget { get => attackTarget; set => attackTarget = value; }
        public GameObject ResourceTarget { get => resourceTarget; set => resourceTarget = value; }
        public Vector3 MovePoint { get => movePoint; set => movePoint = value; }
        public AttackType AttackType { get => attackType; set => attackType = value; }
        public GameObject Projectile { get => projectile; set => projectile = value; }
        public Transform ProjectileSpawnPos { get => projectileSpawnPos; set => projectileSpawnPos = value; }
        public bool HasAttackAnimation { get => hasAttackAnimation; set => hasAttackAnimation = value; }
        public float DamageAngle { get => damageAngle; set => damageAngle = value; }
        public bool IsAttacking { get => isAttacking; set => isAttacking = value; }
        public Camera MainCamera { get => mainCamera; set => mainCamera = value; }
        public GameObject Rig { get => rig; set => rig = value; }
        public bool EnableRagdoll { get => enableRagdoll; set => enableRagdoll = value; }
        public float MaxHitPoints { get => maxHitPoints; set => maxHitPoints = value; }
        public float HitPoints { get => hitPoints; set => hitPoints = value; }
        public bool IsCalculatingPath { get => isCalculatingPath; set => isCalculatingPath = value; }
        public bool IsFollowingPath { get => isFollowingPath; set => isFollowingPath = value; }
        public Vector3 LookPoint { get => lookPoint; set => lookPoint = value; }
        public bool DrawAgentPath { get => drawAgentPath; set => drawAgentPath = value; }
        public float RotationSpeed { get => rotationSpeed; set => rotationSpeed = value; }
        public int DetectionLines { get => detectionLines; set => detectionLines = value; }
        public float HalfHeight { get => halfHeight; set => halfHeight = value; }
        public CandiceBehaviorTree BehaviorTree { get => behaviorTree; set => behaviorTree = value; }
        public CandiceWaypoint Waypoint { get => waypoint; set => waypoint = value; }
        //New Animations properties for the agent
        public Animator thisAnimator { get => templateAnimator; set => templateAnimator = value; }
        public float AnimationSpeed { get => animSpeed; set => animSpeed = value; }
        public float GlobalAnimationsSpeed { get => globalSpeed; set => globalSpeed = value; }
        public ScriptableObject CameraShakeData { get => shakeData; set => shakeData = value; }
        public bool attachKillCamera { get => attachKillCam; set => attachKillCam = value; }
        public bool isTitanic { get => isATitan; set => isATitan = value; }
        public GameObject InventoryDrop { get => inventoryDrop; set => inventoryDrop = value; }

        CandiceAIManager candice;
        private List<Action<bool, int>> readyStateListeners = new List<Action<bool, int>>();

        #endregion
        // Start is called before the first frame update
        void Start()
        {
            //Check if there is a Candice AI Manager Component in the scene.
            candice = FindObjectOfType<CandiceAIManager>();
            if (candice == null)
            {
                Debug.LogError("You need to attach a Candice AI Manager Component to an Empty GameObject.");
            }
            else
            {
                candice.RegisterAgent(gameObject, onRegistrationComplete);
            }
            if (is3D)
                col = GetComponent<Collider>();
            else
                col = GetComponent<Collider2D>();

            //New Animations initialization           
            InitializeAnimations();
        }


        // Update is called once per frame
        void Update()
        {
            if (BehaviorTree != null)
                BehaviorTree.Evaluate();

            //New Animations Assessment
            Animate();

        }
        public void AddRegistrationListener(Action<bool, int> listener)
        {
            readyStateListeners.Add(listener);
        }
        #region Helper Methods 
        /// <summary>
        /// Callback function when the agent is successfully registered with Candice.
        /// </summary>
        private void onRegistrationComplete(bool isRegistered, int agentId)
        {

            if (isRegistered)
            {
                AgentID = agentId;
                combatModule = new CandiceModuleCombat(transform, onAttackComplete, "Agent" + AgentID + "-CandiceModuleCombat");
                movementModule = new CandiceModuleMovement("Agent" + AgentID + "-CandiceModuleMovement");
                detectionModule = new CandiceModuleDetection(gameObject.transform, onObjectFound, "Agent" + AgentID + "-CandiceModuleDetection");
                if(is3D)
                    HalfHeight = ((Collider)col).bounds.extents.y;
                else
                    HalfHeight = ((Collider2D)col).bounds.extents.y;
                //BehaviorTree = GetComponent<CandiceBehaviorTree>();
                if (BehaviorTree != null && BehaviorTree.nodes != null)
                {
                    BehaviorTree.Initialise();
                    BehaviorTree.CreateBehaviorTree(this, OnBTComplete);
                }
                foreach(Action<bool, int> listener in readyStateListeners)
                {
                    listener(isRegistered, agentID);
                }
                //Debug.Log("Agent " + AgentID + " successfully registered with Candice.");
            }

        }

        public void OnBTComplete(CandiceBehaviorTreeEventData data)
        {

        }
        #endregion
        /// <summary>
        /// Use the detection module to scan for objects.
        /// </summary>
        public void ScanForObjects()
        {
            CandiceDetectionRequest req = new CandiceDetectionRequest(sensorType, objectTags, DetectionRadius, DetectionHeight, LineOfSight, Is3D);
            detectionModule.ScanForObjects(req);
        }
        /// <summary>
        /// Use the detection module to scan for objects ina 2D space.
        /// </summary>
        public void ScanForObjects2D()
        {
            CandiceDetectionRequest req = new CandiceDetectionRequest(sensorType, objectTags, DetectionRadius, DetectionHeight, LineOfSight, Is3D);
            detectionModule.ScanForObjects2D(req);
        }

        /// <summary>
        /// Use the detection module to scan for objects ina 2D space.
        /// </summary>
        public void GetPickaxe()
        {
            
        }

        /// <summary>
        /// Use the detection module's obstacle avoidance to evade nearby obstacles.
        /// </summary>
        public void AvoidObstacles()
        {
            
            if (col != null)
            {
                if (is3D)
                    HalfHeight = ((Collider)col).bounds.extents.x * 2;
                else
                    HalfHeight = ((Collider2D)col).bounds.extents.x * 2;
            }
            else
            {
                HalfHeight = gameObject.transform.localScale.x * 2;
            }
            detectionModule.AvoidObstacles(MainTarget.transform, MovePoint, transform,HalfHeight + obstacleAvoidanceAOE,RotationSpeed,true,ObstacleAvoidaceDistance,DetectionLines,PerceptionMask);
        }
        /// <summary>
        /// Do some checks on whether a new path must be requested or not.
        /// </summary>
        /// <returns>Whetehr the agent is following the path.</returns>
        public bool CandicePathfind()
        {
            
            if (!IsCalculatingPath && !IsFollowingPath)
            {
                CalculateAStarPath();
            }
            else if(IsFollowingPath)
            {
                if ((MovePoint - targetPosOld).sqrMagnitude > sqrMoveThreshold)
                {
                    CalculateAStarPath();
                }
                else
                {
                    FollowAStarPath();
                }
            }
            return IsFollowingPath;
        }
        /// <summary>
        /// Function to make a request to the Candice AI Manager for a new path.
        /// </summary>
        /// <returns></returns>
        private void CalculateAStarPath()
        {
            Debug.Log("Calculate");
            sqrMoveThreshold = _pathUpdateMoveThreshold * _pathUpdateMoveThreshold;
            candice.RequestASTARPath(new PathRequest(transform.position, MovePoint, OnPathFound));
            targetPosOld = MovePoint;
            IsCalculatingPath = true;
        }

        /// <summary>
        /// Function to prevent the agent from looking at the target's 0 Y position, but straight.
        /// </summary>
        /// <returns></returns>
        private void SetLookPointY(Vector3 lookPoint)
        {
            LookPoint = new Vector3(lookPoint.x, transform.position.y, lookPoint.z);
        }
        /// <summary>
        /// Function to follow a given path.
        /// </summary>
        /// <remarks>The path is an array of Vector3 objects</remarks>
        /// <returns></returns>
        private void FollowAStarPath()
        {
            SetLookPointY(_path.lookPoints[pathIndex]);
            MovePoint = _path.lookPoints[pathIndex];
            //MovePoint = _path.lookPoints[pathIndex];
            //transform.LookAt(new Vector3(_path.lookPoints[pathIndex].x, transform.position.y, _path.lookPoints[pathIndex].z));
            float speedPercent = 1;
            Vector2 pos2D = new Vector2(transform.position.x, transform.position.z);
            if (_path.turnBoundaries[pathIndex].HasCrossedLine(pos2D))
            {
                if (pathIndex == _path.finishLineIndex)
                {
                    IsFollowingPath = false;
                    //onDestinationReached(this);
                }
                else
                {
                    pathIndex++;
                    SetLookPointY(_path.lookPoints[pathIndex]);
                    MovePoint = _path.lookPoints[pathIndex];
                    //transform.LookAt(new Vector3(_path.lookPoints[pathIndex].x,transform.position.y, _path.lookPoints[pathIndex].z));
                }
            }
            if (IsFollowingPath)
            {
                if (pathIndex >= _path.slowDownIndex && _stoppingDist > 0)
                {
                    speedPercent = Mathf.Clamp01(_path.turnBoundaries[_path.finishLineIndex].DistanceFromPoint(pos2D) / _stoppingDist);
                    if (speedPercent < 0.01f)
                    {
                        IsFollowingPath = false;
                    }
                }
            }
        }

        public void OnPathFound(Vector3[] waypoints, bool pathSuccessful)
        {
            if (pathSuccessful)
            {
                _path = new Path(waypoints, transform.position, _turnDist, _stoppingDist);
                IsCalculatingPath = false;
                IsFollowingPath = true;
                pathIndex = 0;
            }
            else
            {
                //Debug.LogError("Failed to find path");
            }
        }

        public void AttackMelee()
        {
            if (hasAttackAnimation && !IsAttacking)
            {
                //Play attack animation which will call the DealDamage() function in the combat module
                IsAttacking = true;
            }
            else if (!IsAttacking)
            {
                IsAttacking = true;
                if (is3D)
                    StartCoroutine(combatModule.DealTimedDamage(AttackSpeed, AttackDamage, AttackRange, DamageAngle, enemyTags));                    
                else
                    StartCoroutine(combatModule.DealTimedDamage2D(AttackSpeed, AttackDamage, AttackRange, DamageAngle, enemyTags));
                

            }
        }
        public void AttackRanged()
        {
            if (hasAttackAnimation && !IsAttacking)
            {
                //Play attack animation which will call the FireProjectile() function
                IsAttacking = true;
            }
            else if (!IsAttacking)
            {
                IsAttacking = true;
                StartCoroutine(combatModule.FireProjectile(AttackTarget,Projectile,ProjectileSpawnPos,AttackSpeed));
            }
        }
        public void Wander()
        {
            /* Does nothing except pick a new destination to go to
             * Requires a CandiceGrid component attached to the Candice AI Manager instance.
             */
            if (wanderTarget == null)
            {
                wanderTarget = new GameObject("WanderTarget: " + AgentID);
                FindTarget(wanderTarget);
            }

            float distance = Vector3.Distance(wanderTarget.transform.position, transform.position);
            if (distance < 5f)
            {
                switchWanderTarget = true;
            }
            if (switchWanderTarget)
            {
                FindTarget(wanderTarget);

                mainTarget = wanderTarget;
                movePoint = mainTarget.transform.position;
                movePoint.y = 1;
                switchWanderTarget = false;
            }
            else
            {

            }

            // don't need to change direction every frame seeing as you walk in a straight line only
            //transform.LookAt(wayPoint);
            //Debug.Log(wayPoint + " and " + (transform.position - wayPoint).magnitude);
        }
        public void Flee()
        {

            Vector3 moveDirection = transform.position - MainTarget.transform.position;

            LookPoint = moveDirection;
            MovePoint = moveDirection;

        }
        private void FindTarget(GameObject target)
        {
            do
            {
                target.transform.position = new Vector3(UnityEngine.Random.Range(transform.position.x - DetectionRadius, transform.position.x + DetectionRadius), 1, UnityEngine.Random.Range(transform.position.z - DetectionRadius, transform.position.z + DetectionRadius));
            }
            while (!VerifyPoint(target.transform.position));
        }

        private bool VerifyPoint(Vector3 point)
        {
            //This method verifies if the chosen wander/flee point is within the game map and is on a walkable region
            bool isValid = false;
            CandiceGrid grid = candice.grid;
            Vector3 worldBottomLeft = grid.worldBottomLeft;

            if (point.x > worldBottomLeft.x && point.x < worldBottomLeft.x + grid.gridWorldSize.x)
            {
                if (point.z > worldBottomLeft.z && point.z < worldBottomLeft.z + grid.gridWorldSize.x)
                {
                    isValid = true;
                }
            }

            if (!candice.IsPointWalkable(point))
            {
                isValid = false;
            }


            return isValid;
        }
        public void CandiceReceiveDamage(float damage)
        {
            HitPoints = combatModule.ReceiveDamage(damage, HitPoints);
            //Animations Manager
            hit = true;
            atkDamage = damage;
            //End
            if(HitPoints <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            //if kill cam is attached
            if (attachKillCamera && !dead)
            {   
                //Kill Cam
                //if there be an eye in the sky to see you die, if you die
                if (CandiceAnimationManager.candiceCamera != null)
                {
                    CandiceAnimationManager.candiceCamera.target = transform;
                    CandiceAnimationManager.candiceCamera.KillCam();
                    StartCoroutine(CandiceAnimationManager.candiceCamera.FreeFly(1.5f));                    
                }
                //if kill cam, destroy has to happen after the freefly buffer, otherwise this gameObject is destroyed
                Destroy(this.gameObject, 3f);
                //we need to know if we're dead (for possessor)
                dead = true;
            }
            else {
                Destroy(this.gameObject, 0.5f);
                //we need to know if we're dead (for possessor)
                dead = true;
            }
        }

        public bool WithinAttackRange()
        {
            float distance = float.MaxValue;
            try
            {
                distance = Vector3.Distance(transform.position, AttackTarget.transform.position);
            }
            catch (Exception e)
            {
                Debug.LogError("DefaultBehaviors.WithinAttackRange: No target within attack range: " + e.Message);
            }
            if (distance <= AttackRange)
            {
                LookPoint = AttackTarget.transform.position;
                return true;
            }
            else
                return false;
        }

        void onObjectFound(CandiceDetectionResults results)
        {
            /*This is where you put your detection logic. 
             * The code below is only a sample to get you started.
             */
            AllyDetected = false;
            EnemyDetected = false;
            PlayerDetected = false;
            Enemies.Clear();
            Allies.Clear();
            Players.Clear();
            GameResources.Clear();
            foreach (string key in results.objects.Keys)
            {
                if(EnemyTags.Contains(key))
                {
                    EnemyDetected = true;
                    Enemies.AddRange(results.objects[key]);
                    MainTarget = Enemies[0];
                    MovePoint = MainTarget.transform.position;
                    LookPoint = MainTarget.transform.position;
                    AttackTarget = Enemies[0];
                }
                if (AllyTags.Contains(key))
                {
                    AllyDetected = true;
                    Allies.AddRange(results.objects[key]);
                }
                if (key == "Player")
                {
                    PlayerDetected = true;
                    Players.AddRange(results.objects[key]);
                    Player = Players[0];
                    
                }

                if (ResourceTags.Contains(key))
                {
                    ResourceDetected = true;
                    GameResources.AddRange(results.objects[key]);
                    MainTarget = GameResources[0];
                    MovePoint = MainTarget.transform.position;
                    LookPoint = MainTarget.transform.position;
                    ResourceTarget = GameResources[0];
                }
            }
        }
        void onAttackComplete(bool success)
        {
            IsAttacking = false;
        }

        public bool StoneDetected()
        {
            bool stoneDetected = false;
            foreach (GameObject resource in GameResources)
            {
                if (resource.tag.Equals("Stone"))
                {
                    stoneDetected = true;
                    resourceTarget = resource;
                }
            }

            return stoneDetected;
        }
        public bool PickaxeDetected()
        {
            bool pickaxeDetected = false;
            foreach (GameObject resource in GameResources)
            {
                if (resource.tag.Equals("Pickaxe"))
                {
                    pickaxeDetected = true;
                    resourceTarget = resource;
                }
            }

            return pickaxeDetected;
        }

        public void WaypointPatrol()
        {
            bool _pointReached = false;
            if (Vector3.Distance(transform.position, movePoint) < 3f)
            {
                _pointReached = true;
            }
            if (waypoint == null)
            {
                Debug.LogError("No waypoint assigned.");
                return;
            }
            if (_pointReached)
            {
                waypoint = waypoint.nextWaypoint;
                _pointReached = false;
            }
            mainTarget = waypoint.gameObject;
            movePoint = waypoint.GetPosition();
        }


        public void OnDrawGizmos()
        {
            if(candice != null && (candice.DrawAllAgentPaths || DrawAgentPath))
            {
                if (_path != null)
                {
                    _path.DrawWithGizmos();
                }

                if (_path != null)
                {
                    for (int i = 0; i < _path.lookPoints.Length; i++)
                    {
                        Gizmos.color = Color.white;
                        if (i != 0)
                        {
                            Gizmos.DrawLine(_path.lookPoints[i - 1], _path.lookPoints[i]);
                        }

                    }
                }
            }
            
        }

        public void OnCollisionEnter(Collision col)
        {
            IveHitSomething(col);
        }
    }
}
