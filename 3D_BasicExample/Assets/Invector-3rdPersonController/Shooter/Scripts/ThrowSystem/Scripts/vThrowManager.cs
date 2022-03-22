using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Invector.vCharacterController
{
    [RequireComponent(typeof(LineRenderer))]
    [vClassHeader("THROW MANAGER")]
    public class vThrowManager : vMonoBehaviour
    {
        #region variables

      
        public enum CameraStyle
        {
            ThirdPerson, TopDown, SideScroll
        }
      
        [vEditorToolbar("Settings")]
        public CameraStyle cameraStyle;
        public bool canUseThrow;
        public Transform throwStartPoint;
        public GameObject throwEnd;
        public Rigidbody objectToThrow; 
        public LayerMask obstacles = 1 << 0;
        public float throwMaxForce = 15f;
        public float throwDelayTime = .25f;
        public float lineStepPerTime = .1f;
        public float lineMaxTime = 10f;
        public int maxThrowObjects = 6;
        public int currentThrowObject;
        public float exitThrowModeDelay = 0.5f;
        [Tooltip("Set ignore collision to the grenade to not collide with the Player")]
        public bool setIgnoreCollision;
        public bool debug;
        [vSeparator("Only for ThirdPerson Camera Style")]
        [Tooltip("The Third person camera right will be applied as offset to throw start point")]
        public bool useCameraRightAsOffset;
        [Tooltip("Increase or decrease the Offset right value"), vHideInInspector("useCameraRightAsOffset")]
        public float cameraRightOffsetMultiplier = 1f;
        [Tooltip("Rotate to aim point while aiming")]
        public bool rotateWhileAiming = true;
        public bool strafeWhileAiming = true;
        [vEditorToolbar("Inputs")]
        public GenericInput throwInput = new GenericInput("Mouse0", "RB", "RB");
        public GenericInput aimThrowInput = new GenericInput("G", "LB", "LB");
        public bool aimHoldingButton = true;


        [vEditorToolbar("Animations")]
        [Tooltip("Delay to exit the Throw Aiming Mode and get back to default locomotion")]      
        public string throwAnimation = "ThrowObject";
        public string holdingAnimation = "HoldingObject";
        public string cancelAnimation = "CancelThrow";
        [vEditorToolbar("Events")]
        public UnityEngine.Events.UnityEvent onEnableAim;
        public UnityEngine.Events.UnityEvent onCancelAim;
        public UnityEngine.Events.UnityEvent onThrowObject;
        public UnityEngine.Events.UnityEvent onCollectObject;
        public UnityEngine.Events.UnityEvent onFinishThrow;

        public Collider[] selfColliders;
        public virtual  vThrowUI ui
        {
            get
            {
                if (!_ui)
                {
                    _ui = FindObjectOfType<vThrowUI>();
                    if (_ui)
                    {
                        _ui.UpdateCount(this);
                    }
                }
                return _ui;
            }
        }
        [System.Serializable]
        public partial class ThrowObject
        {
            Rigidbody objectToThrow;
            int id;
            int count;
        }

        protected bool isAiming;
        protected bool inThrow;
        protected bool isThrowInput;
        protected Transform rightUpperArm;
        protected LineRenderer lineRenderer;
        protected vThrowUI _ui;
        protected vThirdPersonInput tpInput;
        protected RaycastHit hit;
        protected GameObject lastThrowable;
        protected vExplosive explosive;
        #endregion

        public virtual void CanUseThrow(bool value)
        {
            canUseThrow = value;
        }

        protected virtual IEnumerator Start()
        {
            yield return new WaitForEndOfFrame();
            if (ui != null)
            {
                ui.UpdateCount(this);
            }

            lineRenderer = GetComponent<LineRenderer>();
          
            if (lineRenderer)
            {
                lineRenderer.useWorldSpace = true;
            }

            canUseThrow = true;

            tpInput = GetComponentInParent<vThirdPersonInput>();
            if (currentThrowObject > maxThrowObjects) currentThrowObject = maxThrowObjects;
            if (tpInput)
            {
                selfColliders = tpInput.GetComponentsInChildren<Collider>(true);
                tpInput.onUpdate -= UpdateThrowInput;
                tpInput.onUpdate += UpdateThrowInput;
                tpInput.onFixedUpdate -= UpdateThrowBehavior;
                tpInput.onFixedUpdate += UpdateThrowBehavior;

                rightUpperArm = tpInput.animator.GetBoneTransform(HumanBodyBones.RightUpperArm);

                if (cameraStyle == CameraStyle.SideScroll)
                {
                    tpInput.cc.strafeSpeed.rotateWithCamera = true;
                }
            }
            if(cameraStyle != CameraStyle.ThirdPerson)
            {
                useCameraRightAsOffset = false;
                rotateWhileAiming = true;
                strafeWhileAiming = true;
            }
        }

        protected virtual void UpdateThrowBehavior()
        {
            UpdateThrow();

            if (objectToThrow == null || !tpInput.enabled || tpInput.cc.customAction || !canUseThrow || tpInput.cc.isDead)
            {
                isAiming = false;
                inThrow = false;
                isThrowInput = false;
                return;
            }
            
            MoveAndRotate();
        }

        protected virtual void UpdateThrowInput()
        {
            if (objectToThrow == null || !tpInput.enabled || tpInput.cc.customAction || !canUseThrow || tpInput.cc.isDead)
            {
                isAiming = false;
                inThrow = false;
                isThrowInput = false;
                return;
            }

            if (aimThrowInput.GetButtonDown() && !isAiming && !inThrow)
            {
                PrepareControllerToThrow(true);
                tpInput.animator.CrossFadeInFixedTime(holdingAnimation, 0.2f);
                onEnableAim.Invoke();
                return;
            }
            if (aimThrowInput.GetButtonUp() && aimHoldingButton && isAiming)
            {
                PrepareControllerToThrow(false);
                tpInput.animator.CrossFadeInFixedTime(cancelAnimation, 0.2f);
                onCancelAim.Invoke();
                onFinishThrow.Invoke();
            }

            if (throwInput.GetButtonDown() && isAiming && !inThrow)
            {
                isAiming = false;
                isThrowInput = true;
            }
            else if (!aimHoldingButton && aimThrowInput.GetButtonDown() && !isThrowInput && isAiming)
            {
                PrepareControllerToThrow(false);
                tpInput.animator.CrossFadeInFixedTime(cancelAnimation, 0.2f);
                onCancelAim.Invoke();
                onFinishThrow.Invoke();
            }
        }

        protected virtual void MoveAndRotate()
        {
            if (isAiming || inThrow)
            {
                tpInput.MoveInput();
                switch (cameraStyle)
                {
                    case CameraStyle.ThirdPerson:
                        if (inThrow || rotateWhileAiming) tpInput.cc.RotateToDirection(tpInput.cameraMain.transform.forward);
                        break;
                    case CameraStyle.TopDown:
                        var dir = aimDirection;
                        dir.y = 0;
                        if(inThrow|| rotateWhileAiming)  tpInput.cc.RotateToDirection(dir);
                        break;
                    case CameraStyle.SideScroll:
                        ///
                        break;
                }
            }
        }

        protected virtual void LaunchObject(Rigidbody projectily)
        {
            
            projectily.AddForce(StartVelocity, ForceMode.VelocityChange);
        }

        protected virtual void UpdateThrow()
        {
            if (objectToThrow == null || !tpInput.enabled || tpInput.cc.customAction)
            {
                isAiming = false;
                inThrow = false;
                isThrowInput = false;
                if (lineRenderer && lineRenderer.enabled)
                {
                    lineRenderer.enabled = false;
                }

                if (throwEnd && throwEnd.activeSelf)
                {
                    throwEnd.SetActive(false);
                }

                return;
            }

            if (isAiming)
            {
                DrawTrajectory();
            }
            else
            {
                if (lineRenderer && lineRenderer.enabled)
                {
                    lineRenderer.enabled = false;
                }

                if (throwEnd && throwEnd.activeSelf)
                {
                    throwEnd.SetActive(false);
                }
            }

            if (isThrowInput)
            {
                inThrow = true;
                isThrowInput = false;
                tpInput.animator.CrossFadeInFixedTime(throwAnimation, 0.2f);
                currentThrowObject -= 1;
                StartCoroutine(Launch());
            }
        }

        protected virtual void DrawTrajectory()
        {
            var points = GetTrajectoryPoints(startPoint, StartVelocity, lineStepPerTime, lineMaxTime);
            if (lineRenderer)
            {
                if (!lineRenderer.enabled)
                {
                    lineRenderer.enabled = true;
                }

                lineRenderer.positionCount = points.Count;
                lineRenderer.SetPositions(points.ToArray());
            }
            if (throwEnd)
            {
                if (!throwEnd.activeSelf)
                {
                    throwEnd.SetActive(true);
                }

                if (objectToThrow.gameObject != lastThrowable)
                {
                    lastThrowable = objectToThrow.gameObject;
                    explosive = lastThrowable.GetComponentInChildren<vExplosive>();

                }
                if (explosive)
                {
                    throwEnd.transform.localScale = Vector3.one * explosive.maxExplosionRadius;
                }

                if (points.Count > 1)
                {
                    throwEnd.transform.position = points[points.Count - 1];
                }
            }
        }

        protected virtual IEnumerator Launch()
        {           
            yield return new WaitForSeconds(throwDelayTime);
            var obj = Instantiate(objectToThrow, startPoint, throwStartPoint.rotation);
            if (setIgnoreCollision)
            {
                var coll = obj.GetComponent<Collider>();
                if (coll)
                {

                    for (int i = 0; i < selfColliders.Length; i++)
                    {
                        Physics.IgnoreCollision(coll, selfColliders[i], true);
                    }
                }
            }
            obj.isKinematic = false;
            LaunchObject(obj);          
            if (ui)
            {
                ui.UpdateCount(this);
            }

            onThrowObject.Invoke();

            yield return new WaitForSeconds(2 * lineStepPerTime);          
            inThrow = false;

            if (currentThrowObject <= 0)
            {
                objectToThrow = null;
            }

            yield return new WaitForSeconds(exitThrowModeDelay);
            PrepareControllerToThrow(false);
            onFinishThrow.Invoke();
        }

        protected virtual void PrepareControllerToThrow(bool value)
        {
            isAiming = value;
            tpInput.SetLockAllInput(value);
            tpInput.SetStrafeLocomotion(value && strafeWhileAiming);

            if (cameraStyle == CameraStyle.SideScroll)
            {
                tpInput.cc.strafeSpeed.rotateWithCamera = true;
            }
        }

        protected virtual Vector3 thirdPersonAimPoint
        {
            get
            {
                return startPoint + tpInput.cameraMain.transform.forward * throwMaxForce;
            }
        }

        protected virtual Vector3 topdownAimPoint
        {
            get
            {
                var pos = vMousePositionHandler.Instance.WorldMousePosition(obstacles);
                pos.y = transform.position.y;
                return pos;
            }
        }

        protected virtual Vector3 sideScrollAimPoint
        {
            get
            {
                var localPos = transform.InverseTransformPoint(vMousePositionHandler.Instance.WorldMousePosition(obstacles));
                localPos.x = 0;

                return transform.TransformPoint(localPos);
            }
        }

        protected virtual Vector3 startPoint
        {
            get
            {
                Vector3 point = throwStartPoint.position;
                if (useCameraRightAsOffset && tpInput && tpInput.tpCamera && tpInput.tpCamera.lerpState!=null)
                {
                    point += tpInput.tpCamera.transform.right * tpInput.tpCamera.lerpState.right* cameraRightOffsetMultiplier * tpInput.tpCamera.switchRight;
                }
                return point;
            }
        }

        protected virtual Vector3 StartVelocity
        {
            get
            {
                RaycastHit hit;
                var dist = Vector3.Distance(startPoint, aimPoint);

                if (debug)
                {
                    Debug.DrawLine(startPoint, aimPoint);
                }

                if (cameraStyle == CameraStyle.ThirdPerson)
                {
                    if (Physics.Raycast(startPoint, aimDirection.normalized, out hit,aimDirection.magnitude, obstacles))
                    {
                        dist = hit.distance;                      
                    }
                }

                if (cameraStyle != CameraStyle.SideScroll)
                {
                    var force = Mathf.Clamp(dist, 0, throwMaxForce);
                    //var rotation = Quaternion.LookRotation(aimDirection.normalized, Vector3.up);
                    var dir = aimDirection.normalized;
                    return dir * force;
                }
                else
                {
                    var force = Mathf.Clamp(dist, 0, throwMaxForce);
                    return aimDirection.normalized * force;
                }
            }
        }

        protected virtual Vector3 PlotTrajectoryAtTime(Vector3 start, Vector3 startVelocity, float time)
        {
            return start + startVelocity * time + Physics.gravity * time * time * 0.5f;
        }

        protected virtual List<Vector3> GetTrajectoryPoints(Vector3 start, Vector3 startVelocity, float timestep, float maxTime)
        {
            Vector3 prev = start;
            List<Vector3> points = new List<Vector3>();
            points.Add(prev);
            for (int i = 1; ; i++)
            {
                float t = timestep * i;
                if (t > maxTime)
                {
                    break;
                }

                Vector3 pos = PlotTrajectoryAtTime(start, startVelocity, t);
                RaycastHit hit;
                if (Physics.Linecast(prev, pos, out hit, obstacles))
                {
                    points.Add(hit.point);
                    break;
                }
                if (debug)
                {
                    Debug.DrawLine(prev, pos, Color.red);
                }

                points.Add(pos);
                prev = pos;

            }
            return points;
        }

        public virtual Vector3 aimPoint
        {
            get
            {
                switch (cameraStyle)
                {
                    case CameraStyle.ThirdPerson: return thirdPersonAimPoint;
                    case CameraStyle.TopDown: return topdownAimPoint;
                    case CameraStyle.SideScroll: return sideScrollAimPoint;
                }
                return startPoint + tpInput.cameraMain.transform.forward * throwMaxForce;
            }
        }

        public virtual Vector3 aimDirection
        {
            get
            {
                return aimPoint - startPoint;
            }
        }

        public virtual void SetAmount(int value)
        {
            currentThrowObject += value;
            if (ui)
            {
                ui.UpdateCount(this);
            }

            onCollectObject.Invoke();
        }
    }
}