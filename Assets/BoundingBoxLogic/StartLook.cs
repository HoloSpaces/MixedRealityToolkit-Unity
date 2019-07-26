using UnityEditor;
using UnityEngine;

public class StartLook : MonoBehaviour
{
    private enum TestCase
    {
        MotionController,
        Mouse,
        Hand
    }

    public Transform cube;
    public Transform rotator;
    public Transform hand;
    public Transform beamTip;

    [SerializeField] private TestCase testCase = default(TestCase);
    [SerializeField] private float handSpeed = 2f;

    Vector3 handLocalStart;
    Vector3 cubeUp;
    Vector3 cubeForward;
    Vector3 lookPoint;
    float initialLookPointDistance;
    Quaternion inititalCubeRotation;
    Vector3 tipStartPosition;
    float controllerStartOffset = 0f;

    //float startOffset;
    float offsetScale;
    float initialOrthogonalExtend;
    float initialControllerTilt;
    float initialControllerToCubeAngle;
    Quaternion initialControllerToHandleRotation;
    //float initialControllerOrthogonalExtend;

    // Start is called before the first frame update
    void Start()
    {
        lookPoint = rotator.transform.position;
        initialLookPointDistance = Vector3.Distance(lookPoint, cube.position);
        inititalCubeRotation = cube.rotation;
        tipStartPosition = beamTip.position;
        cubeUp = cube.up;
        offsetScale = 1f / Vector3.Distance(cube.position, lookPoint);
        Vector3 flattendControllerInCube = cube.InverseTransformPoint(transform.position);
        flattendControllerInCube.y = 0;
        initialControllerToHandleRotation = Quaternion.FromToRotation(rotator.localPosition, flattendControllerInCube);
        //Debug.Log(initialControllerToHandleRotation.eulerAngles);

        //Debug.DrawRay(cube.transform.position, cube.InverseTransformDirection(flattendControllerInCube));
        //Debug.DrawRay(cube.transform.position, cube.InverseTransformDirection(rotator.localPosition));
        //Debug.Break();
        switch (testCase)
        {
            case TestCase.Hand:
                handLocalStart = hand.localPosition;
                break;
            default:
                transform.LookAt(lookPoint, cube.up);
                Vector3 controllerToCube = Vector3.ProjectOnPlane(cube.position - transform.position, cubeUp);
                Vector3 controllerForward = Vector3.ProjectOnPlane(transform.forward, cubeUp);

                initialControllerTilt = Vector3.SignedAngle(controllerToCube, controllerForward, cubeUp);
                initialOrthogonalExtend = Mathf.Tan(initialControllerTilt * Mathf.Deg2Rad) * controllerToCube.magnitude * offsetScale;
                cubeForward = cube.forward;
                initialControllerToCubeAngle = Vector3.SignedAngle(controllerToCube, cubeForward, cubeUp);
                //initialControllerOrthogonalExtend = Mathf.Tan(initialControllerToCubeAngle * Mathf.Deg2Rad) * .5f;

                break;
        }

    }

    private void Update()
    {
        switch (testCase)
        {
            case TestCase.MotionController:
                //UpdateControllerBehaviour();
                UpdateControllerBehaviour2();
                break;
            case TestCase.Mouse:
                UpdateMouseBehaviour();
                break;
            case TestCase.Hand:
                UpdateHandBehaviour();
                break;
        }
    }

    // TODO
    private void UpdateHandBehaviour()
    {
        Transform cameraTransform = Camera.main.transform;
        Vector3 handProjection = hand.localPosition - handLocalStart;

        Vector3 handWorldMovement = cameraTransform.TransformDirection(handProjection);

        Vector3 cubeToCamera = Vector3.ProjectOnPlane(cube.position - cameraTransform.position, cubeUp);

        Vector3 right = Vector3.Cross(cubeToCamera.normalized, cubeUp);
        float extend = Vector3.Dot(handWorldMovement, right);

        float result = (((int)extend * 90f + Mathf.Asin(extend % 1f) * Mathf.Rad2Deg) * handSpeed);
        cube.rotation = Quaternion.AngleAxis(result, cubeUp) * inititalCubeRotation;
    }

    private void UpdateMouseBehaviour()
    { 
        Vector3 cubeToCamera = Vector3.ProjectOnPlane(cube.position - Camera.main.transform.position, cubeUp);
        Vector3 mouseForward = Vector3.ProjectOnPlane(transform.position - Camera.main.transform.position, cubeUp);

        if (Vector3.Dot(cubeToCamera, mouseForward) <= 0f)
        {
            return;
        }

        float angleAxis = Vector3.SignedAngle(cubeToCamera, mouseForward, cubeUp);

        float offsetLength = Mathf.Sin(angleAxis * Mathf.Deg2Rad) * cubeToCamera.magnitude;

        float angleToControllerAngle = Vector3.SignedAngle(cubeToCamera, Camera.main.transform.forward, cubeUp);

        float downScaled = offsetLength * offsetScale;

        cube.rotation = Quaternion.LookRotation(rotator.localPosition, cubeUp) * Quaternion.AngleAxis((int)downScaled * -90f - Mathf.Asin(downScaled % 1f) * Mathf.Rad2Deg - angleToControllerAngle, cubeUp);
    }


    private void UpdateControllerBehaviour()
    {
        //transform.LookAt(rotator.transform.position, cube.up);

        Vector3 controllerToCube = Vector3.ProjectOnPlane(cube.position - transform.position, cubeUp);
        Vector3 controllerForward = Vector3.ProjectOnPlane(transform.forward, cubeUp);
        Vector3 initialControllerToCubeDirection = Vector3.ProjectOnPlane(cube.position - tipStartPosition, cubeUp);

        if (Vector3.Dot(controllerToCube, controllerForward) <= 0f)
        {
            return;
        }

        //Debug.DrawRay(transform.position, controllerToCube);
        //Debug.DrawRay(transform.position, controllerForward * controllerToCube.magnitude);

        float controllerTiltAngle = Vector3.SignedAngle(controllerToCube, controllerForward, cubeUp);

        //Debug.DrawRay(cube.position, initialControllerToCubeDirection, Color.yellow);
        //Debug.DrawRay(cube.position, cubeStartRotation * Vector3.forward * 5f, Color.magenta);


        float currentOffsetAngle = Vector3.SignedAngle(Quaternion.LookRotation(rotator.localPosition, cubeUp) * inititalCubeRotation * Vector3.forward, initialControllerToCubeDirection, cubeUp);

        float offsetLength = Mathf.Sin(controllerTiltAngle * Mathf.Deg2Rad) * controllerToCube.magnitude * offsetScale;

        float full90s = (int)offsetLength * 90;
        float partial90 = Mathf.Asin(offsetLength % 1f) * Mathf.Rad2Deg;

        Debug.Log($"{offsetLength} {full90s} {partial90} {currentOffsetAngle} {controllerStartOffset}");

        cube.rotation = Quaternion.AngleAxis((full90s + partial90 + currentOffsetAngle + controllerStartOffset) * Mathf.Sign(Vector3.Cross(Camera.main.transform.right, cubeUp).z), cubeUp) * inititalCubeRotation;
    }

    int otherSideOfBox = -1;
    float correctAngle;
    float controllerTiltAngle;
    Vector3 controllerToCube;
    private void UpdateControllerBehaviour2()
    {
        //transform.LookAt(cube.position, cube.up);

        controllerToCube = cube.position - transform.position;
        Vector3 crossRight = Vector3.Cross(cubeUp, controllerToCube.normalized);
        Vector3 crossUp = Vector3.Cross(controllerToCube.normalized, crossRight);

        //controllerToCube = Vector3.ProjectOnPlane(controllerToCube, cubeUp);
        Vector3 controllerForward = Vector3.ProjectOnPlane(transform.forward, crossUp);

        Debug.DrawRay(cube.position - crossRight * 2.5f - crossUp * 2.5f, crossRight * 5f);
        Debug.DrawRay(cube.position - crossRight * 2.5f - crossUp * 2.5f, crossUp * 5f);
        Debug.DrawRay(cube.position - crossRight * 2.5f - crossUp * 2.5f + crossRight * 5f, crossUp * 5f);
        Debug.DrawRay(cube.position - crossRight * 2.5f - crossUp * 2.5f + crossUp * 5f, crossRight * 5f);

        Debug.DrawRay(transform.position, controllerToCube);
        Debug.DrawRay(transform.position, controllerForward);

        controllerTiltAngle = Vector3.SignedAngle(controllerToCube, controllerForward, crossUp);
        float controllerToCubeAngle = Vector3.SignedAngle(Vector3.ProjectOnPlane(controllerToCube, cubeUp), cubeForward, cubeUp);

        int upsideDown = Vector3.Dot(Vector3.up, cubeUp) >= 0 ? 1 : -1;

        Vector3 cubeToBeam = Vector3.ProjectOnPlane(beamTip.position - cube.position, cubeUp);
        Vector3 cubeToBeamTited = Vector3.ProjectOnPlane(beamTip.position - cube.position, crossUp);

        float orthogonalExtend = Mathf.Tan(controllerTiltAngle * Mathf.Deg2Rad) * controllerToCube.magnitude * offsetScale;

        bool InCircleRange = CalculateVectorCircleContactAngle(controllerTiltAngle, controllerToCube.magnitude, .5f, out correctAngle);

        Quaternion rotationModel;

        if (InCircleRange)
            otherSideOfBox = transform.InverseTransformDirection(cubeToBeam).z >= 0 ? 1 : -1;

        if (InCircleRange && cubeToBeamTited.magnitude < initialLookPointDistance)
        {
            //otherSideOfBox = transform.InverseTransformDirection(cubeToBeam).z >= 0 ? 1 : -1;
            rotationModel = Quaternion.LookRotation(-cubeToBeam, cubeUp);// * inititalCubeRotation;
            Debug.Log($"{cubeToBeam} {rotator.localPosition} {Quaternion.LookRotation(rotator.localPosition, cubeUp)} {inititalCubeRotation} {rotationModel}");
        }
        else
        {
            //orthogonalExtend -= initialOrthogonalExtend;
            float full90s = (int)orthogonalExtend * 90;
            float partial90 = correctAngle;
            float frontAngle = full90s + partial90;

            //float floatInitialFrontAngle = (int)initialOrthogonalExtend * 90 + Mathf.Asin(initialOrthogonalExtend % 1f) * Mathf.Rad2Deg;
            //float initialcontrollerToCube = (int)initialControllerOrthogonalExtend * 90 + Mathf.Asin(initialControllerOrthogonalExtend % 1f) * Mathf.Rad2Deg;

            float sideAngle = frontAngle - otherSideOfBox * controllerToCubeAngle - initialControllerToCubeAngle;// - initialControllerToCubeAngle;// + (controllerToCubeAngle - initialControllerToCubeAngle);
            Debug.Log($"{correctAngle} {orthogonalExtend} {frontAngle} {controllerToCubeAngle} {initialControllerToCubeAngle} {initialControllerToHandleRotation.eulerAngles} {otherSideOfBox}");
            sideAngle += otherSideOfBox == 1f ? 180f : 0f;
            rotationModel = Quaternion.AngleAxis(correctAngle * otherSideOfBox, cubeUp) * inititalCubeRotation * initialControllerToHandleRotation;// * inititalCubeRotation;
        }

        cube.rotation = rotationModel;
    }

    private bool CalculateVectorCircleContactAngle(float outerAngle, float hypotenuse, float opposite, out float resultAngle)
    {
        float sin = Mathf.Sin(outerAngle * Mathf.Deg2Rad) * hypotenuse / opposite;
        if (Mathf.Abs(sin) > 1)
        {
            resultAngle = 90f;
            return false;
        }
        resultAngle = Mathf.Asin(sin) * Mathf.Rad2Deg - outerAngle;
        return true;
    }

    private void OnDrawGizmos()
    {
        Handles.DrawWireDisc(cube.position, cubeUp, .5f);
        Handles.DrawLine(transform.position, cube.position - transform.position);
        Handles.DrawSolidArc(transform.position, cubeUp, cube.position - transform.position, controllerTiltAngle, .3f);
        Handles.DrawLine(transform.position, transform.position + Quaternion.Euler(0, controllerTiltAngle, 0) * Vector3.forward * controllerToCube.magnitude);

        float correctExtend;
        if (!CalculateVectorCircleContactAngle(controllerTiltAngle, controllerToCube.magnitude, .5f, out correctExtend))
            return;
        Handles.Label(Vector3.up * .1f, correctExtend.ToString());
        Handles.Label(Vector3.up * .3f, (180f - correctExtend).ToString());
        Handles.DrawSolidArc(Vector3.zero, Vector3.up, Vector3.back, -(correctExtend - controllerTiltAngle), .3f);
        Handles.DrawLine(Vector3.zero, Quaternion.Euler(0, -(correctExtend - controllerTiltAngle), 0) * Vector3.back * .5f);
    }
}
