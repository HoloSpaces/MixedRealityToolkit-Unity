using System;
using UnityEngine;

public class StartLook : MonoBehaviour
{
    private enum TestCase
    {
        MotionController = 0,
        Mouse,
        Hand
    }

    public Transform cube;
    public Transform rotator;

    [SerializeField] private TestCase testCase = default(TestCase);

    Vector3 cameraStartPosition;

    //float startOffset;
    float offsetScale;

    // Start is called before the first frame update
    void Start()
    {
        Vector3 lookPoint = rotator.transform.TransformPoint(Vector3.right * .5f);
        transform.LookAt(lookPoint);

        offsetScale = 1f / Vector3.Distance(cube.position, lookPoint);
        cameraStartPosition = Camera.main.transform.position;
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
        Vector3 cubeToCamera = cube.position - cameraStartPosition;
        cubeToCamera.y = 0f;
        Vector3 mouseForward = transform.position - cameraStartPosition;

        transform.rotation = Quaternion.LookRotation(mouseForward, Vector3.up);

        mouseForward.y = 0f;


        float angleAxis = Vector3.SignedAngle(cubeToCamera, mouseForward, cube.up);

        float offsetLength = Mathf.Sin(angleAxis * Mathf.Deg2Rad) * cubeToCamera.magnitude;

        float angleToControllerAngle = Vector3.SignedAngle(cubeToCamera, Vector3.forward, Vector3.up);

        float downScaled = offsetLength * offsetScale;

        cube.rotation = Quaternion.LookRotation(rotator.localPosition, cube.up) * Quaternion.Euler(0, (int)downScaled * -90f - Mathf.Asin(downScaled % 1f) * Mathf.Rad2Deg - angleToControllerAngle, 0);
    }

    private void UpdateMouseBehaviour()
    { 
        Vector3 cubeToCamera = Vector3.ProjectOnPlane(cube.position - Camera.main.transform.position, cube.up);
        Vector3 mouseForward = Vector3.ProjectOnPlane(transform.position - Camera.main.transform.position, cube.up);

        if (Vector3.Dot(cubeToCamera, mouseForward) <= 0f)
        {
            return;
        }

        float angleAxis = Vector3.SignedAngle(cubeToCamera, mouseForward, cube.up);

        float offsetLength = Mathf.Sin(angleAxis * Mathf.Deg2Rad) * cubeToCamera.magnitude;

        float angleToControllerAngle = Vector3.SignedAngle(cubeToCamera, Vector3.forward, Vector3.up);

        float downScaled = offsetLength * offsetScale;

        cube.rotation = Quaternion.LookRotation(rotator.localPosition, cube.up) * Quaternion.Euler(0, (int)downScaled * -90f - Mathf.Asin(downScaled % 1f) * Mathf.Rad2Deg - angleToControllerAngle, 0);
    }

    private void UpdateControllerBehaviour()
    {
        Vector3 cubeToController = Vector3.ProjectOnPlane(cube.position - transform.position, cube.up);
        Vector3 controllerForward = Vector3.ProjectOnPlane(transform.forward, cube.up);

        if (Vector3.Dot(cubeToController, controllerForward) <= 0f)
        {
            return;
        }

        float angleAxis = Vector3.SignedAngle(cubeToController, controllerForward, cube.up);

        float worldAngleToController = Vector3.SignedAngle(cubeToController, Vector3.forward, Vector3.up);

        float offsetLength = Mathf.Sin(angleAxis * Mathf.Deg2Rad) * cubeToController.magnitude * offsetScale;

        Quaternion handleOffsetRotation = Quaternion.LookRotation(rotator.localPosition, cube.up);
        float full90s = (int)offsetLength * 90f;
        float partial90 = Mathf.Asin(offsetLength % 1f) * Mathf.Rad2Deg;

        cube.rotation = handleOffsetRotation * Quaternion.Euler(0, - (full90s - partial90 - worldAngleToController), 0);
    }
}
