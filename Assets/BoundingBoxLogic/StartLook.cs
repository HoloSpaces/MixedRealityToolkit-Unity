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

    [SerializeField] private TestCase testCase;
    [SerializeField] private float handSpeed = 2f;
    //[SerializeField] private float testDegree = 15f;

    Vector3 handLocalStart;
    Vector3 cubeUp;
    Quaternion cubeStartRotation;
    float controllerStartAngleOffset;

    //float startOffset;
    float offsetScale;

    // Start is called before the first frame update
    void Start()
    {
        Vector3 lookPoint = rotator.transform.TransformPoint(Vector3.right * .5f);
        cubeStartRotation = cube.rotation;
        cubeUp = cube.up;
        switch (testCase)
        {
            case TestCase.Hand:
                handLocalStart = hand.localPosition;
                break;
            default:
                transform.LookAt(lookPoint);
                Vector3 cubeToController = Vector3.ProjectOnPlane(cube.position - transform.position, cubeUp);
                Vector3 controllerForward = Vector3.ProjectOnPlane(transform.forward, cubeUp);

                if (Vector3.Dot(cubeToController, controllerForward) > 0f)
                {
                    controllerStartAngleOffset = Vector3.SignedAngle(cubeToController, controllerForward, cubeUp);
                }
                
                break;
        }

        offsetScale = 1f / Vector3.Distance(cube.position, lookPoint);
    }

    private void Update()
    {
        switch (testCase)
        {
            case TestCase.MotionController:
                UpdateControllerBehaviour();
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
        cube.rotation = Quaternion.AngleAxis(result, cubeUp) * cubeStartRotation;
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
        Vector3 cubeToController = Vector3.ProjectOnPlane(cube.position - transform.position, cubeUp);
        Vector3 controllerForward = Vector3.ProjectOnPlane(transform.forward, cubeUp);

        if (Vector3.Dot(cubeToController, controllerForward) <= 0f)
        {
            return;
        }

        float angleAxis = Vector3.SignedAngle(cubeToController, controllerForward, cubeUp);

        float worldAngleToController = Vector3.SignedAngle(cubeToController, Camera.main.transform.forward, cubeUp);

        float offsetLength = Mathf.Sin(angleAxis * Mathf.Deg2Rad) * cubeToController.magnitude * offsetScale;

        Quaternion handleOffsetRotation = Quaternion.LookRotation(rotator.localPosition, cubeUp);
        float full90s = (int)offsetLength * 90f;
        float partial90 = Mathf.Asin(offsetLength % 1f) * Mathf.Rad2Deg;

        cube.rotation = handleOffsetRotation * Quaternion.AngleAxis(- (full90s + partial90 + worldAngleToController), cubeUp);
    }
}
