//System
using System;
using System.Collections;
using System.Collections.Generic;
//Unity
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.Animations;
//Candice AI
using CandiceAIforGames.AI;

namespace CandiceAIforGames.AI
{
    public class CandiceAnimationManager : MonoBehaviour
    {

        //Some enemy type control, basic, advanced, titanic, boss        
        public bool isATitan = false;

        //Animations Module
        [HideInInspector]
        public CandiceModuleAnimations CandiceModuleAnimations;
        private CandiceStandardActions standardActions;
        private CandiceHumanoidMelee candiceHumanoidMelee;
        private CandicePlayerOverrides candicePlayerOverrides;
        [HideInInspector]
        public static CandiceCamera candiceCamera;
        [HideInInspector]
        public static GameObject mainCam;
        [HideInInspector]
        public static GameObject KillCam;
        public bool attachKillCam = false;
        //private bool killShot = false;
        public ScriptableObject shakeData;
        [HideInInspector]
        public CandiceUI candiceUI;

        //Candice Agent
        [HideInInspector]
        public Transform thisAgent;
        public float atkDamage;
        private bool isAttack = false;
        private bool isWalking = false;

        //Template AnimationController (we provide various out of the box animation controllers for humanoid and standard rigs, so you don't necessarily need to provide your own). 
        //This could be on the agent gameObject, a child gameObject of the agent or on any gameObject in the scene.
        public Animator TemplateAnimator;
        private Transform CandiceVSFX;
        private Rigidbody thisRigidbody;

        //Animation Speed Control
        //When set on character animSpeed overides globalSpeed, but when gloablSpeed > 1f then it is possible to slow down time, or generate other complex time manipulation animation scenarios.
        public float animSpeed = 1f;        
        public static float globalSpeed = 1f;
        private float thisGlobalSpeed = 1f;
        private float thisCharacterMoveSpeed;
        private float thisCharacterJumpSpeed;

        //Inventory manager
        private CandiceInventoryManager candiceInventoryManager;
        [HideInInspector]
        public GameObject inventoryDrop;

        //Healthbar if attached to UI layer (usually player healthbar)
        public GameObject HealthBar;
        public bool hit = false;
        public bool dead = false;

        //Combo Control
        [HideInInspector]
        private float timeSinceAttack = 0.0f;
        private int currentAttackInChainedCombo = 0;
        public float thisComboDamagePerHit = 0f;

        void Start() {
            mainCam = Camera.main.gameObject.transform.parent.gameObject;
            thisRigidbody = GetComponent<Rigidbody>();
            //When called in Start it allows for you to attach this script to an agent independently from the CandiceAIController
            //Some scenarios call for control this way. Scenarios such as cataclysms, massive destruction on cosmic scales etc.
            InitializeAnimations();
        }

        // Start is called before the first frame update
        public void InitializeAnimations()
        {

            /// 
            /// CANDICE ANIMATIONS
            /// 

            //Create a new instance of the Candice Animations module if not passed via public reference instance from CandiceAIController.
            if (CandiceModuleAnimations == null) {
                CandiceModuleAnimations = new CandiceModuleAnimations();
                //get standard actions
                standardActions = new CandiceStandardActions();
                //get melee actions
                candiceHumanoidMelee = new CandiceHumanoidMelee();
                //get player overrides
                candicePlayerOverrides = new CandicePlayerOverrides();
                
            }

            //Get Candice AI Agent being animated
            //Since CandiceAIController now inherits from this class, we can instantiate the agent transform here first.
            thisAgent = transform;
            CandiceVSFX = thisAgent.Find("VSFX");
            if (CandiceVSFX != null) {
                foreach (Transform vfx in CandiceVSFX)
                {
                    if (vfx.gameObject.tag == "CandiceVSFX")
                    {
                        vfx.gameObject.SetActive(false);
                    }
                }
            }

            //Get this Animator in case one is not provided as a template, has to be attached on this agent.
            //All standard CandiceAI prefabs contain an animator and animation controller.
            if (TemplateAnimator == null)
            {
                TemplateAnimator = thisAgent.GetComponent<Animator>();
            }

            //now assign animator
            CandiceModuleAnimations.TemplateAnimator = TemplateAnimator;
            standardActions.TemplateAnimator = TemplateAnimator;
            candiceHumanoidMelee.TemplateAnimator = TemplateAnimator;

            //animation speed control
            if (globalSpeed > 1f)
            {
                if (TemplateAnimator != null) {
                    SetSpeed(TemplateAnimator, "animSpeed", globalSpeed);
                }                
            }
            else
            {
                if (TemplateAnimator != null) {
                    SetSpeed(TemplateAnimator, "animSpeed", animSpeed);
                }
            }

            //CANDICE CAMERA
            if (candiceCamera == null)
            {   
                candiceCamera = new CandiceCamera();
            }
            //Set main camera      
            if (mainCam == null) {
                Transform camTransform = Camera.main.gameObject.transform;
                if(camTransform.parent != null)
                {
                    mainCam = camTransform.parent.gameObject;
                }
                else
                {
                    mainCam = camTransform.gameObject;
                }
                
            }
            candiceCamera.MainCamera = mainCam;

            //Set shake data for CandiceAI Tag objects
            if (thisAgent.gameObject.tag == "Player" || thisAgent.gameObject.tag == "CandiceAgent")
            {
                shakeData = thisAgent.GetComponent<CandiceAIController>().CameraShakeData;
                //add the shake data to candiceCamera
                candiceCamera.ShakeData = shakeData;
            }
            else {
                //add the shake data to candiceCamera
                candiceCamera.ShakeData = shakeData;
            }
            
            //add a killcam if checked
            if (attachKillCam) {
                //kill cam if any
                if (KillCam != null)
                {
                    KillCam.SetActive(false);
                    candiceCamera.KillCameraParent = KillCam;                    
                }
                else {
                    KillCam = GameObject.Find("KillCamera");
                    if (KillCam != null)
                    {
                        KillCam.SetActive(false);
                    }
                    candiceCamera.KillCameraParent = KillCam;
                }
            }

            //CANDICE INVENTORY
            //drop support
            if (candiceInventoryManager == null)
            {                
                candiceInventoryManager = gameObject.AddComponent(typeof(CandiceInventoryManager)) as CandiceInventoryManager;
                if (candiceInventoryManager != null && inventoryDrop != null)
                {
                    candiceInventoryManager.drop = inventoryDrop;
                }
            }
            else {
                candiceInventoryManager = gameObject.AddComponent(typeof(CandiceInventoryManager)) as CandiceInventoryManager;
                if (candiceInventoryManager != null && inventoryDrop != null)
                {
                    candiceInventoryManager.drop = inventoryDrop;
                }
            }

            //CANDICE UI
            if (candiceUI == null) {
                candiceUI = new CandiceUI();
            }
            //set the agent on the UI element
            candiceUI.thisAgent = thisAgent.gameObject;
            //if agent has no healthbar
            if (HealthBar == null) {
                //if agent is player
                if (thisAgent.gameObject.tag == "Player")
                {
                    HealthBar = GameObject.FindWithTag("PlayerHealthBar");
                }
                else {
                    HealthBar = GameObject.FindWithTag("AgentHealthBar");
                }
                
            }
            //assign Healthbar in candiceUI
            candiceUI.HealthBar = HealthBar;

            //grab the player controller speed variables
            if (thisAgent.gameObject.tag == "Player") {
                thisGlobalSpeed = globalSpeed;
                //we want to inference the player controller speeds with the animation speeds for advanced time effects
                //we can also control the player controller speeds this way for special animations like: shiftJump (also called blink or phaseShift), groundSmash and other high-level animations requiring special timing.
                CandiceAIPlayerController candicePlayerController = thisAgent.GetComponent<CandiceAIPlayerController>();
                if(candicePlayerController != null)
                {
                    thisCharacterMoveSpeed = candicePlayerController.speed;
                    thisCharacterJumpSpeed = candicePlayerController.jumpSpeed;
                }
                
            }

            //quick ui check
            //remove later
            GameObject rendp1 = GameObject.Find("Renderer P1");
            GameObject rendp2 = GameObject.Find("Renderer P2");
            if (rendp1 != null && rendp2 != null) {
                /*rendp1.GetComponent<LincolnCpp.HUDIndicator.IndicatorRenderer>().camera = Camera.main;
                rendp2.GetComponent<LincolnCpp.HUDIndicator.IndicatorRenderer>().camera = Camera.main;*/
            }

        }

        //Update is called once per frame
        public void Animate()
        {
            if (TemplateAnimator == null || TemplateAnimator.runtimeAnimatorController == null)
            {
                return;
            }
            //player animations
            if (gameObject.tag == "Player")
            {
                CandiceAIPlayerController candicePlayerController = thisAgent.GetComponent<CandiceAIPlayerController>();
                if (candicePlayerController != null)
                {
                    PlayerInput();
                }
            }
            //all other agents
            else {
                AgentInput();
            }
        }

        //Collision Control
        public bool IveHitSomething(Collision col)
        {
            //if colliding with agent projectiles
            if (col.gameObject.tag == "Projectile")
            {
                //ive been hit by a candice projectile
                hit = true;
                //we want some control over the attack damage of the projectile
                atkDamage = col.gameObject.transform.GetComponent<CandiceProjectile>().attackDamage;
            }
            else if (col.gameObject.tag == "Player")
            {
                //ive been hit by a candice projectile
                hit = true;
            }
            //return hit
            return hit;

        }

        //Support for PC & Standard Inputs defined in Edit > Project Settings > Input Manager currently        
        public bool StandardInputCall(string input) {
            if (Input.GetButton(input))
            {
                return true;
            }            
            return false;
        }

        //Generic Evaluate Input with multi input support //also supports while pressed, on key press down, and on keypress up
        public bool EvaluateInput(string input, bool isKey, bool down, bool up)
        {

            bool returnValue = false;

            //if key then just give it a key
            if (isKey)
            {
                if (down)
                {
                    returnValue = Input.GetKeyDown(input);
                }
                else if (up)
                {
                    returnValue = Input.GetKeyUp(input);
                }
                else
                {
                    returnValue = Input.GetKey(input);
                }

            }
            //otherwise uses unity input system
            else
            {
                if (down)
                {
                    returnValue = Input.GetButtonDown(input);
                }
                else if (up)
                {
                    returnValue = Input.GetButtonUp(input);
                }
                else
                {
                    returnValue = Input.GetButton(input);
                }
            }

            return returnValue;


        }

        //Support for new Unity Input System and Manager upcoming
        public bool InputManager2Call(string input) {
            return false;
        }

        //Handles all player input
        public void PlayerInput() {

            //player input animSpeed overrides global
            //animation speed control
            if (animSpeed > 1f)
            {
                //use speed function on thisAgent TemplateAnimator or set directly on globalSpeed when working with advanced timed effects.
                SetSpeed(TemplateAnimator, "animSpeed", animSpeed);
            }

            //temporary global movement speed for flash step
            float templGlobalSpeed = 3f;

            //get movement axes
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");

            //Primary fire                        
            //we have this set currently for light hand combo 1                        
            if (EvaluateInput("Fire1", false, true, false))            {
                timeSinceAttack += Time.deltaTime;
                currentAttackInChainedCombo++;
                if (timeSinceAttack > 0.1f)
                {
                    timeSinceAttack = 0.0f;
                    currentAttackInChainedCombo = 0;
                }
                isAttack = true;
                StartCoroutine(candiceHumanoidMelee.GenericCombo(currentAttackInChainedCombo, 0.33f));
                //Shake it baby!!!
                candiceCamera.ShakeData = shakeData;
                candiceCamera.CameraShake();
                verticalInput = 0f;
                horizontalInput = 0f;
            }
            //Throwing
            else if (EvaluateInput("Fire2", false, false, false))
            {
                candiceHumanoidMelee.Throw();
                candicePlayerOverrides.PATRAN_BOOSTED_CANDICEAI(thisAgent.GetComponent<CandiceAIController>());         
                verticalInput = 0f;
                horizontalInput = 0f;
            }
            //GroundSmash
            else if (EvaluateInput("Fire3", false, true, false)) {
                candiceHumanoidMelee.GroundSmash();
                //Shake it baby!!!
                candiceCamera.ShakeData = shakeData;
                candiceCamera.CameraShake();
            }
            //Jumping
            else if (EvaluateInput("Jump", false, false, false))
            {
                verticalInput = 0f;
                horizontalInput = 0f;
                standardActions.Jump();          
            }
            //axis based movement animation (walk, run, strafe) (run uses standard input call)
            else if (verticalInput > 0f && horizontalInput == 0f && !EvaluateInput("Run", false, false, false) && !EvaluateInput("Jump", false, false, false))
            {
                standardActions.Walk();
                standardActions.WalkForwards();
            }
            else if (verticalInput > 0f && horizontalInput < 0f && !EvaluateInput("Run", false, false, false) && !EvaluateInput("Jump", false, false, false))
            {
                standardActions.Walk();
                standardActions.WalkForwardsLeft();
            }
            else if (verticalInput == 0f && horizontalInput < 0f && !EvaluateInput("Run", false, false, false) && !EvaluateInput("Jump", false, false, false))
            {
                standardActions.Walk();
                standardActions.StrafeLeft();
            }
            else if (verticalInput > 0f && horizontalInput > 0f && !EvaluateInput("Run", false, false, false) && !EvaluateInput("Jump", false, false, false))
            {
                standardActions.Walk();
                standardActions.WalkForwardsRight();
            }
            else if (verticalInput == 0f && horizontalInput > 0f && !EvaluateInput("Run", false, false, false) && !EvaluateInput("Jump", false, false, false))
            {
                standardActions.Walk();
                standardActions.StrafeRight();
            }
            else if (verticalInput < 0f && horizontalInput == 0f && !EvaluateInput("Run", false, false, false) && !EvaluateInput("Jump", false, false, false))
            {
                standardActions.Walk();
                standardActions.WalkBackwards();
            }
            else if (verticalInput < 0f && horizontalInput < 0f && !EvaluateInput("Run", false, false, false) && !EvaluateInput("Jump", false, false, false))
            {
                standardActions.Walk();
                standardActions.WalkBackwardsLeft();
            }
            else if (verticalInput < 0f && horizontalInput > 0f && !EvaluateInput("Run", false, false, false) && !EvaluateInput("Jump", false, false, false))
            {
                standardActions.Walk();
                standardActions.WalkBackwardsRight();
            }
            else if (verticalInput == 0f && horizontalInput == 0f && !EvaluateInput("Run", false, false, false) && !EvaluateInput("Jump", false, false, false))
            {
                //when standing still, reset movement
                standardActions.Idle();
                globalSpeed = thisGlobalSpeed;
                SetSpeed(TemplateAnimator, "animSpeed", globalSpeed);
                thisAgent.GetComponent<CandiceAIPlayerController>().speed = thisCharacterMoveSpeed;
                thisAgent.GetComponent<CandiceAIPlayerController>().jumpSpeed = thisCharacterJumpSpeed;
            }

            //Running
            else if (verticalInput > 0f && horizontalInput == 0 && EvaluateInput("Run", false, false, false))
            {
                standardActions.RunForwards();
                //flash step
                globalSpeed = templGlobalSpeed;
                SetSpeed(TemplateAnimator, "animSpeed", globalSpeed);
                thisAgent.GetComponent<CandiceAIPlayerController>().speed += (templGlobalSpeed * Time.deltaTime);
                thisAgent.GetComponent<CandiceAIPlayerController>().jumpSpeed += (templGlobalSpeed * Time.deltaTime);
                //Shake it baby!!!
                candiceCamera.ShakeData = shakeData;
                candiceCamera.CameraShake();

            }
            else if (verticalInput > 0f && horizontalInput < 0f && EvaluateInput("Run", false, false, false))
            {
                standardActions.RunForwardsLeft();
                //flash step
                globalSpeed = templGlobalSpeed;
                SetSpeed(TemplateAnimator, "animSpeed", globalSpeed);
                thisAgent.GetComponent<CandiceAIPlayerController>().speed += (templGlobalSpeed * Time.deltaTime);
                thisAgent.GetComponent<CandiceAIPlayerController>().jumpSpeed += (templGlobalSpeed * Time.deltaTime);
                //Shake it baby!!!
                candiceCamera.ShakeData = shakeData;
                candiceCamera.CameraShake();

            }
            else if (verticalInput > 0f && horizontalInput > 0f && EvaluateInput("Run", false, false, false))
            {
                standardActions.RunForwardsRight();
                //flash step
                globalSpeed = templGlobalSpeed;
                SetSpeed(TemplateAnimator, "animSpeed", globalSpeed);
                thisAgent.GetComponent<CandiceAIPlayerController>().speed += (templGlobalSpeed * Time.deltaTime);
                thisAgent.GetComponent<CandiceAIPlayerController>().jumpSpeed += (templGlobalSpeed * Time.deltaTime);
                //Shake it baby!!!
                candiceCamera.ShakeData = shakeData;
                candiceCamera.CameraShake();
            }

            //Health & UI
            if (CheckHealth(1f) && hit) { standardActions.Hurt(); candiceUI.UpdateHealthUI("ClassicProgressBar", atkDamage); hit = false; }
            else if (dead) { standardActions.Death(); candiceInventoryManager.drop = inventoryDrop; candiceInventoryManager.Drop(thisAgent); }

            //Handicaps
            if (Handicaper.SelectedHandicap == "Bleed")
            {
                thisAgent.GetComponent<CandiceAIController>().CandiceReceiveDamage(5f * Time.deltaTime);
                candiceUI.UpdateHealthUI("ClassicProgressBar", 5f * Time.deltaTime);
            }

            //VSFX
            if (CandiceVSFX != null) {
                //when in movement
                if (horizontalInput != 0f || verticalInput != 0f)
                {
                    //special considerations
                    foreach (Transform vfx in CandiceVSFX)
                    {
                        if (isAttack && vfx.gameObject.tag == "CandiceVSFX" && vfx.gameObject.name == "Attack")
                        {
                            GameObject attack = Instantiate(vfx.gameObject, vfx.position, Quaternion.identity);
                            attack.SetActive(true);
                            Destroy(attack, 1f);
                            isAttack = false;
                        }
                        if (hit && vfx.gameObject.tag == "CandiceVSFX" && vfx.gameObject.name == "Hurt")
                        {
                            GameObject hurt = Instantiate(vfx.gameObject, vfx.position, Quaternion.identity);
                            hurt.SetActive(true);
                            Destroy(hurt, 5f);
                            hit = false;
                        }
                        else if (dead && vfx.gameObject.tag == "CandiceVSFX" && vfx.gameObject.name == "Death")
                        {
                            GameObject death = Instantiate(vfx.gameObject, transform.position, Quaternion.identity);
                            death.SetActive(true);
                            candiceCamera.CameraShake();
                            Destroy(death, 5f);
                            dead = false;

                        }
                        else if (vfx.gameObject.tag == "CandiceVSFX" && vfx.gameObject.name == "Footsteps")
                        {
                            GameObject footsteps = Instantiate(vfx.gameObject, transform.position, Quaternion.identity);
                            footsteps.SetActive(true);
                            Destroy(footsteps, 1f);
                        }
                        else if (vfx.gameObject.tag == "CandiceVSFX" && vfx.gameObject.name == "PowerAura")
                        {
                            vfx.gameObject.SetActive(true);
                        }
                        else if (vfx.gameObject.tag == "CandiceVSFX" && vfx.gameObject.name == "Run" && EvaluateInput("Run", false, false, false)) {
                            GameObject run = Instantiate(vfx.gameObject, transform.position, Quaternion.identity);
                            run.SetActive(true);
                            Destroy(run, 0.33f);
                            globalSpeed = thisGlobalSpeed;
                            SetSpeed(TemplateAnimator, "animSpeed", thisGlobalSpeed);
                        }
                        else if (vfx.gameObject.tag == "CandiceVSFX" && vfx.gameObject.name == "GroundSmash" && EvaluateInput("Fire3", false, true, false))
                        {
                            GameObject groundSmash = Instantiate(vfx.gameObject, transform.position, Quaternion.identity);
                            groundSmash.SetActive(true);
                            Destroy(groundSmash, 1f);
                        }

                    }
                }
                //when idle, show no sfx except idle animation
                //can be tweaked
                else
                {
                    foreach (Transform vfx in CandiceVSFX)
                    {
                        if (isAttack && vfx.gameObject.tag == "CandiceVSFX" && vfx.gameObject.name == "Attack")
                        {
                            GameObject attack = Instantiate(vfx.gameObject, vfx.position, Quaternion.identity);
                            attack.SetActive(true);
                            Destroy(attack, 1f);
                            isAttack = false;
                        }
                        else if (hit && vfx.gameObject.tag == "CandiceVSFX" && vfx.gameObject.name == "Hurt")
                        {
                            GameObject hurt = Instantiate(vfx.gameObject, vfx.position, Quaternion.identity);
                            hurt.SetActive(true);
                            Destroy(hurt, 5f);
                            hit = false;
                        }
                        else if (dead && vfx.gameObject.tag == "CandiceVSFX" && vfx.gameObject.name == "Death")
                        {
                            GameObject death = Instantiate(vfx.gameObject, transform.position, Quaternion.identity);
                            death.SetActive(true);
                            candiceCamera.CameraShake();
                            //Destroy(death, 5f);
                            dead = false;
                        }
                        else if (vfx.gameObject.tag == "CandiceVSFX" && vfx.gameObject.name == "GroundSmash" && EvaluateInput("Fire3", false, true, false))
                        {
                            GameObject groundSmash = Instantiate(vfx.gameObject, transform.position, Quaternion.identity);
                            groundSmash.SetActive(true);
                            Destroy(groundSmash, 1f);
                        }
                        else if (vfx.gameObject.tag == "CandiceVSFX")
                        {
                            vfx.gameObject.SetActive(false);
                        }
                    }
                }
            }

            //IF IS TITANIC
            if (isATitan) {
                if (horizontalInput != 0f || verticalInput != 0f) {
                    candiceCamera.ShakeData = shakeData;
                    candiceCamera.CameraShake();
                }
            }

        }

        //Handles all AI input
        public void AgentInput() {
            
            isAttack = thisAgent.GetComponent<CandiceAIController>().isAttacking;

            //when in motion or not
            if (thisRigidbody == null) {
                thisRigidbody = thisAgent.GetComponent<Rigidbody>();
                if (thisRigidbody != null) {
                    //walk and no attack
                    if (thisRigidbody.velocity.magnitude != 0f && !isAttack)
                    {
                        isWalking = true;
                        standardActions.Walk();
                        if (isATitan)
                        {
                            candiceCamera.ShakeData = shakeData;
                            candiceCamera.CameraShake();
                        }
                    }
                    //walk and attack
                    else if ((thisRigidbody.velocity.magnitude != 0f && isAttack) || (thisRigidbody.velocity.magnitude == 0f && isAttack)) {
                        isWalking = false;
                        standardActions.Attack();
                        candiceCamera.ShakeData = shakeData;
                        candiceCamera.CameraShake();
                    }
                    //all other
                    else
                    {
                        isWalking = false;
                        standardActions.Idle();
                    }
                }
            }
            else {
                //walk and no attack
                if (thisRigidbody.velocity.magnitude != 0f && !isAttack)
                {
                    isWalking = true;
                    standardActions.Walk();
                    if (isATitan)
                    {
                        candiceCamera.ShakeData = shakeData;
                        candiceCamera.CameraShake();
                    }
                }
                //walk and attack
                else if ((thisRigidbody.velocity.magnitude != 0f && isAttack) || (thisRigidbody.velocity.magnitude == 0f && isAttack))
                {
                    isWalking = false;
                    standardActions.Attack();
                    candiceCamera.ShakeData = shakeData;
                    candiceCamera.CameraShake();
                }
                //all other
                else
                {
                    isWalking = false;
                    standardActions.Idle();
                }
            }

            //if you collide and you're not dead
            //ui support to come (in case you want to display enemy health bars in 3D and not in the UI layer.
            if (CheckHealth(1f) && hit) { standardActions.Hurt(); hit = false; }
            else if (dead) { standardActions.Death(); candiceInventoryManager.drop = inventoryDrop; candiceInventoryManager.Drop(thisAgent);} 

            //VFX
            if (CandiceVSFX != null)
            {
                //Special Considerations
                foreach (Transform vfx in CandiceVSFX)
                {
                    if (hit && vfx.gameObject.tag == "CandiceVSFX" && vfx.gameObject.name == "Hurt")
                    {
                        GameObject hurt = Instantiate(vfx.gameObject, transform.position, Quaternion.identity);
                        hurt.SetActive(true);
                        Destroy(hurt, 5f);
                        hit = false;
                    }
                    else if (dead && vfx.gameObject.tag == "CandiceVSFX" && vfx.gameObject.name == "Death")
                    {
                        GameObject death = Instantiate(vfx.gameObject, transform.position, Quaternion.identity);
                        death.SetActive(true);
                        candiceCamera.CameraShake();
                        Destroy(death, 5f);
                        dead = false;
                    }
                    else if (isWalking && vfx.gameObject.tag == "CandiceVSFX" && vfx.gameObject.name == "Footsteps") {
                        GameObject footsteps = Instantiate(vfx.gameObject, transform.position, Quaternion.identity);
                        footsteps.SetActive(true);
                        candiceCamera.CameraShake();
                        Destroy(footsteps, 1f);
                        isWalking = false;
                    }
                }
            }

        }

        //Used to set animation speed (local or global)
        private void SetSpeed(Animator animator, string name, float animSpeed) {
            animator.SetFloat(name, animSpeed);
        }

        //Used to assess all agent types health
        private bool CheckHealth(float minHealthToTrigger) {            
            if (thisAgent.GetComponent<CandiceAIController>().hitPoints >= minHealthToTrigger)
            {
                return true;
            }
            else {
                return false;
            }
        }

        private IEnumerator TimedDelay(float waitTime) { 
            yield return new WaitForSeconds(waitTime);
        }

        //OnTriggerEnter ensures this script can also be attached to a gameObject for more direct control of some variables, while fully invokable by type and serialization in CandiceAIController
        //Currently used to add impact vsfx to environment and other objects
        void OnTriggerEnter(Collider collider) {
            if (collider.gameObject.tag == "Projectile")
            {
                if (CandiceVSFX != null)
                {
                    foreach (Transform vfx in CandiceVSFX)
                    {

                        //environment
                        if (vfx.gameObject.tag == "CandiceVSFX" && vfx.gameObject.name == "EnviroImpacts")
                        {
                            GameObject enviroImpact = Instantiate(vfx.gameObject, collider.transform.position, Quaternion.identity);
                            enviroImpact.SetActive(true);
                            //Shake it baby!!!
                            candiceCamera.ShakeData = shakeData;
                            candiceCamera.CameraShake();
                            Destroy(enviroImpact, 5f);
                            if (LayerMask.LayerToName(transform.gameObject.layer) == "Obstacle") {
                                Destroy(transform.gameObject);
                            }
                        }
                    }
                }
                else
                {
                    //shake on any other CandiceVSFX activation during projectile collisions
                    if (thisAgent.gameObject.tag == "CandiceVSFX")
                    {
                        candiceCamera.ShakeData = shakeData;
                        candiceCamera.CameraShake();
                    }
                }

            }
            else if (collider.gameObject.tag == "Enemy") {                
                if (collider.gameObject.GetComponent<CandiceAIController>().hitPoints > 0.01f) {
                    collider.gameObject.GetComponent<CandiceAIController>().IsAttacking = true;
                    collider.gameObject.GetComponent<CandiceAIController>().CandiceReceiveDamage(thisComboDamagePerHit);
                }
            }
        }


    }
}