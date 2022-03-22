using UnityEngine;


namespace Invector.vCharacterController
{
    using IK;
    using vShooter;
    [vClassHeader("SHOOTER/MELEE INPUT", iconName = "inputIcon")]
    public class vShooterMeleeInput : vMeleeCombatInput, vIShooterIKController, PlayerController.vILockCamera
    {
        #region Shooter Inputs

        [vEditorToolbar("Inputs")]
        [Header("Shooter Inputs")]
        public GenericInput aimInput = new GenericInput("Mouse1", false, "LT", true, "LT", false);
        public GenericInput shotInput = new GenericInput("Mouse0", false, "RT", true, "RT", false);
        public GenericInput reloadInput = new GenericInput("R", "LB", "LB");
        public GenericInput switchCameraSideInput = new GenericInput("Tab", "RightStickClick", "RightStickClick");
        public GenericInput scopeViewInput = new GenericInput("Z", "RB", "RB");

        #endregion

        #region Shooter Variables       

        [HideInInspector] public vShooterManager shooterManager;

        internal bool isAimingByInput;
        internal bool isReloading;
        internal bool defaultStrafeWalk;
        internal Transform leftHand, rightHand, rightLowerArm, leftLowerArm, rightUpperArm, leftUpperArm;
        internal bool aimConditions;
     
        internal bool ignoreIK = false;
        protected int onlyArmsLayer;
        protected int shotLayer;
        protected int shootCountA;

        protected bool allowAttack;
      
        protected bool isUsingScopeView;
        protected bool isCameraRightSwitched;
        protected float onlyArmsLayerWeight;
        protected float supportIKWeight, weaponIKWeight;

        protected float armAlignmentWeight;
        protected float aimWeight;

        protected float lastAimDistance;
        protected Quaternion handRotation, upperArmRotation;

        protected vHeadTrack headTrack;

        protected bool lastRotateWithCamera;
        protected vControlAimCanvas _controlAimCanvas;      
        protected GameObject aimAngleReference;      
        protected Vector3 ikRotationOffset;
        protected Vector3 ikPositionOffset;
        protected bool checkCanAimInit;
        protected GameObject lastShooterWeapon;
        protected Quaternion upperArmRotationAlignment, handRotationAlignment;       

        protected bool _ignoreIKFromAnimator;
        protected bool _walkingByDefaultWasChanged;
        protected float _aimTimming;
      
        protected float checkCanAimOffsetStartX;
        protected float checkCanAimOffsetStartY;
        protected float checkCanAimOffsetEndX;
        protected float checkCanAimOffsetEndY;
        protected float checkCanAimHeight;
        protected float scopeDirectionWeight;
        protected RaycastHit checkCanAimHit;
        protected Vector3 aimHitPoint;
        protected Vector3 upperArmPosition;
        protected Vector3 muzzlePosition;
        protected Vector3 muzzleForward;
        protected IKAdjust _currentIKAdjust;

        #region IKController Interface Properties

        public virtual vIKSolver LeftIK { get; set; }

        public virtual vIKSolver RightIK { get; set; }

        public virtual vWeaponIKAdjustList WeaponIKAdjustList
        {
            get
            {
                if (shooterManager)
                {
                    return shooterManager.weaponIKAdjustList;
                }


                return null;
            }
            set
            {
                if (shooterManager)
                {
                    shooterManager.weaponIKAdjustList = value;
                }
            }
        }

        public virtual vWeaponIKAdjust CurrentWeaponIK
        {
            get
            {
                if (shooterManager)
                {
                    return shooterManager.CurrentWeaponIK;
                }


                return null;
            }
        }

        public virtual IKAdjust CurrentIKAdjust
        {
            get
            {
                if (CurrentWeaponIK == null) return null;
                if (CurrentIKAdjustStateWithTag != (IKWeaponTag + TargetIKAdjustState) ||_currentIKAdjust==null)
                {
                    CurrentIKAdjustStateWithTag = (IKWeaponTag + TargetIKAdjustState);
                    CurrentIKAdjustState = TargetIKAdjustState;                   
                    _currentIKAdjust = CurrentWeaponIK.GetIKAdjust(CurrentIKAdjustState, CurrentActiveWeapon.isLeftWeapon);
                    
                }
                return _currentIKAdjust;
            }
        }

        public virtual bool EditingIKGlobalOffset
        {
            get;set;
        }

        public virtual string DefaultIKAdjustState =>CurrentWeaponIK?  CurrentWeaponIK.GetDefaultStateName(this):string.Empty;

        protected virtual string TargetIKAdjustState => (!IsUsingCustomIKAdjust ? DefaultIKAdjustState : CustomIKAdjustState);

        protected virtual string IKWeaponTag => CurrentActiveWeapon ? CurrentActiveWeapon.weaponCategory + "@":"";

        public virtual string CurrentIKAdjustStateWithTag { get; set; }

        public virtual string CurrentIKAdjustState { get; protected set; }

        public virtual bool IsUsingCustomIKAdjust => !string.IsNullOrEmpty(CustomIKAdjustState);

        public string CustomIKAdjustState { get; protected set; }

        public virtual void SetCustomIKAdjustState(string value)
        {
            if (!string.IsNullOrEmpty(value)) CustomIKAdjustState = value;
        }

        public virtual void ResetCustomIKAdjustState()
        {
            if (!string.IsNullOrEmpty(CustomIKAdjustState)) CustomIKAdjustState = string.Empty;
        }

        public virtual bool IsIgnoreIK
        {
            get
            {
                return ignoreIK || _ignoreIKFromAnimator;
            }
        }

        public virtual bool IsSupportHandIKEnabled
        {
            get;protected set;
            
        }

        public virtual void UpdateWeaponIK()
        {
            if (shooterManager)
            {
                shooterManager.UpdateWeaponIK();
                if (CurrentWeaponIK == null) return;
               _currentIKAdjust = CurrentWeaponIK.GetIKAdjust(CurrentIKAdjustState, CurrentActiveWeapon.isLeftWeapon);
            }
        }

        public event IKUpdateEvent onStartUpdateIK;

        public event IKUpdateEvent onFinishUpdateIK;

        public virtual Vector3 AimPosition { get; protected set; }

        public virtual bool LockAiming
        {
            get
            {
                return shooterManager && shooterManager.alwaysAiming;
            }
            set
            {
                shooterManager.alwaysAiming = value;
            }
        }

        public virtual bool LockHipFireAiming
        {
            get;set;
        }

        public virtual bool IsCrouching
        {
            get
            {
                return cc.isCrouching;
            }
            set
            {
                cc.isCrouching = value;
            }
        }

        public virtual bool IsLeftWeapon
        {
            get
            {

                return shooterManager && shooterManager.IsLeftWeapon;
            }
        }

        public virtual bool LockCamera
        {
            get
            {
                return tpCamera && tpCamera.LockCamera;
            }
            set
            {
                if (tpCamera)
                {
                    tpCamera.LockCamera = value;
                }
            }
        }
        #endregion       
        
        /// <summary>
        /// Is Aiming by <see cref="isAimingByInput"/> or <see cref="isAimingByHipFire"/>
        /// </summary>
        public virtual bool IsAiming
        {
            get
            {
                return (!cc.isRolling) && (isAimingByInput || isAimingByHipFire);
            }
        }

        public virtual bool isAimingByHipFire { get { if (!shooterManager.hipfireShot && _aimTimming > 0 || (isReloading&& !shooterManager.keepAimingWhenReload) || isEquipping) { _aimTimming = 0; return false; }  return shooterManager.hipfireShot && ((_aimTimming > 0||(shotInput.GetButton() && shooterManager.CurrentWeapon != null)) || (!isAimingByInput && shootCountA > 0)); } }

        public virtual vControlAimCanvas controlAimCanvas
        {
            get
            {
                if (!_controlAimCanvas)
                {
                    _controlAimCanvas = FindObjectOfType<vControlAimCanvas>();
                    if (_controlAimCanvas)
                    {
                        _controlAimCanvas.Init(cc);
                    }
                }

                return _controlAimCanvas;
            }
        }

        internal bool lockShooterInput;

        public override bool lockInventory
        {
            get
            {
                return base.lockInventory || isReloading || cc.customAction || cc.isRolling;
            }
        }

        #endregion

        protected override void Start()
        {
            shooterManager = GetComponent<vShooterManager>();

            base.Start();
            checkCanAimHeight = cc._capsuleCollider.height;
            leftHand = animator.GetBoneTransform(HumanBodyBones.LeftHand);
            rightHand = animator.GetBoneTransform(HumanBodyBones.RightHand);
            leftLowerArm = animator.GetBoneTransform(HumanBodyBones.LeftLowerArm);
            rightLowerArm = animator.GetBoneTransform(HumanBodyBones.RightLowerArm);
            leftUpperArm = animator.GetBoneTransform(HumanBodyBones.LeftUpperArm);
            rightUpperArm = animator.GetBoneTransform(HumanBodyBones.RightUpperArm);

            onlyArmsLayer = animator.GetLayerIndex("OnlyArms");
            shotLayer = animator.GetLayerIndex("Shot");
            aimAngleReference = new GameObject("aimAngleReference");           
            aimAngleReference.tag = ("Ignore Ragdoll");           
            aimAngleReference.transform.rotation = transform.rotation;
            var aimAngleParent = animator.GetBoneTransform(HumanBodyBones.Head);
            aimAngleReference.transform.SetParent(aimAngleParent);           
            aimAngleReference.transform.localPosition = Vector3.zero;         
            defaultStrafeWalk = cc.strafeSpeed.walkByDefault;
            headTrack = GetComponent<vHeadTrack>(); 
            lastRotateWithCamera = cc.strafeSpeed.rotateWithCamera;
            if(headTrack)
            {
                  headTrack.onInitUpdate.AddListener(UpdateAimAngleReference);
            }
            if (!controlAimCanvas)
            {
                Debug.LogWarning("Missing the AimCanvas, drag and drop the prefab to this scene in order to Aim", gameObject);
            }
        }

        protected override void LateUpdate()
        {
            if ((!updateIK && animator.updateMode == AnimatorUpdateMode.AnimatePhysics))
            {
                return;
            }

            base.LateUpdate();
            UpdateAimBehaviour();
        }

        #region Shooter Inputs    

        protected virtual void Reset()
        {
            // We change the Melee Attack Input for the Shooter because 'Mouse1' is the same input to Shot a Fire Weapon
            weakAttackInput = new GenericInput("Mouse2", "RB", "RB");
            // By default it's disable because it uses the same input as the switchCameraSideInput
            strafeInput.useInput = false;
        }

        /// <summary>
        /// Lock only shooter inputs
        /// </summary>
        /// <param name="value">lock or unlock</param>
        public virtual void SetLockShooterInput(bool value)
        {
            lockShooterInput = value;

            if (value)
            {
                isBlocking = false;
                isAimingByInput = false;
                _aimTimming = 0f;
                if (controlAimCanvas)
                {
                    controlAimCanvas.SetActiveAim(false);
                    controlAimCanvas.SetActiveScopeCamera(false);

                }
            }
        }

        public override void SetLockAllInput(bool value)
        {
            base.SetLockAllInput(value);
            SetLockShooterInput(value);
        }

        /// <summary>
        /// Set Always Aiming
        /// </summary>
        /// <param name="value">value to set aiming</param>
        public virtual void SetAlwaysAim(bool value)
        {
            shooterManager.alwaysAiming = value;
        }

        /// <summary>
        /// Current active weapon (if weapon gameobject is disabled this return null)
        /// </summary>
        public virtual vShooterWeapon CurrentActiveWeapon
        {
            get
            {
                return shooterManager.CurrentWeapon && shooterManager.IsCurrentWeaponActive() ? shooterManager.CurrentWeapon : null;
            }
        }

        /// <summary>
        /// Handles all the Controller Input 
        /// </summary>
        protected override void InputHandle()
        {
            if (cc == null || cc.isDead)
            {
                AimInput();
                return;
            }

            #region BasicInput

            if (!cc.ragdolled && !lockInput)
            {
                MoveInput();
                SprintInput();
                CrouchInput();
                StrafeInput();
                JumpInput();
                RollInput();
            }

            #endregion

            #region MeleeInput

            if (MeleeAttackConditions() && !IsAiming && !isReloading && !lockMeleeInput && !CurrentActiveWeapon)
            {
                if (shooterManager.canUseMeleeWeakAttack_H || shooterManager.CurrentWeapon == null)
                {
                    MeleeWeakAttackInput();
                }

                if (shooterManager.canUseMeleeStrongAttack_H || shooterManager.CurrentWeapon == null)
                {
                    MeleeStrongAttackInput();
                }

                if (shooterManager.canUseMeleeBlock_H || shooterManager.CurrentWeapon == null)
                {
                    BlockingInput();
                }
                else
                {
                    isBlocking = false;
                }
            }

            #endregion

            #region ShooterInput

            if (lockShooterInput)
            {
                isAimingByInput = false;
                _aimTimming = 0;
                if (controlAimCanvas != null)
                {
                    if (controlAimCanvas.isAimActive)
                    {
                        controlAimCanvas.SetActiveAim(false);
                    }
                    if (controlAimCanvas.isScopeCameraActive)
                    {
                        controlAimCanvas.SetActiveScopeCamera(false);
                    }
                }
            }
            else if (shooterManager.CurrentWeapon)
            {
                if (MeleeAttackConditions() && (!IsAiming || shooterManager.canUseMeleeAiming))
                {
                    if (shooterManager.canUseMeleeWeakAttack_E)
                    {
                        MeleeWeakAttackInput();
                    }
                    if (shooterManager.canUseMeleeStrongAttack_E)
                    {
                        MeleeStrongAttackInput();
                    }
                    if (shooterManager.canUseMeleeBlock_E)
                    {
                        BlockingInput();
                    }
                    else
                    {
                        isBlocking = false;
                    }
                }
                else
                {
                    isBlocking = false;
                }

                if (shooterManager == null || CurrentActiveWeapon == null || isEquipping)
                {
                    if (IsAiming)
                    {
                        isAimingByInput = false;
                        _aimTimming = 0;
                        if (!cc.lockInStrafe && cc.isStrafing)
                        {
                            cc.Strafe();
                          
                        }

                        if (controlAimCanvas != null)
                        {
                            controlAimCanvas.SetActiveAim(false);
                            controlAimCanvas.SetActiveScopeCamera(false);
                        }
                        if (shooterManager && shooterManager.CurrentWeapon && shooterManager.CurrentWeapon.chargeWeapon && shooterManager.CurrentWeapon.powerCharge != 0)
                        {
                            CurrentActiveWeapon.powerCharge = 0;
                        }

                        shootCountA = 0;
                    }
                }
                else
                {
                    AimInput();
                    ShotInput();
                    ReloadInput();
                    SwitchCameraSideInput();
                    ScopeViewInput();
                }
            }
            else
            {
                isAimingByInput = false;
                _aimTimming = 0;
                if (controlAimCanvas != null)
                {
                    if (controlAimCanvas.isAimActive)
                    {
                        controlAimCanvas.SetActiveAim(false);
                    }
                    if (controlAimCanvas.isScopeCameraActive)
                    {
                        controlAimCanvas.SetActiveScopeCamera(false);
                    }
                }
            }
            #endregion
        }

        /// <summary>
        /// Override the Melee TriggerStrongAttack method to add the call to CancelReload when attacking
        /// </summary>
        public override void TriggerStrongAttack()
        {
            shooterManager.CancelReload();
            base.TriggerStrongAttack();
        }       

        /// <summary>
        /// Control Aim Input
        /// </summary>
        public virtual void AimInput()
        {
            //Change Rotation Method While Aiming 
            cc.strafeSpeed.rotateWithCamera = IsAiming ? true : lastRotateWithCamera;
            if(_walkingByDefaultWasChanged && !IsAiming)
            {
                _walkingByDefaultWasChanged = false;
                SetWalkByDefault(defaultStrafeWalk);
            }
            if (!shooterManager || isAttacking || (isReloading && ( isUsingScopeView || !shooterManager.keepAimingWhenReload)))
            {
                if (!isReloading || (isReloading && !shooterManager.keepAimingWhenReload))
                {
                    isAimingByInput = false;

                    SetWalkByDefault(defaultStrafeWalk);
                    _walkingByDefaultWasChanged = false;
                    if (cc.isStrafing)
                    {
                        cc.Strafe();
                    }
                }
                if (controlAimCanvas)
                {
                    controlAimCanvas.SetActiveAim(false);
                    if(isUsingScopeView)DisableScopeView();

                }              

                return;
            }


            if (LockHipFireAiming) _aimTimming = 1f;
          
            if (shooterManager.onlyWalkWhenAiming  && (!isReloading || shooterManager.keepAimingWhenReload))
            {
                SetWalkByDefault(isAimingByInput? true:defaultStrafeWalk);
                _walkingByDefaultWasChanged = isAimingByInput ? true : defaultStrafeWalk;
            }

            if (cc.locomotionType == vThirdPersonMotor.LocomotionType.OnlyFree)
            {
                Debug.LogWarning("Shooter behaviour needs to be OnlyStrafe or Free with Strafe. \n Please change the Locomotion Type.");
                return;
            }

            if (shooterManager.hipfireShot && !LockHipFireAiming)
            {
                // countdown for the hipfire to reset the aim back to idle
                if (_aimTimming > 0 && (!shooterManager.isShooting && !shooterManager.isShootingEmptyClip) && CanRotateAimArm())
                {
                    _aimTimming -= Time.deltaTime;
                }

                // reset the aimTimming if you sprint while still aiming through the hipfire
                if (sprintInput.GetButtonDown() && _aimTimming > 0f)
                {
                    _aimTimming = 0f;
                }
            }

            if (!shooterManager || !CurrentActiveWeapon)
            {
                if (controlAimCanvas)
                {
                    controlAimCanvas.SetActiveAim(false);
                    controlAimCanvas.SetActiveScopeCamera(false);
                }
                isAimingByInput = false;
                if (cc.isStrafing)
                {
                    cc.Strafe();
                }
                return;
            }

            if (!cc.isRolling)
            {
                isAimingByInput = (!isReloading || shooterManager.keepAimingWhenReload) && (aimInput.GetButton() || (shooterManager.alwaysAiming && CurrentActiveWeapon)) && !cc.ragdolled && !cc.customAction
                    || (cc.customAction && cc.isJumping);
            }

            if (aimInput.GetButtonUp() && !shotInput.GetButton())
            {
                _aimTimming = 0f;
            }
            if (headTrack)
            {
                headTrack.alwaysFollowCamera = isAimingByInput;
            }

            if (cc.locomotionType == vThirdPersonMotor.LocomotionType.FreeWithStrafe)
            {                
                if(!cc.lockInStrafe)
                {
                    if (IsAiming && !cc.isStrafing)
                    {
                        cc.Strafe();
                    }
                    else if (!IsAiming && cc.isStrafing)
                    {                       
                        cc.Strafe();
                    }
                }
              
            }
            if (IsAiming && shooterManager.onlyWalkWhenAiming && cc.isSprinting)
            {
                cc.isSprinting = false;
            }

            if (controlAimCanvas)
            {
                if (IsAiming && !controlAimCanvas.isAimActive)
                {
                    controlAimCanvas.SetActiveAim(true);
                }

                if (!IsAiming && controlAimCanvas.isAimActive)
                {
                    controlAimCanvas.SetActiveAim(false);
                }
            }
            if (shooterManager.rWeapon)
            {
                shooterManager.rWeapon.SetActiveAim(IsAiming && aimConditions);
                shooterManager.rWeapon.SetActiveScope(IsAiming && isUsingScopeView);
            }
            else if (shooterManager.lWeapon)
            {
                shooterManager.lWeapon.SetActiveAim(IsAiming && aimConditions);
                shooterManager.lWeapon.SetActiveScope(IsAiming && isUsingScopeView);
            }
        }

        /// <summary>
        /// Control shot inputs (primary and secundary weapons)
        /// </summary>
        public virtual void ShotInput()
        {
            if (!shooterManager || CurrentActiveWeapon == null || cc.isDead|| isReloading||isAttacking||isEquipping)
            {
                if (shooterManager && shooterManager.CurrentWeapon.chargeWeapon && shooterManager.CurrentWeapon.powerCharge != 0)
                {
                    CurrentActiveWeapon.powerCharge = 0;
                }

                shootCountA = 0;

                return;
            }

            if ((IsAiming && !shooterManager.hipfireShot || shooterManager.hipfireShot) && !shooterManager.isShooting && aimConditions)
            {
                if (CurrentActiveWeapon || (shooterManager.CurrentWeapon && shooterManager.hipfireShot))
                {
                    HandleShotCount(shooterManager.CurrentWeapon, shotInput.GetButton());
                }
            }
            else if (!IsAiming)
            {
                if (shooterManager.CurrentWeapon.chargeWeapon && shooterManager.CurrentWeapon.powerCharge != 0)
                {
                    CurrentActiveWeapon.powerCharge = 0;
                }

                shootCountA = 0;
            }
        }

        /// <summary>
        /// Control Shot count
        /// </summary>
        /// <param name="weapon">target weapon</param>
        /// <param name="weaponInput">check input</param>
        public virtual void HandleShotCount(vShooterWeapon weapon, bool weaponInput = true)
        {
           
            if (weapon.chargeWeapon)
            {
                if (shooterManager.WeaponHasLoadedAmmo() && weapon.powerCharge < 1 && weaponInput)
                {
                    if (shooterManager.hipfireShot)
                    {
                        _aimTimming = shooterManager.HipfireAimTime+CurrentActiveWeapon.shootFrequency;
                    }

                    weapon.powerCharge += Time.deltaTime * weapon.chargeSpeed;
                }
                else if ((weapon.powerCharge >= 1 && weapon.autoShotOnFinishCharge && weaponInput) ||
                    (!weaponInput && IsAiming && weapon.powerCharge > 0f))
                {
                    if (shooterManager.hipfireShot)
                    {
                        _aimTimming = shooterManager.HipfireAimTime + CurrentActiveWeapon.shootFrequency;
                    }                   
                    shootCountA=1;
                 
                }
                else if (!shooterManager.WeaponHasLoadedAmmo() && shooterManager.WeaponHasUnloadedAmmo() && !isReloading && CurrentActiveWeapon.autoReload)
                {
                    shooterManager.ReloadWeapon();
                }
                    animator.SetFloat(vAnimatorParameters.PowerCharger, weapon.powerCharge);
            }
            else if (weapon.automaticWeapon && weaponInput)
            {
                if (shooterManager.hipfireShot && !isAimingByInput)
                {
                   
                    _aimTimming = shooterManager.HipfireAimTime;
                   
                }
                shootCountA=1;      
            }
            else if (weaponInput)
            {
                if (shooterManager.hipfireShot && !isAimingByInput)
                { 
                    _aimTimming = shooterManager.HipfireAimTime;
                }
                
                if (allowAttack == false)
                {
                    shootCountA = 1;
                    allowAttack = true;
                }
            }
            else
            {
                allowAttack = false;
            }
        }

        /// <summary>
        /// Do Shots by shotcount after Ik behaviour updated
        /// </summary>
        public virtual void DoShots()
        {                         
                if (CanDoShots())
                {
                    animator.SetFloat(vAnimatorParameters.Shot_ID, shooterManager.GetShotID());                   
                    shooterManager.Shoot(AimPosition, !isAimingByInput);
                    if (CurrentActiveWeapon.chargeWeapon) CurrentActiveWeapon.powerCharge = 0;
                    shootCountA--;
                } 
        }

        /// <summary>
        /// Reload current weapon
        /// </summary>
        public virtual void ReloadInput()
        {
            if (!shooterManager || CurrentActiveWeapon == null)
            {
                return;
            }

            if (reloadInput.GetButtonDown() && !isReloading && !cc.customAction && !cc.ragdolled && !shooterManager.isShooting)
            {
                shootCountA = 0;
                _aimTimming = 0f;
                shooterManager.ReloadWeapon();
            }
        }

        /// <summary>
        /// Control Switch Camera side Input
        /// </summary>
        public virtual void SwitchCameraSideInput()
        {
            if (tpCamera == null)
            {
                return;
            }

            if (switchCameraSideInput.GetButtonDown())
            {
                SwitchCameraSide();
            }
        }

        /// <summary>
        /// Change side view of the <seealso cref="Invector.vCamera.vThirdPersonCamera"/>
        /// </summary>
        public virtual void SwitchCameraSide()
        {
            if (tpCamera == null)
            {
                return;
            }

            isCameraRightSwitched = !isCameraRightSwitched;
            tpCamera.SwitchRight(isCameraRightSwitched);
        }

        /// <summary>
        /// Reset the Aiming and AimCanvas to false
        /// </summary>
        public virtual void CancelAiming()
        {
            isAimingByInput = false;
            _aimTimming = 0;
            if (controlAimCanvas)
            {
                controlAimCanvas.SetActiveAim(false);
                controlAimCanvas.SetActiveScopeCamera(false);
            }
        }
      
        /// <summary>
        /// Control Scope view input
        /// </summary>
        public virtual void ScopeViewInput()
        {
            if (!shooterManager || CurrentActiveWeapon == null)
            {
                return;
            }

            if (isAimingByInput && aimConditions && (scopeViewInput.GetButtonDown() || CurrentActiveWeapon.onlyUseScopeUIView))
            {
                if (controlAimCanvas && CurrentActiveWeapon.scopeTarget)
                {
                    if (!isUsingScopeView && CurrentActiveWeapon.onlyUseScopeUIView)
                    {
                        EnableScopeView();
                    }
                    else if (isUsingScopeView && !CurrentActiveWeapon.onlyUseScopeUIView)
                    {
                        DisableScopeView();
                    }
                    else if (!isUsingScopeView)
                    {
                        EnableScopeView();
                    }
                }
            }
            else if (isUsingScopeView && (controlAimCanvas && !isAimingByInput || controlAimCanvas && !aimConditions || cc.isRolling))
            {
                DisableScopeView();
            }
        }

        /// <summary>
        /// Enable scope view (just if is aiming)
        /// </summary>
        public virtual void EnableScopeView()
        {
            if (!isAimingByInput||!controlAimCanvas.scopeBackgroundCamera || isReloading||isEquipping)
            {
                return;
            }

            isUsingScopeView = true;
            controlAimCanvas.SetActiveScopeCamera(true, CurrentActiveWeapon.useUI);
        }

        /// <summary>
        /// Disable scope view
        /// </summary>
        public virtual void DisableScopeView()
        {
            if (!controlAimCanvas.scopeBackgroundCamera) return;
            isUsingScopeView = false;
            controlAimCanvas.SetActiveScopeCamera(false);
        }

        ///// <summary>
        ///// Enable the BlockInput if you don't have any Shooter Weapons equipped
        ///// </summary>
        //public override void BlockingInput()
        //{
        //    if (shooterManager == null || (CurrentActiveWeapon == null && shooterManager.canUseMeleeBlock_H))
        //        base.BlockingInput();
        //}

        #endregion

        #region Update Animations

        protected override void UpdateMeleeAnimations()
        {
            // disable the onlyarms layer and run the melee methods if the character is not using any shooter weapon
            if (!animator)
            {
                return;
            }

            // find states with the IsEquipping tag
            isEquipping = cc.IsAnimatorTag("IsEquipping");
            // Check if Animator state need to ignore IK
            _ignoreIKFromAnimator = cc.IsAnimatorTag("IgnoreIK");

            if (cc.customAction)
            {
                ResetMeleeAnimations();
                ResetShooterAnimations();
                // reset to the default camera state
                UpdateCameraStates();
                // reset the aiming
                CancelAiming();
                return;
            }
            // update MeleeManager Animator Properties
            if ((shooterManager == null || !CurrentActiveWeapon) && meleeManager)
            {
                base.UpdateMeleeAnimations();
                // set the uppbody id (armsonly layer)
                //animator.SetFloat(vAnimatorParameters.UpperBody_ID, 0, .2f, Time.deltaTime);
                // turn on the onlyarms layer to aim 
                onlyArmsLayerWeight = Mathf.Lerp(onlyArmsLayerWeight, 0, 6f * vTime.deltaTime);
                animator.SetLayerWeight(onlyArmsLayer, onlyArmsLayerWeight);
                // reset aiming parameter
                animator.SetBool(vAnimatorParameters.IsAiming, false);
               // animator.SetBool(vAnimatorParameters.IsHipFire, false);
                isReloading = false;
            }
            // update ShooterManager Animator Properties
            else if (shooterManager && CurrentActiveWeapon)
            {
                UpdateShooterAnimations();
            }
            // reset Animator Properties
            else
            {
                ResetMoveSet();
                ResetMeleeAnimations();
                ResetShooterAnimations();
            }
        }

        public virtual void ResetMoveSet()
        {
            cc.animator.SetFloat(vAnimatorParameters.MoveSet_ID, defaultMoveSetID, .2f, Time.deltaTime);
        }

        public virtual void ResetShooterAnimations()
        {
            if (shooterManager == null || !animator)
            {
                return;
            }
            // set the uppbody id (armsonly layer)
            animator.SetFloat(vAnimatorParameters.UpperBody_ID, 0, .2f, vTime.deltaTime);
            // set if the character can aim or not (upperbody layer)
            animator.SetBool(vAnimatorParameters.CanAim, false);
            // character is aiming
            animator.SetBool(vAnimatorParameters.IsAiming, false);
           // animator.SetBool(vAnimatorParameters.IsHipFire, false);
            // turn on the onlyarms layer to aim 
            onlyArmsLayerWeight = Mathf.Lerp(onlyArmsLayerWeight, 0, 6f * vTime.deltaTime);
            animator.SetLayerWeight(onlyArmsLayer, onlyArmsLayerWeight);
        }

        protected virtual void UpdateShooterAnimations()
        {
            if (shooterManager == null)
            {
                return;
            }
           
            // turn on the onlyarms layer to aim 
            onlyArmsLayerWeight = Mathf.Lerp(onlyArmsLayerWeight, (CurrentActiveWeapon || isEquipping) ? 1f : 0f, shooterManager.onlyArmsSpeed * vTime.deltaTime);
            animator.SetLayerWeight(onlyArmsLayer, onlyArmsLayerWeight);
            if(CurrentActiveWeapon && IsAiming) animator.SetLayerWeight(shotLayer, isUsingScopeView ? CurrentActiveWeapon.scopeShootAnimationWeight : 1f);

            if (CurrentActiveWeapon && !shooterManager.useDefaultMovesetWhenNotAiming || IsAiming ||isReloading)
            {
                // set the move set id (base layer) 
                animator.SetFloat(vAnimatorParameters.MoveSet_ID, shooterMoveSetID, .1f, vTime.deltaTime);
            }
            else if (!CurrentActiveWeapon && !shooterManager.useDefaultMovesetWhenNotAiming || shooterManager.useDefaultMovesetWhenNotAiming)
            {
                // set the move set id (base layer) 
                animator.SetFloat(vAnimatorParameters.MoveSet_ID, defaultMoveSetID, .1f, vTime.deltaTime);
            }

            // set the isBlocking false while using shooter weapons
            animator.SetBool(vAnimatorParameters.IsBlocking, isBlocking);
            // set the uppbody id (armsonly layer)
            animator.SetFloat(vAnimatorParameters.UpperBody_ID, shooterManager.GetUpperBodyID());
            // set if the character can aim or not (upperbody layer)
            animator.SetBool(vAnimatorParameters.CanAim, aimConditions);
            // character is aiming
            animator.SetBool(vAnimatorParameters.IsAiming, IsAiming);
           // animator.SetBool(vAnimatorParameters.IsHipFire, isAimingByHipFire);
            // find states with the Reload tag
            isReloading = cc.IsAnimatorTag("IsReloading") || shooterManager.isReloadingWeapon;
            

            vAnimatorParameter ap = new vAnimatorParameter(animator, "IsReloading");
            
        }

        /// <summary>
        /// Current moveset id based if is using weapon or not
        /// </summary>
        public virtual int shooterMoveSetID
        {
            get
            {
                int id = shooterManager.GetMoveSetID();
                if (id == 0 || overrideWeaponMoveSetID)
                {
                    id = defaultMoveSetID;
                }

                return id;
            }
        }

        public override void UpdateCameraStates()
        {
            // CAMERA STATE - you can change the CameraState here, the bool means if you want lerp of not, make sure to use the same CameraState String that you named on TPCameraListData
            if (ignoreTpCamera)
            {
                return;
            }

            if (tpCamera == null)
            {
                tpCamera = FindObjectOfType<vCamera.vThirdPersonCamera>();
                if (tpCamera == null)
                {
                    return;
                }

                if (tpCamera)
                {
                    tpCamera.SetMainTarget(this.transform);
                    tpCamera.Init();
                }
            }          

            if (changeCameraState)
            {
                tpCamera.ChangeState(customCameraState, customlookAtPoint, true);
            }
            else if (cc.isCrouching && !isAimingByInput)
            {
                tpCamera.ChangeState("Crouch", true);
            }
            else if (cc.isStrafing && !isAimingByInput)
            {
                tpCamera.ChangeState("Strafing", true);
            }
            else if (isAimingByInput && CurrentActiveWeapon)
            {
                if (isUsingScopeView)
                {
                    if (string.IsNullOrEmpty(CurrentActiveWeapon.customScopeCameraState))
                    {
                        tpCamera.ChangeState(cc.isCrouching ? "CrouchingAiming" : "Aiming", true);
                    }
                    else
                    {
                        tpCamera.ChangeState(CurrentActiveWeapon.customScopeCameraState, true);
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(CurrentActiveWeapon.customAimCameraState))
                    {
                        tpCamera.ChangeState(cc.isCrouching ? "CrouchingAiming" : "Aiming", true);
                    }
                    else
                    {
                        tpCamera.ChangeState(CurrentActiveWeapon.customAimCameraState, true);
                    }
                }
            }
            else
            {
                tpCamera.ChangeState("Default", true);
            }
        }

        #endregion

        #region Update Aim

        protected virtual void UpdateAimPosition()
        {
            if (!shooterManager)
            {
                return;
            }

            if (CurrentActiveWeapon == null)
            {
                return;
            }

            var camT = isUsingScopeView && controlAimCanvas && controlAimCanvas.scopeBackgroundCamera ? //Check if is using canvas scope view
                    CurrentActiveWeapon.zoomScopeCamera ? /* if true, check if weapon has a zoomScopeCamera, 
                if true...*/
                    CurrentActiveWeapon.zoomScopeCamera.transform : controlAimCanvas.scopeBackgroundCamera.transform :
                    /*else*/cameraMain.transform;

            var origin1 = camT.position;
            if (!(controlAimCanvas && controlAimCanvas.isScopeCameraActive && controlAimCanvas.scopeBackgroundCamera))
            {
                origin1 = camT.position;
            }

            var vOrigin = origin1;
            vOrigin += controlAimCanvas && controlAimCanvas.isScopeCameraActive && controlAimCanvas.scopeBackgroundCamera ? camT.forward : Vector3.zero;
            AimPosition = camT.position + camT.forward * 100f;
            //aimAngleReference.transform.eulerAngles = new Vector3(aimAngleReference.transform.eulerAngles.x, transform.eulerAngles.y, aimAngleReference.transform.eulerAngles.z);
            if (!isUsingScopeView)
            {
                lastAimDistance = 100f;
            }

            if (shooterManager.raycastAimTarget && CurrentActiveWeapon.raycastAimTarget)
            {
                RaycastHit hit;
                Ray ray = new Ray(vOrigin, camT.forward);
                
                if (Physics.Raycast(ray, out hit, cameraMain.farClipPlane, shooterManager.damageLayer))
                {
                    bool canAimToHit = false;
                    if (hit.collider.transform.IsChildOf(transform))
                    {                       
                        var playerChild = hit.collider.gameObject;
                        var hits = Physics.RaycastAll(ray, cameraMain.farClipPlane, shooterManager.damageLayer);
                        var dist = cameraMain.farClipPlane;
                        for (int i = 0; i < hits.Length; i++)
                        {                           
                            if (hits[i].distance < dist && hits[i].collider.gameObject != playerChild.gameObject && !hits[i].collider.transform.IsChildOf(transform))
                            {
                                canAimToHit = true;
                                dist = hits[i].distance;
                                hit = hits[i];
                            }
                        }
                    }
                    else canAimToHit = true;
                    if (hit.collider && canAimToHit)
                    {
                        if (!isUsingScopeView)
                        {
                            lastAimDistance = Vector3.Distance(camT.position, hit.point);
                        }                       
                        AimPosition = hit.point;
                    }
                  
                }
                if (shooterManager.showCheckAimGizmos)
                {
                    Debug.DrawLine(ray.origin, AimPosition);
                }
            }
            if (isAimingByInput)
            {
                shooterManager.CameraSway();
            }
        }

        #endregion

        #region IK behaviour

        protected virtual void OnDrawGizmos()
        {
            if (!shooterManager || !shooterManager.showCheckAimGizmos)
            {
                return;
            }
            if (CurrentActiveWeapon && shooterManager.useCheckAim)
            {              
                Vector3 startPoint = Vector3.zero;
                Vector3 endPoint = Vector3.zero;               
                UpdateCheckAimPoints(ref startPoint, ref endPoint);
                var color = Gizmos.color;
                color = aimConditions ? Color.green : Color.red;
                color.a = 1f;
                Gizmos.color = color;               
                Gizmos.DrawLine(startPoint, endPoint);               
                Gizmos.DrawSphere(startPoint, 0.02f);
                Gizmos.DrawSphere(endPoint, shooterManager.checkAimRadius);
            }
        }

        protected virtual void UpdateAimBehaviour()
        {
            if (cc.isDead)
            {
                return;
            }           

            UpdateAimPosition();
            UpdateHeadTrack();
            OnStartUpdateIK();
            if (shooterManager && CurrentActiveWeapon)
            {                
                UpdateIKAdjust(shooterManager.IsLeftWeapon);              
                RotateAimArm(shooterManager.IsLeftWeapon);
                RotateAimHand(shooterManager.IsLeftWeapon);
                UpdateArmsIK(shooterManager.IsLeftWeapon);
                UpdateCheckAimHelpers(shooterManager.IsLeftWeapon);
            }
           
            OnFinishUpdateIK();
            CheckAimConditions();
            UpdateAimHud();
            DoShots();
        }
        
        protected virtual void UpdateAimAngleReference()
        {
            aimAngleReference.transform.rotation = transform.rotation;
        }

        protected virtual void UpdateCheckAimPoints(ref Vector3 start,ref Vector3 end)
        {
            if (CurrentActiveWeapon)
            {
                float checkAimSmooth = shooterManager.checkAimOffsetSmooth;
                ///Lerp offsets 
                checkCanAimOffsetStartX = Mathf.Lerp(checkCanAimOffsetStartX, IsCrouching ? shooterManager.checkAimCrouchedOffsetStartX : shooterManager.checkAimStandingOffsetStartX, checkAimSmooth * Time.deltaTime);
                checkCanAimOffsetStartY = Mathf.Lerp(checkCanAimOffsetStartY, IsCrouching ? shooterManager.checkAimCrouchedOffsetStartY : shooterManager.checkAimStandingOffsetStartY, checkAimSmooth * Time.deltaTime);
                checkCanAimOffsetEndX = Mathf.Lerp(checkCanAimOffsetEndX, IsCrouching ? shooterManager.checkAimCrouchedOffsetEndX : shooterManager.checkAimStandingOffsetEndX, checkAimSmooth * Time.deltaTime);
                checkCanAimOffsetEndY = Mathf.Lerp(checkCanAimOffsetEndY, IsCrouching ? shooterManager.checkAimCrouchedOffsetEndY : shooterManager.checkAimStandingOffsetEndY, checkAimSmooth * Time.deltaTime);
              
                /// Make original points to check aim 
                Vector3 startPoint = aimAngleReference.transform.TransformPoint(upperArmPosition);
                Vector3 endPoint = aimAngleReference.transform.TransformPoint(muzzlePosition);
                Vector3 forward = aimAngleReference.transform.InverseTransformDirection(muzzleForward);

                ///Apply offsets              
                Vector3 newStartPoint = startPoint + cameraMain.transform.right * (checkCanAimOffsetStartX * (tpCamera.switchRight > 0 ? 1 : -1)) + cameraMain.transform.up * checkCanAimOffsetStartY;
                Vector3 newEndPoint = endPoint + cameraMain.transform.right * (checkCanAimOffsetEndX * (tpCamera.switchRight > 0 ? 1 : -1)) + cameraMain.transform.up * checkCanAimOffsetEndY+forward * shooterManager.checkAimOffsetZ;

                start = newStartPoint;
                end = newEndPoint;
            }
        }

        protected virtual void OnFinishUpdateIK()
        {
            onFinishUpdateIK?.Invoke();
        }

        protected virtual void OnStartUpdateIK()
        {
            onStartUpdateIK?.Invoke();
        }  

        protected virtual void UpdateIKAdjust(bool isUsingLeftHand)
        {
            
            // create left arm ik solver if equal null
            if (LeftIK == null || !LeftIK.isValidBones)
            {
                LeftIK = new vIKSolver(animator, AvatarIKGoal.LeftHand);
                LeftIK.UpdateIK();
            }
            if (RightIK == null || !RightIK.isValidBones)
            {
                RightIK = new vIKSolver(animator, AvatarIKGoal.RightHand);
                RightIK.UpdateIK();
            }


            if (WeaponIKAdjustList == null) return;
            else
            {
                CurrentActiveWeapon.handIKTargetOffset.localPosition = isUsingLeftHand ? WeaponIKAdjustList.ikTargetPositionOffsetL : WeaponIKAdjustList.ikTargetPositionOffsetR;
                CurrentActiveWeapon.handIKTargetOffset.localEulerAngles = isUsingLeftHand ? WeaponIKAdjustList.ikTargetRotationOffsetL : WeaponIKAdjustList.ikTargetRotationOffsetR;
            }

            if (!CurrentWeaponIK || IsIgnoreIK)
            {
                LeftIK.UpdateIK();
                RightIK.UpdateIK();
                RightIK.SetIKWeight(0);
                LeftIK.SetIKWeight(0);
                weaponIKWeight = 0;
                return;
            }
            bool isValidIK = !cc.customAction && !isReloading && !isEquipping && CurrentWeaponIK != null && CurrentIKAdjust != null; 
            weaponIKWeight = Mathf.Lerp(weaponIKWeight, isValidIK ?1: 0, 25f * vTime.deltaTime);
            if (weaponIKWeight <= 0)
            {               
                return;
            }          
            
            if (isUsingLeftHand)
            {
                ApplyOffsets(LeftIK, RightIK,isValidIK);
            }
            else
            {
                ApplyOffsets(RightIK, LeftIK,isValidIK);
            }       
        }       

        protected virtual void ApplyOffsets(vIKSolver weaponHand, vIKSolver supportHand,bool isValidIK = true)
        {
            if (!weaponHand.isValidBones || !supportHand.isValidBones) return;
            //Apply Offset to Weapon Arm
            weaponHand.SetIKWeight(weaponIKWeight);
            ApplyOffsetToTargetBone(isValidIK? CurrentIKAdjust.weaponHandOffset : null, weaponHand.endBoneOffset, isValidIK);
            ApplyOffsetToTargetBone(isValidIK? CurrentIKAdjust.weaponHintOffset : null, weaponHand.middleBoneOffset, isValidIK);
            weaponHand.AnimationToIK();
            //Apply offset to Support Weapon Arm
           // supportHand.SetIKWeight(weaponIKWeight);
            ApplyOffsetToTargetBone(isValidIK ? CurrentIKAdjust.supportHandOffset : null, supportHand.endBoneOffset,!EditingIKGlobalOffset && isValidIK);
            ApplyOffsetToTargetBone(isValidIK ? CurrentIKAdjust.supportHintOffset : null, supportHand.middleBoneOffset, !EditingIKGlobalOffset && isValidIK);          
        }      

        protected virtual void ApplyOffsetToTargetBone(IKOffsetTransform iKOffset, Transform target, bool isValidIK)
        {
            try
            {
                target.localPosition = Vector3.Lerp(target.localPosition, isValidIK ? iKOffset.position : Vector3.zero, shooterManager.ikAdjustSmooth * vTime.deltaTime);
                target.localRotation = Quaternion.Lerp(target.localRotation, isValidIK ? Quaternion.Euler(iKOffset.eulerAngles) : Quaternion.Euler(Vector3.zero), shooterManager.ikAdjustSmooth * vTime.deltaTime);
            }catch
            {
                Debug.LogWarning("Can't Get IK Adjust");
            }
        }

        protected virtual void UpdateArmsIK(bool isUsingLeftHand = false)
        {
            // create left arm ik solver if equal null
            if (LeftIK == null || !LeftIK.isValidBones)
            {
                LeftIK = new vIKSolver(animator, AvatarIKGoal.LeftHand);
            }

            if (RightIK == null || !RightIK.isValidBones)
            {
                RightIK = new vIKSolver(animator, AvatarIKGoal.RightHand);
            }

            vIKSolver targetIK = null;

            if (isUsingLeftHand)
            {
                targetIK = RightIK;
            }
            else
            {
                targetIK = LeftIK;
            }
            bool useIK = isUsingLeftHand ? shooterManager.useLeftIK : shooterManager.useRightIK;
            if ((!shooterManager || !CurrentActiveWeapon || !useIK || IsIgnoreIK || isEquipping) ||
                (cc.IsAnimatorTag("Shot Fire") && CurrentActiveWeapon.disableIkOnShot))
            {
                if (supportIKWeight > 0)
                {
                    supportIKWeight = 0;
                    targetIK.SetIKWeight(0);
                }

               // Debug.Log($"Use ik {useIK.ToStringColor()}, IsIgnoreIK {IsIgnoreIK.ToStringColor()} "); // Debug IK conditions
                return;
            }
          
            bool useIkConditions = false;
            var animatorInput = System.Math.Round(cc.inputMagnitude,1);
            if (!IsAiming && !isAttacking)
            {
                var locomotionValidation = cc.isStrafing ? CurrentActiveWeapon.strafeIKOptions : CurrentActiveWeapon.freeIKOptions;
                if (locomotionValidation.use)
                {
                    if (animatorInput <= 0.1f)
                    {
                        useIkConditions = locomotionValidation.useOnIdle;
                    }
                    else if (animatorInput <= 0.5f)
                    {
                        useIkConditions = locomotionValidation.useOnWalk;
                    }
                    else if (animatorInput <= 1f)
                    {
                        useIkConditions = locomotionValidation.useOnRun;
                    }
                    else if (animatorInput <= 1.5f)
                    {
                        useIkConditions = locomotionValidation.useOnSprint;
                    }
                }
                else useIkConditions = false;
            }
            else if (IsAiming && !isAttacking)
            {
                useIkConditions =shooterManager.isShooting?!CurrentActiveWeapon.disableIkOnShot: CurrentActiveWeapon.useIKOnAiming;
            }
            else if (isAttacking)
            {
                useIkConditions = CurrentActiveWeapon.useIkAttacking;
            }

            IsSupportHandIKEnabled = useIkConditions;

            if (targetIK != null)
            {
                if (shooterManager.weaponIKAdjustList)
                {
                    if (isUsingLeftHand)
                    {
                        ikRotationOffset = shooterManager.weaponIKAdjustList.ikTargetRotationOffsetR;
                        ikPositionOffset = shooterManager.weaponIKAdjustList.ikTargetPositionOffsetR;
                    }
                    else
                    {
                        ikRotationOffset = shooterManager.weaponIKAdjustList.ikTargetRotationOffsetL;
                        ikPositionOffset = shooterManager.weaponIKAdjustList.ikTargetPositionOffsetL;
                    }
                }

                // Debug.Log($"using ik {useIkConditions.ToStringColor()}");//Debug IK Conditions
                // control weight of ik
                if (CurrentActiveWeapon && CurrentActiveWeapon.handIKTargetOffset && !isReloading && !cc.customAction && (cc.isGrounded || (IsAiming)) && useIkConditions)
                {
                    supportIKWeight = Mathf.Lerp(supportIKWeight, 1,shooterManager.armIKSmoothIn * vTime.deltaTime);
                }
                else
                {
                    supportIKWeight = Mathf.Lerp(supportIKWeight, 0, shooterManager.armIKSmoothOut * vTime.deltaTime);
                }

                if (supportIKWeight <= 0)
                {
                    return;
                }

                // update IK
                targetIK.SetIKWeight(shooterManager.armIKCurve.Evaluate(supportIKWeight));
                if (shooterManager && CurrentActiveWeapon && CurrentActiveWeapon.handIKTargetOffset)
                {

                    targetIK.SetIKPosition(CurrentActiveWeapon.handIKTargetOffset.position);
                    targetIK.SetIKRotation(CurrentActiveWeapon.handIKTargetOffset.rotation);


                    if (shooterManager.CurrentWeaponIK)
                    {
                        targetIK.AnimationToIK();
                    }
                }
            }
        }

        protected virtual bool CanRotateAimArm()
        {
            return cc.IsAnimatorTag("Upperbody Pose") && IsAimAlignWithForward() && cc.animatorStateInfos.GetCurrentNormalizedTime(cc.upperBodyLayer)>0.5f;
        }

        protected virtual bool CanDoShots()
        {
            return armAlignmentWeight >= 0.9f && cc.IsAnimatorTag("Upperbody Pose")  && shootCountA > 0 && !isReloading;
        }

        protected virtual void RotateAimArm(bool isUsingLeftHand = false)
        {
            if (!shooterManager)
            {
                return;
            }
          
            armAlignmentWeight = IsAiming && aimConditions && CanRotateAimArm() ? Mathf.Lerp(armAlignmentWeight, Mathf.Clamp(cc.upperBodyInfo.normalizedTime, 0, 1f), shooterManager.smoothArmWeight * (.001f + Time.deltaTime)) : 0;
            if (CurrentActiveWeapon && armAlignmentWeight > 0.01f && CurrentActiveWeapon.alignRightUpperArmToAim)
            {
                var upperArm = isUsingLeftHand ? leftUpperArm : rightUpperArm;
                var aimPoint = targetArmAlignmentPosition;               
                var aimPointLocal = transform.InverseTransformPoint(aimPoint);
                var upperArmLocal = transform.InverseTransformPoint(upperArm.position);
              
                aimPoint = transform.TransformPoint(aimPointLocal);
                Vector3 v = aimPoint - CurrentActiveWeapon.aimReference.position;
                var orientation = CurrentActiveWeapon.aimReference.forward;

              
                var rot = Quaternion.FromToRotation(upperArm.InverseTransformDirection(orientation), upperArm.InverseTransformDirection(v));

                if ((!float.IsNaN(rot.x) && !float.IsNaN(rot.y) && !float.IsNaN(rot.z)))
                {
                    upperArmRotationAlignment = shooterManager.isShooting ? armAlignmentWeight > .98f ? upperArmRotation : Quaternion.identity : rot;
                }

                var angle = Vector3.Angle((AimPosition - aimAngleReference.transform.position).normalized, aimAngleReference.transform.forward);
               
                if ((!(angle > shooterManager.maxAimAngle || angle < -shooterManager.maxAimAngle)) || controlAimCanvas && controlAimCanvas.isScopeCameraActive)
                {
                    upperArmRotation = Quaternion.Lerp(upperArmRotation, upperArmRotationAlignment, shooterManager.smoothArmIKRotation * (.001f + Time.deltaTime));
                }

                if (!float.IsNaN(upperArmRotation.x) && !float.IsNaN(upperArmRotation.y) && !float.IsNaN(upperArmRotation.z))
                {
                    var armWeight = CurrentActiveWeapon.alignRightHandToAim ? Mathf.Clamp(armAlignmentWeight, 0, 0.5f) : armAlignmentWeight;
                    upperArm.localRotation *= Quaternion.Euler(upperArmRotation.eulerAngles.NormalizeAngle() * armWeight);
                }

            }
            else
            {
                upperArmRotation = Quaternion.Euler(0, 0, 0);
            }
           
           
        }

        protected virtual void RotateAimHand(bool isUsingLeftHand = false)
        {
            if (!shooterManager)
            {
                return;
            }

            if (CurrentActiveWeapon && armAlignmentWeight > 0.01f && CurrentActiveWeapon.alignRightHandToAim)
            {
                var hand = isUsingLeftHand ? leftHand : rightHand;
                var aimPoint = targetArmAlignmentPosition;
                var aimPointLocal = transform.InverseTransformPoint(aimPoint);
                var handLocal = transform.InverseTransformPoint(hand.position);
                aimPoint = transform.TransformPoint(aimPointLocal);
                Vector3 v = aimPoint - (CurrentActiveWeapon.aimReference.position);
                var orientation = CurrentActiveWeapon.aimReference.forward;
                var rot = Quaternion.FromToRotation(hand.InverseTransformDirection(orientation), hand.InverseTransformDirection(v));
                if ((!float.IsNaN(rot.x) && !float.IsNaN(rot.y) && !float.IsNaN(rot.z)))
                {
                    handRotationAlignment =  shooterManager.isShooting  ? armAlignmentWeight > .98f? handRotation:Quaternion.identity : rot;
                }

                var angle = Vector3.Angle((AimPosition - aimAngleReference.transform.position).normalized, aimAngleReference.transform.forward);
                if ((!(angle > shooterManager.maxAimAngle || angle < -shooterManager.maxAimAngle)) || (controlAimCanvas && controlAimCanvas.isScopeCameraActive))
                {
                    handRotation = Quaternion.Lerp(handRotation, handRotationAlignment, shooterManager.smoothArmIKRotation * (.001f + Time.deltaTime));
                }

                if (!float.IsNaN(handRotation.x) && !float.IsNaN(handRotation.y) && !float.IsNaN(handRotation.z))
                {
                    var armWeight = armAlignmentWeight;
                    hand.localRotation *= Quaternion.Euler(handRotation.eulerAngles.NormalizeAngle() * armWeight);
                }


                CurrentActiveWeapon.SetScopeLookTarget(aimPoint);
            }
            else
            {
                handRotation = Quaternion.Euler(0, 0, 0);
            }         

        }

        protected void UpdateCheckAimHelpers(bool isUsingLeftHand)
        {           
            if (aimConditions == false||!IsAiming) return;
            if (armAlignmentWeight >= 1)
            {
                var upperArm = isUsingLeftHand ? leftUpperArm : rightUpperArm;
                upperArmPosition = aimAngleReference.transform.InverseTransformPoint(upperArm.position);
                muzzlePosition = aimAngleReference.transform.InverseTransformPoint(CurrentActiveWeapon.muzzle.position);
                muzzleForward = aimAngleReference.transform.InverseTransformDirection(CurrentActiveWeapon.muzzle.forward);
               
            }
        }

        protected virtual void CheckAimConditions()
        {
            if (!shooterManager)
            {
                return;
            }

            var weaponSide =  (tpCamera.switchRight<0 ? -1 : 1);

            if (CurrentActiveWeapon == null)
            {
                aimConditions = false;
                return;
            }

            if (shooterManager.useCheckAim == false ||!IsAiming)
            {
                aimConditions = true;
                return;
            }
            if (animator.IsInTransition(0)) return;
            Vector3 startPoint = Vector3.zero;
            Vector3 endPoint = Vector3.zero;

            UpdateCheckAimPoints(ref startPoint, ref endPoint);           
            if (Vector3.Distance(startPoint, AimPosition) < Vector3.Distance(startPoint, endPoint))
                aimConditions = false;
            var _ray = new Ray(startPoint, ((endPoint) - startPoint).normalized);

            if (Physics.SphereCast(_ray, shooterManager.checkAimRadius, out checkCanAimHit, (endPoint - startPoint).magnitude, shooterManager.blockAimLayer))
            {
                aimConditions = false;
            }
            else
            {
                aimConditions = true;
            }

            //aimWeight = Mathf.Lerp(aimWeight, aimConditions ? 1 : 0, 10 * Time.deltaTime);
        }        
     
        protected virtual bool IsAimAlignWithForward()
        {
            if (!shooterManager)
            {
                return false;
            }

            var dirA = aimAngleReference.transform.forward;
            dirA.y = 0;
            var dirB = targetArmAligmentDirection;          
            dirB.y = 0;

            var angle = dirA.AngleFormOtherDirection(dirB).y;

            return  ((Mathf.Abs(angle) < shooterManager.maxAimAngle));
        }

        protected virtual Vector3 targetArmAlignmentPosition
        {
            get
            {
                return isUsingScopeView && controlAimCanvas.scopeBackgroundCamera ? cameraMain.transform.position + cameraMain.transform.forward * lastAimDistance :shooterManager.alignArmToHitPoint?  AimPosition : cameraMain.transform.position+cameraMain.transform.forward*100;
            }
        }

        protected virtual Vector3 targetArmAligmentDirection
        {
            get
            {
                var t = controlAimCanvas && controlAimCanvas.isScopeCameraActive && controlAimCanvas.scopeBackgroundCamera ? controlAimCanvas.scopeBackgroundCamera.transform : cameraMain.transform;
                return t.forward;
            }
        }

        protected virtual void UpdateHeadTrack()
        {

            if (headTrack)
            {

                headTrack.ignoreSmooth = (IsAiming) || isUsingScopeView;
                UpdateHeadTrackLookPoint();
            }
            if (!shooterManager || !headTrack)
            {
                if (headTrack)
                {
                    headTrack.offsetSpine = Vector2.Lerp(headTrack.offsetSpine, Vector2.zero, headTrack.Smooth);
                    headTrack.offsetHead = Vector2.Lerp(headTrack.offsetHead, Vector2.zero, headTrack.Smooth);

                }
                return;
            }
            if (!CurrentActiveWeapon || !headTrack || !CurrentWeaponIK || CurrentIKAdjust==null)
            {
                if (headTrack)
                {
                    headTrack.offsetSpine = Vector2.Lerp(headTrack.offsetSpine, Vector2.zero, headTrack.Smooth);
                    headTrack.offsetHead = Vector2.Lerp(headTrack.offsetHead, Vector2.zero, headTrack.Smooth);
                }
                return;
            }
            if (IsAiming)
            {
                var ikAdjust = CurrentIKAdjust;
                var offsetSpine = ikAdjust.spineOffset.spine;
                var offsetHead = ikAdjust.spineOffset.head;
                headTrack.offsetSpine = Vector2.Lerp(headTrack.offsetSpine, offsetSpine, headTrack.Smooth);
                headTrack.offsetHead = Vector2.Lerp(headTrack.offsetHead, offsetHead, headTrack.Smooth);
            }
            else
            {


                var ikAdjust = CurrentIKAdjust;
                var offsetSpine = ikAdjust.spineOffset.spine;
                var offsetHead = ikAdjust.spineOffset.head;
                headTrack.offsetSpine = Vector2.Lerp(headTrack.offsetSpine, offsetSpine, headTrack.Smooth);
                headTrack.offsetHead = Vector2.Lerp(headTrack.offsetHead, offsetHead, headTrack.Smooth);
            }
        }

        protected virtual void UpdateHeadTrackLookPoint()
        {
            if (IsAiming && !isUsingScopeView)
            {
                headTrack.SetTemporaryLookPoint(cameraMain.transform.position + cameraMain.transform.forward * 10, 0.1f);
            }
        }

        protected virtual void UpdateAimHud()
        {
            if (!shooterManager || !controlAimCanvas)
            {
                return;
            }

            if (CurrentActiveWeapon == null)
            {
                return;
            }

            controlAimCanvas.SetAimCanvasID(CurrentActiveWeapon.scopeID);
            if (controlAimCanvas.scopeBackgroundCamera && controlAimCanvas.scopeBackgroundCamera.gameObject.activeSelf)
            {
                controlAimCanvas.SetAimToCenter(true);
            }
            else if (IsAiming)
            {
                RaycastHit hit;

                if (Physics.Linecast(CurrentActiveWeapon.muzzle.position, AimPosition, out hit, shooterManager.blockAimLayer))
                {
                    Debug.DrawLine(CurrentActiveWeapon.muzzle.position, hit.point);
                    controlAimCanvas.SetWordPosition(hit.point, aimConditions);
                }
                else
                {
                    Debug.DrawLine(CurrentActiveWeapon.muzzle.position, AimPosition);
                    controlAimCanvas.SetWordPosition(AimPosition, aimConditions);
                }
            }
            else
            {
                controlAimCanvas.SetAimToCenter(true);
            }
          
            if (controlAimCanvas.scopeBackgroundCamera && CurrentActiveWeapon.scopeTarget)
            {
                if (isUsingScopeView)
                {
                    RaycastHit hit;
                    if (Physics.Raycast(CurrentActiveWeapon.scopeTarget.position, CurrentActiveWeapon.scopeTarget.forward, out hit, 100f, shooterManager.blockAimLayer) || Physics.Raycast(CurrentActiveWeapon.scopeTarget.position, CurrentActiveWeapon.scopeTarget.forward, out hit, 100f, shooterManager.damageLayer))
                    {
                        AimPosition = hit.point;
                    }
                    else AimPosition = CurrentActiveWeapon.scopeTarget.position + CurrentActiveWeapon.scopeTarget.forward * 100;
                    var weight = (shooterManager.isShooting ? (1 - CurrentActiveWeapon.scopeShootAnimationWeight) : 1f);
                    var scopeLookDirection = Vector3.Lerp(controlAimCanvas.scopeBackgroundCamera.transform.forward, CurrentActiveWeapon.scopeTarget.forward, shooterManager.isShooting ? scopeDirectionWeight = 0 : scopeDirectionWeight = Mathf.Lerp(scopeDirectionWeight, 1.001f, (10f / CurrentActiveWeapon.shootFrequency) * Time.deltaTime));
                    var scopePosition = Vector3.Lerp(controlAimCanvas.scopeBackgroundCamera.transform.position, CurrentActiveWeapon.scopeTarget.position, weight);
                    controlAimCanvas.UpdateScopeCamera(scopePosition, scopeLookDirection, CurrentActiveWeapon.backGroundScopeZoom);
                }
                else scopeDirectionWeight = 1f;
            }
           
        }
      
        #endregion

    }

    public static partial class vAnimatorParameters
    {
        public static int UpperBody_ID = Animator.StringToHash("UpperBody_ID");
        public static int CanAim = Animator.StringToHash("CanAim");
        public static int IsAiming = Animator.StringToHash("IsAiming");
        public static int IsHipFire = Animator.StringToHash("IsHipFire");
        public static int Shot_ID = Animator.StringToHash("Shot_ID");
        public static int PowerCharger = Animator.StringToHash("PowerCharger");
    }
}