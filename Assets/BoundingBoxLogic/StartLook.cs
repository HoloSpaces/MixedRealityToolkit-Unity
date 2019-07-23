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

    [SerializeField] private TestCase testCase = default(TestCase)  ;
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

    // Start is called before the first frame update
    void Start()
    {
        lookPoint = rotator.transform.position;
        initialLookPointDistance = Vector3.Distance(lookPoint, cube.position);
        inititalCubeRotation = cube.rotation;
        tipStartPosition = beamTip.position;
        cubeUp = cube.up;
        offsetScale = 1f / Vector3.Distance(cube.position, lookPoint);


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
                cubeForward = cube.forward;
                initialControllerToCubeAngle = Vector3.SignedAngle(controllerToCube, cubeForward, cubeUp);
                initialOrthogonalExtend = Mathf.Sin(initialControllerTilt * Mathf.Deg2Rad) * controllerToCube.magnitude * offsetScale;
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
    private void UpdateControllerBehaviour2()
    {
        //transform.LookAt(cube.position, cube.up);

        Vector3 controllerToCube = Vector3.ProjectOnPlane(cube.position - transform.position, cubeUp);
        Vector3 controllerForward = Vector3.ProjectOnPlane(transform.forward, cubeUp);

        Debug.DrawRay(transform.position, controllerToCube);
        Debug.DrawRay(transform.position, controllerForward * controllerToCube.magnitude);

        float controllerTiltAngle = Vector3.SignedAngle(controllerToCube, controllerForward, cubeUp);
        float controllerToCubeAngle = Vector3.SignedAngle(controllerToCube, cubeForward, cubeUp);

        float orthogonalExtend = Mathf.Sin(controllerTiltAngle * Mathf.Deg2Rad) * controllerToCube.magnitude * offsetScale;


        float full90s = (int)orthogonalExtend * 90;
        float partial90 = Mathf.Asin(orthogonalExtend % 1f) * Mathf.Rad2Deg;

        int upsideDown = Vector3.Dot(Vector3.up, cubeUp) >= 0 ? 1 : -1;

        Vector3 cubeToBeam = Vector3.ProjectOnPlane(beamTip.position - cube.position, cubeUp);

        Quaternion rotationModel;

        if (Mathf.Abs(orthogonalExtend) < initialLookPointDistance * 2f)
         otherSideOfBox = transform.InverseTransformDirection(cubeToBeam).z >= 0 ? 1 : -1;

        if (Mathf.Abs(orthogonalExtend) < initialLookPointDistance * 2f && cubeToBeam.magnitude < initialLookPointDistance)
        {
            otherSideOfBox = transform.InverseTransformDirection(cubeToBeam).z >= 0 ? 1 : -1;
            rotationModel = Quaternion.LookRotation(cubeToBeam, cube.up) * Quaternion.Inverse(Quaternion.LookRotation(rotator.localPosition, cubeUp)) * inititalCubeRotation;
            Debug.Log($"{cubeToBeam} {rotator.localPosition} {Quaternion.LookRotation(rotator.localPosition, cubeUp)} {inititalCubeRotation} {rotationModel}");
        }
        else
        {
            float initialAngle = Mathf.Asin(initialOrthogonalExtend) * Mathf.Rad2Deg;
            Debug.Log($"{orthogonalExtend < initialLookPointDistance} {full90s} {partial90} {upsideDown} {otherSideOfBox} {initialControllerTilt} {partial90 / initialAngle} {1 - partial90 / initialAngle}");
            rotationModel = Quaternion.AngleAxis((full90s + partial90 - (initialAngle + initialControllerTilt * (1 - partial90 / initialAngle)) + (controllerToCubeAngle - initialControllerToCubeAngle)) * otherSideOfBox, cubeUp) * inititalCubeRotation;
        }



        cube.rotation = rotationModel;
    }
}
