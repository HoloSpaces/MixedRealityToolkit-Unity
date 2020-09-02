// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Experimental.UI;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Physics;
using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

namespace Microsoft.MixedReality.Toolkit.UI
{
    /// <summary>
    /// This script allows for an object to be movable, scalable, and rotatable with one or two hands. 
    /// You may also configure the script on only enable certain manipulations. The script works with 
    /// both HoloLens' gesture input and immersive headset's motion controller input.
    /// </summary>
    [HelpURL("https://microsoft.github.io/MixedRealityToolkit-Unity/Documentation/README_ObjectManipulator.html")]
    public class ObjectManipulator : MonoBehaviour, IMixedRealityPointerHandler, IMixedRealityFocusChangedHandler
    {
        #region Public Enums

        /// <summary>
        /// Describes what pivot the manipulated object will rotate about when
        /// you rotate your hand. This is not a description of any limits or 
        /// additional rotation logic. If no other factors (such as constraints)
        /// are involved, rotating your hand by an amount should rotate the object
        /// by the same amount.
        /// For example a possible future value here is RotateAboutUserDefinedPoint
        /// where the user could specify a pivot that the object is to rotate
        /// around.
        /// An example of a value that should not be found here is MaintainRotationToUser
        /// as this restricts rotation of the object when we rotate the hand.
        /// </summary>
        public enum RotateInOneHandType
        {
            RotateAboutObjectCenter,
            RotateAboutGrabPoint
        };
        [System.Flags]
        public enum ReleaseBehaviorType
        {
            KeepVelocity = 1 << 0,
            KeepAngularVelocity = 1 << 1
        }
        #endregion Public Enums

        #region Serialized Fields

        [SerializeField]
        [Tooltip("Transform that will be dragged. Defaults to the object of the component.")]
        private Transform hostTransform = null;

        /// <summary>
        /// Transform that will be dragged. Defaults to the object of the component.
        /// </summary>
        public Transform HostTransform
        {
            get
            {
                if (hostTransform == null)
                {
                    hostTransform = gameObject.transform;
                }

                return hostTransform;
            }
            set => hostTransform = value;
        }

        [SerializeField]
        [EnumFlags]
        [Tooltip("Can manipulation be done only with one hand, only with two hands, or with both?")]
        private ManipulationHandFlags manipulationType = ManipulationHandFlags.OneHanded | ManipulationHandFlags.TwoHanded;

        /// <summary>
        /// Can manipulation be done only with one hand, only with two hands, or with both?
        /// </summary>
        public ManipulationHandFlags ManipulationType
        {
            get => manipulationType;
            set => manipulationType = value;
        }

        [SerializeField]
        [EnumFlags]
        [Tooltip("What manipulation will two hands perform?")]
        private TransformFlags twoHandedManipulationType = TransformFlags.Move | TransformFlags.Rotate | TransformFlags.Scale;

        /// <summary>
        /// What manipulation will two hands perform?
        /// </summary>
        public TransformFlags TwoHandedManipulationType
        {
            get => twoHandedManipulationType;
            set => twoHandedManipulationType = value;
        }

        [SerializeField]
        [Tooltip("Specifies whether manipulation can be done using far interaction with pointers.")]
        private bool allowFarManipulation = true;

        /// <summary>
        /// Specifies whether manipulation can be done using far interaction with pointers.
        /// </summary>
        public bool AllowFarManipulation
        {
            get => allowFarManipulation;
            set => allowFarManipulation = value;
        }

        [SerializeField]
        [Tooltip("Rotation behavior of object when using one hand near")]
        private RotateInOneHandType oneHandRotationModeNear = RotateInOneHandType.RotateAboutGrabPoint;

        /// <summary>
        /// Rotation behavior of object when using one hand near
        /// </summary>
        public RotateInOneHandType OneHandRotationModeNear
        {
            get => oneHandRotationModeNear;
            set => oneHandRotationModeNear = value;
        }

        [SerializeField]
        [Tooltip("Rotation behavior of object when using one hand at distance")]
        private RotateInOneHandType oneHandRotationModeFar = RotateInOneHandType.RotateAboutGrabPoint;

        /// <summary>
        /// Rotation behavior of object when using one hand at distance
        /// </summary>
        public RotateInOneHandType OneHandRotationModeFar
        {
            get => oneHandRotationModeFar;
            set => oneHandRotationModeFar = value;
        }

        [SerializeField]
        [EnumFlags]
        [Tooltip("Rigid body behavior of the dragged object when releasing it.")]
        private ReleaseBehaviorType releaseBehavior = ReleaseBehaviorType.KeepVelocity | ReleaseBehaviorType.KeepAngularVelocity;

        [SerializeField]
        [Tooltip("Multiply with existing velocity")]
        public float keepVelocityMutliplier = 1.0f;

        [SerializeField]
        [Tooltip("Uses object veolicity Instead of hand velocity")]
        public bool useObjectVelocity = true;

        /// <summary>
        /// Rigid body behavior of the dragged object when releasing it.
        /// </summary>
        public ReleaseBehaviorType ReleaseBehavior
        {
            get => releaseBehavior;
            set => releaseBehavior = value;
        }

        [SerializeField]
        [Tooltip("Check to enable frame-rate independent smoothing.")]
        private bool smoothingActive = true;

        /// <summary>
        /// Check to enable frame-rate independent smoothing.
        /// </summary>
        public bool SmoothingActive
        {
            get => smoothingActive;
            set => smoothingActive = value;
        }

        [SerializeField]
        [Range(0, 1)]
        [Tooltip("Enter amount representing amount of smoothing to apply to the movement. Smoothing of 0 means no smoothing. Max value means no change to value.")]
        private float moveLerpTime = 0.001f;

        /// <summary>
        /// Enter amount representing amount of smoothing to apply to the movement. Smoothing of 0 means no smoothing. Max value means no change to value.
        /// </summary>
        public float MoveLerpTime
        {
            get => moveLerpTime;
            set => moveLerpTime = value;
        }

        [SerializeField]
        [Range(0, 1)]
        [Tooltip("Enter amount representing amount of smoothing to apply to the rotation. Smoothing of 0 means no smoothing. Max value means no change to value.")]
        private float rotateLerpTime = 0.001f;

        /// <summary>
        /// Enter amount representing amount of smoothing to apply to the rotation. Smoothing of 0 means no smoothing. Max value means no change to value.
        /// </summary>
        public float RotateLerpTime
        {
            get => rotateLerpTime;
            set => rotateLerpTime = value;
        }

        [SerializeField]
        [Range(0, 1)]
        [Tooltip("Enter amount representing amount of smoothing to apply to the scale. Smoothing of 0 means no smoothing. Max value means no change to value.")]
        private float scaleLerpTime = 0.001f;

        [SerializeField]
        [Tooltip("Condition if Z Axis Offset Transformation should be allowed.")]
        private bool enableZAxisOffset = false;

        [SerializeField]
        [Tooltip("Max distance transformation should be allowed in axis.")]
        private float maxZaxisDistanceMove = 20.0f;

        [SerializeField]
        [Tooltip("Factor for the velocity for a Z Axis Transformation.")]
        private float zAxisOffsetVelocity = 0.5f;

        /// <summary>
        /// Enter amount representing amount of smoothing to apply to the scale. Smoothing of 0 means no smoothing. Max value means no change to value.
        /// </summary>
        public float ScaleLerpTime
        {
            get => scaleLerpTime;
            set => scaleLerpTime = value;
        }

        #endregion Serialized Fields

        #region Event handlers
        [Header("Manipulation Events")]
        [SerializeField]
        [FormerlySerializedAs("OnManipulationStarted")]
        private ManipulationEvent onManipulationStarted = new ManipulationEvent();

        /// <summary>
        /// Unity event raised on manipulation started
        /// </summary>
        public ManipulationEvent OnManipulationStarted
        {
            get => onManipulationStarted;
            set => onManipulationStarted = value;
        }

        [SerializeField]
        [FormerlySerializedAs("OnManipulationEnded")]
        private ManipulationEvent onManipulationEnded = new ManipulationEvent();

        /// <summary>
        /// Unity event raised on manipulation ended
        /// </summary>
        public ManipulationEvent OnManipulationEnded
        {
            get => onManipulationEnded;
            set => onManipulationEnded = value;
        }

        [SerializeField]
        [FormerlySerializedAs("OnHoverEntered")]
        private ManipulationEvent onHoverEntered = new ManipulationEvent();

        /// <summary>
        /// Unity event raised on hover started
        /// </summary>
        public ManipulationEvent OnHoverEntered
        {
            get => onHoverEntered;
            set => onHoverEntered = value;
        }

        [SerializeField]
        [FormerlySerializedAs("OnHoverExited")]
        private ManipulationEvent onHoverExited = new ManipulationEvent();

        /// <summary>
        /// Unity event raised on hover ended
        /// </summary>
        public ManipulationEvent OnHoverExited
        {
            get => onHoverExited;
            set => onHoverExited = value;
        }
        #endregion

        #region Private Properties

        private ManipulationMoveLogic moveLogic;
        private TwoHandScaleLogic scaleLogic;
        private TwoHandRotateLogic rotateLogic;

        private readonly Vector3[] velocityPositionsCache = new Vector3[velocityUpdateInterval];
        private Vector3 velocityPositionsSum = Vector3.zero;
        private float deltaTimeStart;
        private const int velocityUpdateInterval = 6;
        private int frameOn = 0;
        private Vector3 objectVelocity = Vector3.zero;

        /// <summary>
        /// Holds the pointer and the initial intersection point of the pointer ray 
        /// with the object on pointer down in pointer space
        /// </summary>
        private struct PointerData
        {
            public IMixedRealityPointer pointer;
            private Vector3 initialGrabPointInPointer;

            public PointerData(IMixedRealityPointer pointer, Vector3 worldGrabPoint) : this()
            {
                this.pointer = pointer;
                this.initialGrabPointInPointer = Quaternion.Inverse(pointer.Rotation) * (worldGrabPoint - pointer.Position);
            }

            public bool IsNearPointer => pointer is IMixedRealityNearPointer;

            /// Returns the grab point on the manipulated object in world space
            public Vector3 GrabPoint => (pointer.Rotation * initialGrabPointInPointer) + pointer.Position;
        }

        private Dictionary<uint, PointerData> pointerIdToPointerMap = new Dictionary<uint, PointerData>();
        private Quaternion objectToGripRotation;
        private bool isNearManipulation;
        private bool isManipulationStarted;

#pragma warning disable 108
        private Rigidbody rigidbody;
#pragma warning restore 108
        private bool wasKinematic = false;

        public bool UseRigidBody { get; set; } = false;

        private ConstraintManager constraints;

        private float manipulationZOffset = 0.0f; // This is is to support, object moveforwad/backward on touch interactions
        public float ManipulationOffset
        {
            get => manipulationZOffset;
            set => manipulationZOffset = value;
        }

        private bool IsOneHandedManipulationEnabled => manipulationType.HasFlag(ManipulationHandFlags.OneHanded) && pointerIdToPointerMap.Count == 1;
        private bool IsTwoHandedManipulationEnabled => manipulationType.HasFlag(ManipulationHandFlags.TwoHanded) && pointerIdToPointerMap.Count > 1;

        #endregion

        #region MonoBehaviour Functions

        private void Awake()
        {
            moveLogic = new ManipulationMoveLogic();
            rotateLogic = new TwoHandRotateLogic();
            scaleLogic = new TwoHandScaleLogic();
        }

        private TouchpadPositionListener listner;
        private void Start()
        {
            rigidbody = HostTransform.GetComponent<Rigidbody>();

            if(rigidbody == null)
                UseRigidBody = false;
            
            if (enableZAxisOffset)
            {
                OnManipulationStarted.AddListener(AddTouchpadEventListner);
                OnManipulationEnded.AddListener(RemoveTouchpadEventListner);
            }
              
            constraints = new ConstraintManager(gameObject);
        }
        #endregion MonoBehaviour Functions

        #region Private Methods

        private void AddTouchpadEventListner(ManipulationEventData data)
        {
            manipulationZOffset = 0;
            TouchpadPositionListener.Instance.RegisterScrollCallback(data, (scrollDelta) =>
            {
                float newOffsetDelta = scrollDelta * Time.deltaTime * zAxisOffsetVelocity;
                manipulationZOffset += newOffsetDelta;
            });
        }

        private void RemoveTouchpadEventListner(ManipulationEventData data)
        {
            TouchpadPositionListener.Instance.UnRegisterScrollCallback();
        }

        private Vector3 GetPointersGrabPoint()
        {
            Vector3 sum = Vector3.zero;
            int count = 0;
            foreach (var p in pointerIdToPointerMap.Values)
            {
                sum += p.GrabPoint;
                count++;
            }
            return sum / Math.Max(1, count);
        }

        private MixedRealityPose GetPointersPose()
        {
            Vector3 sumPos = Vector3.zero;
            Vector3 sumDir = Vector3.zero;
            int count = 0;
            foreach (var p in pointerIdToPointerMap.Values)
            {
                sumPos += p.pointer.Position;
                sumDir += p.pointer.Rotation * Vector3.forward;
                count++;
            }

            return new MixedRealityPose
            {
                Position = sumPos / Math.Max(1, count),
                Rotation = Quaternion.LookRotation(sumDir / Math.Max(1, count))
            };
        }

        private Vector3 GetPointersVelocity()
        {
            Vector3 sum = Vector3.zero;
            int numControllers = 0;
            foreach (var p in pointerIdToPointerMap.Values)
            {
                // Check pointer has a valid controller (e.g. gaze pointer doesn't)
                if (p.pointer.Controller != null)
                {
                    numControllers++;
                    sum += p.pointer.Controller.Velocity;
                }
            }
            return sum / Math.Max(1, numControllers);
        }

        private Vector3 GetPointersAngularVelocity()
        {
            Vector3 sum = Vector3.zero;
            int numControllers = 0;
            foreach (var p in pointerIdToPointerMap.Values)
            {
                // Check pointer has a valid controller (e.g. gaze pointer doesn't)
                if (p.pointer.Controller != null)
                {
                    numControllers++;
                    sum += p.pointer.Controller.AngularVelocity;
                }
            }
            return sum / Math.Max(1, numControllers);
        }

        private bool IsNearManipulation()
        {
            foreach (var item in pointerIdToPointerMap)
            {
                if (item.Value.IsNearPointer)
                {
                    return true;
                }
            }
            return false;
        }
        #endregion Private Methods

        #region Public Methods

        /// <summary>
        /// Releases the object that is currently manipulated
        /// </summary>
        public void ForceEndManipulation()
        {
            // end manipulation
            if (isManipulationStarted)
            {
                HandleManipulationEnded(GetPointersGrabPoint(), GetPointersVelocity(), GetPointersAngularVelocity());
            }
            pointerIdToPointerMap.Clear();
        }

        /// <summary>
        /// Gets the grab point for the given pointer id.
        /// Only use if you know that your given pointer id corresponds to a pointer that has grabbed
        /// this component.
        /// </summary>
        public Vector3 GetPointerGrabPoint(uint pointerId)
        {
            Assert.IsTrue(pointerIdToPointerMap.ContainsKey(pointerId));
            return pointerIdToPointerMap[pointerId].GrabPoint;
        }

        #endregion Public Methods

        #region Hand Event Handlers

        /// <inheritdoc />
        public void OnPointerDown(MixedRealityPointerEventData eventData)
        {
            if (eventData.used ||
                (!allowFarManipulation && eventData.Pointer as IMixedRealityNearPointer == null))
            {
                return;
            }

            // If we only allow one handed manipulations, check there is no hand interacting yet. 
            if (manipulationType != ManipulationHandFlags.OneHanded || pointerIdToPointerMap.Count == 0)
            {
                uint id = eventData.Pointer.PointerId;
                // Ignore poke pointer events
                if (!pointerIdToPointerMap.ContainsKey(id))
                {
                    // cache start ptr grab point
                    pointerIdToPointerMap.Add(id, new PointerData(eventData.Pointer, eventData.Pointer.Result.Details.Point));

                    // Call manipulation started handlers
                    if (IsTwoHandedManipulationEnabled)
                    {
                        if (!isManipulationStarted)
                        {
                            HandleManipulationStarted();
                        }
                        HandleTwoHandManipulationStarted();
                    }
                    else if (IsOneHandedManipulationEnabled)
                    {
                        if (!isManipulationStarted)
                        {
                            HandleManipulationStarted();
                        }
                        HandleOneHandMoveStarted();
                    }
                }
            }

            if (pointerIdToPointerMap.Count > 0)
            {
                // Always mark the pointer data as used to prevent any other behavior to handle pointer events
                // as long as the ObjectManipulator is active.
                // This is due to us reacting to both "Select" and "Grip" events.
                eventData.Use();
            }
        }

        public void OnPointerDragged(MixedRealityPointerEventData eventData)
        {
            // Call manipulation updated handlers
            if (IsOneHandedManipulationEnabled)
            {
                HandleOneHandMoveUpdated();
            }
            else if (IsTwoHandedManipulationEnabled)
            {
                HandleTwoHandManipulationUpdated();
            }
        }

        /// <inheritdoc />
        public void OnPointerUp(MixedRealityPointerEventData eventData)
        {
            // Get pointer data before they are removed from the map
            Vector3 grabPoint = GetPointersGrabPoint();
            Vector3 velocity = GetPointersVelocity();
            Vector3 angularVelocity = GetPointersAngularVelocity();

            uint id = eventData.Pointer.PointerId;
            if (pointerIdToPointerMap.ContainsKey(id))
            {
                pointerIdToPointerMap.Remove(id);
            }

            // Call manipulation ended handlers
            var handsPressedCount = pointerIdToPointerMap.Count;
            if (manipulationType.HasFlag(ManipulationHandFlags.TwoHanded) && handsPressedCount == 1)
            {
                if (manipulationType.HasFlag(ManipulationHandFlags.OneHanded))
                {
                    HandleOneHandMoveStarted();
                }
                else
                {
                    HandleManipulationEnded(grabPoint, velocity, angularVelocity);
                }
            }
            else if (isManipulationStarted && handsPressedCount == 0)
            {
                HandleManipulationEnded(grabPoint, velocity, angularVelocity);
            }

            eventData.Use();
        }

        #endregion Hand Event Handlers

        #region Private Event Handlers
        private void HandleTwoHandManipulationStarted()
        {
            var handPositionArray = GetHandPositionArray();

            if (twoHandedManipulationType.HasFlag(TransformFlags.Rotate))
            {
                rotateLogic.Setup(handPositionArray, HostTransform);
            }
            if (twoHandedManipulationType.HasFlag(TransformFlags.Move))
            {
                MixedRealityPose pointerPose = GetPointersPose();
                MixedRealityPose hostPose = new MixedRealityPose(HostTransform.position, HostTransform.rotation);
                moveLogic.Setup(pointerPose, GetPointersGrabPoint(), hostPose, HostTransform.localScale);
            }
            if (twoHandedManipulationType.HasFlag(TransformFlags.Scale))
            {
                scaleLogic.Setup(handPositionArray, HostTransform);
            }
        }

        private void HandleTwoHandManipulationUpdated()
        {
            var targetTransform = new MixedRealityTransform(HostTransform.position, HostTransform.rotation, HostTransform.localScale);

            var handPositionArray = GetHandPositionArray();

            if (twoHandedManipulationType.HasFlag(TransformFlags.Scale))
            {
                targetTransform.Scale = scaleLogic.UpdateMap(handPositionArray);
                constraints.ApplyScaleConstraints(ref targetTransform, false, IsNearManipulation());
            }
            if (twoHandedManipulationType.HasFlag(TransformFlags.Rotate))
            {
                targetTransform.Rotation = rotateLogic.Update(handPositionArray, targetTransform.Rotation);
                constraints.ApplyRotationConstraints(ref targetTransform, false, IsNearManipulation());
            }
            if (twoHandedManipulationType.HasFlag(TransformFlags.Move))
            {
                MixedRealityPose pose = GetPointersPose();
                targetTransform.Position = moveLogic.Update(pose, targetTransform.Rotation, targetTransform.Scale, true);
                constraints.ApplyTranslationConstraints(ref targetTransform, false, IsNearManipulation());
            }

            ApplyTargetTransform(targetTransform);
        }

        private void HandleOneHandMoveStarted()
        {
            Assert.IsTrue(pointerIdToPointerMap.Count == 1);
            PointerData pointerData = GetFirstPointer();
            IMixedRealityPointer pointer = pointerData.pointer;

            // Calculate relative transform from object to grip.
            Quaternion gripRotation;
            TryGetGripRotation(pointer, out gripRotation);
            Quaternion worldToGripRotation = Quaternion.Inverse(gripRotation);
            objectToGripRotation = worldToGripRotation * HostTransform.rotation;

            MixedRealityPose pointerPose = new MixedRealityPose(pointer.Position, pointer.Rotation);
            MixedRealityPose hostPose = new MixedRealityPose(HostTransform.position, HostTransform.rotation);
            moveLogic.Setup(pointerPose, pointerData.GrabPoint, hostPose, HostTransform.localScale);
        }

      
        private void HandleOneHandMoveUpdated()
        {
            Debug.Assert(pointerIdToPointerMap.Count == 1);
            PointerData pointerData = GetFirstPointer();
            IMixedRealityPointer pointer = pointerData.pointer;

            var targetTransform = new MixedRealityTransform(HostTransform.position, HostTransform.rotation, HostTransform.localScale);

            constraints.ApplyScaleConstraints(ref targetTransform, true, IsNearManipulation());

            Quaternion gripRotation;
            TryGetGripRotation(pointer, out gripRotation);
            targetTransform.Rotation = gripRotation * objectToGripRotation;

            constraints.ApplyRotationConstraints(ref targetTransform, true, IsNearManipulation());

            RotateInOneHandType rotateInOneHandType = isNearManipulation ? oneHandRotationModeNear : oneHandRotationModeFar;
            MixedRealityPose pointerPose = new MixedRealityPose(pointer.Position, pointer.Rotation);
            targetTransform.Position = moveLogic.Update(pointerPose, targetTransform.Rotation, targetTransform.Scale, rotateInOneHandType != RotateInOneHandType.RotateAboutObjectCenter);
        
            if (enableZAxisOffset) // if z axis move enabled, 
            {
                float distance = Vector3.Distance(targetTransform.Position, pointer.Position);

                float minOffsetDistance = -distance + targetTransform.Scale.z;
                float maxOffsetDistance = maxZaxisDistanceMove > distance ? maxZaxisDistanceMove - distance: 0.0f;
                manipulationZOffset = Mathf.Clamp(manipulationZOffset, minOffsetDistance, maxOffsetDistance);

                Vector3 offsetVector = pointer.Rotation * Vector3.forward * manipulationZOffset;
                targetTransform.Position = targetTransform.Position + offsetVector; 
            } 

            constraints.ApplyTranslationConstraints(ref targetTransform, true, IsNearManipulation());

            ApplyTargetTransform(targetTransform);
        }

        private void HandleManipulationStarted()
        {
            isManipulationStarted = true;
            isNearManipulation = IsNearManipulation();
            // TODO: If we are on HoloLens 1, push and pop modal input handler so that we can use old
            // gaze/gesture/voice manipulation. For HoloLens 2, we don't want to do this.
            if (OnManipulationStarted != null)
            {
                OnManipulationStarted.Invoke(new ManipulationEventData
                {
                    ManipulationSource = gameObject,
                    IsNearInteraction = isNearManipulation,
                    Pointer = GetFirstPointer().pointer,
                    PointerCentroid = GetPointersGrabPoint(),
                    PointerVelocity = GetPointersVelocity(),
                    PointerAngularVelocity = GetPointersAngularVelocity()
                });
            }

            if (rigidbody != null && UseRigidBody)
            {
                wasKinematic = rigidbody.isKinematic;
                rigidbody.isKinematic = false;
            }

            constraints.Initialize(new MixedRealityPose(HostTransform.position, HostTransform.rotation));
        }

        private void HandleManipulationEnded(Vector3 pointerGrabPoint, Vector3 pointerVelocity, Vector3 pointerAnglularVelocity)
        {
            isManipulationStarted = false;
            // TODO: If we are on HoloLens 1, push and pop modal input handler so that we can use old
            // gaze/gesture/voice manipulation. For HoloLens 2, we don't want to do this.
            if (OnManipulationEnded != null)
            {
                OnManipulationEnded.Invoke(new ManipulationEventData
                {
                    ManipulationSource = gameObject,
                    IsNearInteraction = isNearManipulation,
                    PointerCentroid = pointerGrabPoint,
                    PointerVelocity = pointerVelocity,
                    PointerAngularVelocity = pointerAnglularVelocity
                });
            }

            ReleaseRigidBody(pointerVelocity, pointerAnglularVelocity);
        }

        #endregion Private Event Handlers

        #region Unused Event Handlers
        /// <inheritdoc />
        public void OnPointerClicked(MixedRealityPointerEventData eventData) { }
        public void OnBeforeFocusChange(FocusEventData eventData) { }

        #endregion Unused Event Handlers

        #region Private methods

        private void ApplyTargetTransform(MixedRealityTransform targetTransform)
        {
            if (!UseRigidBody || rigidbody == null)
            {
                HostTransform.position = smoothingActive ? Smoothing.SmoothTo(HostTransform.position, targetTransform.Position, moveLerpTime, Time.deltaTime) : targetTransform.Position;
                HostTransform.rotation = smoothingActive ? Smoothing.SmoothTo(HostTransform.rotation, targetTransform.Rotation, rotateLerpTime, Time.deltaTime) : targetTransform.Rotation;
                HostTransform.localScale = smoothingActive ? Smoothing.SmoothTo(HostTransform.localScale, targetTransform.Scale, scaleLerpTime, Time.deltaTime) : targetTransform.Scale;
            }
            else
            {
                rigidbody.velocity = ((1f - Mathf.Pow(moveLerpTime, Time.deltaTime)) / Time.deltaTime) * (targetTransform.Position - HostTransform.position);

                var relativeRotation = targetTransform.Rotation * Quaternion.Inverse(HostTransform.rotation);
                relativeRotation.ToAngleAxis(out float angle, out Vector3 axis);
                if (angle > 180f)
                    angle -= 360f;
                if (axis.IsValidVector())
                {
                    rigidbody.angularVelocity = ((1f - Mathf.Pow(rotateLerpTime, Time.deltaTime)) / Time.deltaTime) * (axis.normalized * angle * Mathf.Deg2Rad);
                }

                HostTransform.localScale = smoothingActive ? Smoothing.SmoothTo(HostTransform.localScale, targetTransform.Scale, scaleLerpTime, Time.deltaTime) : targetTransform.Scale;
            }

            UpdateVelocity(HostTransform.position);
        }

        protected void UpdateVelocity(Vector3 position) // code from basehand
        {
            if (frameOn < velocityUpdateInterval)
            {
                velocityPositionsCache[frameOn] = position;
                velocityPositionsSum += velocityPositionsCache[frameOn];
            }
            else
            {
                int frameIndex = frameOn % velocityUpdateInterval;
                float deltaTime = Time.unscaledTime - deltaTimeStart;
                Vector3 newPosition = position;
                Vector3 newPositionsSum = velocityPositionsSum - velocityPositionsCache[frameIndex] + newPosition;
                objectVelocity = (newPositionsSum - velocityPositionsSum) / deltaTime / velocityUpdateInterval;
                velocityPositionsCache[frameIndex] = newPosition;
                velocityPositionsSum = newPositionsSum;
            }

            deltaTimeStart = Time.unscaledTime;
            frameOn++;
        }

        private Vector3[] GetHandPositionArray()
        {
            var handPositionMap = new Vector3[pointerIdToPointerMap.Count];
            int index = 0;
            foreach (var item in pointerIdToPointerMap)
            {
                handPositionMap[index++] = item.Value.pointer.Position;
            }
            return handPositionMap;
        }

        public void OnFocusChanged(FocusEventData eventData)
        {
            bool isFar = !(eventData.Pointer is IMixedRealityNearPointer);
            if (isFar && !AllowFarManipulation)
            {
                return;
            }

            if (eventData.OldFocusedObject == null ||
                !eventData.OldFocusedObject.transform.IsChildOf(transform))
            {
                if (OnHoverEntered != null)
                {
                    OnHoverEntered.Invoke(new ManipulationEventData
                    {
                        ManipulationSource = gameObject,
                        Pointer = eventData.Pointer,
                        IsNearInteraction = !isFar
                    });
                }
            }
            else if (eventData.NewFocusedObject == null ||
                    !eventData.NewFocusedObject.transform.IsChildOf(transform))
            {
                if (OnHoverExited != null)
                {
                    OnHoverExited.Invoke(new ManipulationEventData
                    {
                        ManipulationSource = gameObject,
                        Pointer = eventData.Pointer,
                        IsNearInteraction = !isFar
                    });
                }
            }
        }

        private void ReleaseRigidBody(Vector3 velocity, Vector3 angularVelocity)
        {
            if (rigidbody != null && UseRigidBody)
            {
                rigidbody.isKinematic = wasKinematic;

                if (releaseBehavior.HasFlag(ReleaseBehaviorType.KeepVelocity))
                {
                    Vector3 finalVelocity = useObjectVelocity?objectVelocity: velocity;
                    finalVelocity = finalVelocity * keepVelocityMutliplier;
                    rigidbody.velocity = finalVelocity;
                }

                if (releaseBehavior.HasFlag(ReleaseBehaviorType.KeepAngularVelocity))
                {
                    rigidbody.angularVelocity = angularVelocity;
                }
            }
        }

        private PointerData GetFirstPointer()
        {
            // We may be able to do this without allocating memory.
            // Moving to a method for later investigation.
            return pointerIdToPointerMap.Values.First();
        }

        private bool TryGetGripRotation(IMixedRealityPointer pointer, out Quaternion rotation)
        {
            for (int i = 0; i < pointer.Controller.Interactions.Length; i++)
            {
                if (pointer.Controller.Interactions[i].InputType == DeviceInputType.SpatialGrip)
                {
                    rotation = pointer.Controller.Interactions[i].RotationData;
                    return true;
                }
            }
            rotation = Quaternion.identity;
            return false;
        }

        #endregion
    }
}
