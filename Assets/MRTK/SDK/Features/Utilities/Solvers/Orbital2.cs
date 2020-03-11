﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities.Solvers
{
    /// <summary>
    /// Provides a solver that follows the TrackedObject/TargetTransform in an orbital motion.
    /// </summary>
    [RequireComponent(typeof(ControllerFinderOrbital2))]
    public class Orbital2 : Solver
    {
        [SerializeField]
        [Tooltip("The desired object reference type. Can be the tracked object, any scene object, or a body part")]
        private ReferenceObjectType referenceObjectType = default;

        /// <summary>
        /// The desired object reference
        /// </summary>
        public ReferenceObjectType ReferenceObjectType
        {
            get { return referenceObjectType; }
            set
            {
                if (referenceObjectType != value)
                {
                    referenceObjectType = value;
                    TrackNewFaceTarget();
                }
            }
        }

        [SerializeField]
        [Tooltip("Tracked object to calculate facing direction for. If you want to manually override and use a scene object, use the FaceTarget field.")]
        private TrackedObjectType trackedObjectToFace = default;

        /// <summary>
        /// Tracked object to calculate position and orientation from. If you want to manually override and use a scene object, use the TransformTarget field.
        /// </summary>
        public TrackedObjectType TrackedObjectToFace
        {
            get { return trackedObjectToFace; }
            set
            {
                if (trackedObjectToFace != value)
                {
                    trackedObjectToFace = value;
                    TrackNewFaceTarget();
                }
            }
        }

        [SerializeField]
        [Tooltip("Manual override for FacedObjectToReference if you want to use a scene object. Leave empty if you want to use head, motion-tracked controllers, or motion-tracked hands.")]
        private Transform faceTarget;

        public Transform FaceTarget
        {
            get { return faceTarget; }
            set
            {
                if (faceTarget != value)
                {
                    faceTarget = value;

                    // faceTarget might be one of the body parts, but that's supposed to be happening by assigning ReferenceObjectType.BodyPart and therefore unlikely
                    if (faceTarget == SolverHandler.TransformTarget)
                    {
                        referenceObjectType = ReferenceObjectType.TrackedObject;
                    }
                    else
                    {
                        referenceObjectType = ReferenceObjectType.SceneObject;
                    }
                }
            }
        }

        [SerializeField]
        [Tooltip("Sets an additional distance from the TargetTransform to orbit around it")]
        private float orbitDistance;

        /// <summary>
        /// Sets an additional distance from the TargetTransform to orbit around it
        /// </summary>
        public float OrbitDistance
        {
            get { return orbitDistance; }
            set { orbitDistance = value; }
        }

        [SerializeField]
        [Tooltip("The constraint on the view rotation")]
        private PivotAxis pivotAxis = PivotAxis.Free;

        /// <summary>
        /// The desired axis constraint.
        /// </summary>
        /// <remarks>
        /// Default leaves the object capable of facing the target on all axis.
        /// </remarks>
        public PivotAxis PivotAxis
        {
            get { return pivotAxis; }
            set { pivotAxis = value; }
        }

        [SerializeField]
        [Tooltip("The space in which the XYZ offset is used")]
        private TransformationSpaceType offsetSpace = default;

        /// <summary>
        /// The space in which the XYZ offset is used
        /// </summary>
        public TransformationSpaceType OffsetSpace
        {
            get { return offsetSpace; }
            set { offsetSpace = value; }
        }

        [SerializeField]
        [Tooltip("XYZ offset for this object in relation to the TrackedObject/TargetTransform")]
        private Vector3 offset = default;

        /// <summary>
        /// XYZ offset for this object in relation to the TrackedObject/TargetTransform.
        /// </summary>
        public Vector3 Offset
        {
            get { return offset; }
            set { offset = value; }
        }

        [SerializeField]
        [Tooltip("Lock the rotation to a specified number of steps around the tracked object.")]
        private bool useAngleStepping = false;

        /// <summary>
        /// Lock the rotation to a specified number of steps around the tracked object.
        /// </summary>
        public bool UseAngleStepping
        {
            get { return useAngleStepping; }
            set { useAngleStepping = value; }
        }

        [Range(2, 24)]
        [SerializeField]
        [Tooltip("The division of steps this object can tether to. Higher the number, the more snapple steps.")]
        private int tetherAngleSteps = 6;

        /// <summary>
        /// The division of steps this object can tether to. Higher the number, the more snapple steps.
        /// </summary>
        public int TetherAngleSteps
        {
            get { return tetherAngleSteps; }
            set
            {
                tetherAngleSteps = Mathf.Clamp(value, 2, 24);
            }
        }

        private ControllerFinderOrbital2 controllerFinder;
        private Transform transformTarget;
        private bool useFaceTargetUp;

        private bool LookAtFaceTarget => referenceObjectType != ReferenceObjectType.TrackedObject && faceTarget != null && faceTarget != SolverHandler.TransformTarget;

        protected override void Awake()
        {
            base.Awake();
            transform.parent = null;
            controllerFinder = GetComponent<ControllerFinderOrbital2>();
        }

        protected override void Start()
        {
            base.Start();
            TrackNewFaceTarget();
        }

        public override void SolverUpdate()
        {
            transformTarget = SolverHandler.TransformTarget;

            if (transformTarget == null && faceTarget == null)
            {
                SetGoalPositionAndRotation(Vector3.zero, Quaternion.identity);
                return;
            }

            Vector3 desiredPos = transformTarget != null ? transformTarget.position : Vector3.zero;
            Vector3 lookDirection;
            Vector3 up;

            if (LookAtFaceTarget)
            {
                lookDirection = offsetSpace == TransformationSpaceType.LocalSpace ? faceTarget.TransformPoint(offset) : faceTarget.position + offset;
                lookDirection -= desiredPos;
                up = faceTarget.up;
            }
            else if (transformTarget != null)
            {
                lookDirection = transformTarget.forward;
                up = transformTarget.up;
            }
            else
            {
                lookDirection = Vector3.forward;
                up = Vector3.up;
            }

            ModifyPositionAndRotation(desiredPos, lookDirection, up);

        }

        private void ModifyPositionAndRotation(Vector3 targetPosition, Vector3 lookDirection, Vector3 up)
        {
            // constrain it
            Quaternion desiredRot = CalculateRotationConstraint(lookDirection, up);
            // tether angle it
            if (pivotAxis == PivotAxis.Y)
                desiredRot = SnapToTetherAngleSteps(desiredRot);
            // apply it

            SetGoalPositionAndRotation(targetPosition, desiredRot);
        }

        private void SetGoalPositionAndRotation(Vector3 goalPosition, Quaternion goalRotation)
        {
            GoalPosition = goalPosition;
            GoalRotation = goalRotation;

            UpdateWorkingPositionToGoal();
            UpdateWorkingRotationToGoal();
        }

        public void TrackNewFaceTarget()
        {
            transformTarget = SolverHandler.TransformTarget;

            switch (referenceObjectType)
            {
                case ReferenceObjectType.TrackedObject:
                    faceTarget = transformTarget;
                    break;
                case ReferenceObjectType.BodyPart:

                    if (transformTarget == null && trackedObjectToFace == SolverHandler.TrackedTargetType)
                    {
                        faceTarget = transformTarget;
                        break;
                    }

                    switch (trackedObjectToFace)
                    {
                        case TrackedObjectType.Head:
                            faceTarget = CameraCache.Main.transform;
                            break;

                        case TrackedObjectType.HandJoint:
                            // Set to None, so the underlying ControllerFinder doesn't attach to a controller.
                            // TODO: Make this more generic / configurable for hands vs controllers. Also resolve the duplicate Handedness variables.
                            switch (SolverHandler.CurrentTrackedHandedness)
                            {
                                case Handedness.Left:
                                    controllerFinder.GetControllerTransform(Handedness.None);
                                    faceTarget = controllerFinder.GetControllerTransform(Handedness.Left);
                                    break;

                                case Handedness.Right:
                                    controllerFinder.GetControllerTransform(Handedness.None);
                                    faceTarget = controllerFinder.GetControllerTransform(Handedness.Right);
                                    break;
                            }
                            break;
                    }
                    break;
            }
        }

        private Quaternion CalculateRotationConstraint(Vector3 directionToTarget, Vector3 up)
        {
            useFaceTargetUp = true;

            // Adjust for the pivot axis.
            switch (pivotAxis)
            {
                case PivotAxis.X:
                    directionToTarget.x = 0.0f;
                    useFaceTargetUp = false;
                    break;

                case PivotAxis.Y:
                    directionToTarget.y = 0.0f;
                    useFaceTargetUp = false;
                    break;

                case PivotAxis.Z:
                    directionToTarget.x = 0.0f;
                    directionToTarget.y = 0.0f;
                    break;

                case PivotAxis.XY:
                    useFaceTargetUp = false;
                    break;

                case PivotAxis.XZ:
                    directionToTarget.x = 0.0f;
                    break;

                case PivotAxis.YZ:
                    directionToTarget.y = 0.0f;
                    break;

                case PivotAxis.Free:
                default:
                    // No changes needed.
                    break;
            }

            // If we are right next to the camera the rotation is undefined. 
            if (directionToTarget.sqrMagnitude < 0.001f)
            {
                return Quaternion.identity;
            }

            // Calculate and apply the rotation required to reorient the object
            return useFaceTargetUp ? Quaternion.LookRotation(directionToTarget, up) : Quaternion.LookRotation(directionToTarget);
        }

        private Quaternion SnapToTetherAngleSteps(Quaternion localRotationToSnap)
        {
            if (!UseAngleStepping || !LookAtFaceTarget)
            {
                return localRotationToSnap;
            }

            float stepAngle = 360f / tetherAngleSteps;
            int numberOfSteps = Mathf.RoundToInt(localRotationToSnap.eulerAngles.y / stepAngle);

            float newAngle = stepAngle * numberOfSteps;

            return Quaternion.Euler(0f, newAngle, 0f);
        }
    }
}