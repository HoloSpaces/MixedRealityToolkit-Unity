using System;
using UnityEngine;

public class StartLook : MonoBehaviour
{
    public Transform cube;
    public Transform rotator;

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
        //UpdateControllerBehaviour();
        UpdateMouseBehaviour();
        UpdateHandBehaviour();
    }

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
        Vector3 cubeToCamera = cube.position - Camera.main.transform.position;
        cubeToCamera.y = 0f;
        Vector3 mouseForward = transform.position - Camera.main.transform.position;

        transform.rotation = Quaternion.LookRotation(mouseForward, Vector3.up);

        mouseForward.y = 0f;


        float angleAxis = Vector3.SignedAngle(cubeToCamera, mouseForward, cube.up);

        float offsetLength = Mathf.Sin(angleAxis * Mathf.Deg2Rad) * cubeToCamera.magnitude;

        float angleToControllerAngle = Vector3.SignedAngle(cubeToCamera, Vector3.forward, Vector3.up);

        float downScaled = offsetLength * offsetScale;

        cube.rotation = Quaternion.LookRotation(rotator.localPosition, cube.up) * Quaternion.Euler(0, (int)downScaled * -90f - Mathf.Asin(downScaled % 1f) * Mathf.Rad2Deg - angleToControllerAngle, 0);
    }

    private void UpdateControllerBehaviour()
    {
        Vector3 cubeToController = cube.position - transform.position;
        cubeToController.y = 0f;
        Vector3 controllerForward = transform.forward;
        controllerForward.y = 0f;
        float angleAxis = Vector3.SignedAngle(cubeToController, controllerForward, cube.up);

        float offsetLength = Mathf.Sin(angleAxis * Mathf.Deg2Rad) * cubeToController.magnitude;

        float angleToControllerAngle = Vector3.SignedAngle(cubeToController, Vector3.forward, Vector3.up);

        float downScaled = offsetLength * offsetScale;

        cube.rotation = Quaternion.LookRotation(rotator.localPosition, cube.up) * Quaternion.Euler(0, (int)downScaled * -90f - Mathf.Asin(downScaled % 1f) * Mathf.Rad2Deg - angleToControllerAngle, 0);
    }
}
