using UnityEngine;

[RequireComponent(typeof(Hovercraft))]
public class HovercraftHarpoon : MonoBehaviour
{
    public float AimArrowDistance = 2.0f;
    public GameObject AimArrowGameObject;

    private Hovercraft _hovercraft;

    void Start()
    {
        Vector3 pos = Vector3.forward * AimArrowDistance;

        AimArrowGameObject.transform.position = pos;
        AimArrowGameObject.transform.LookAt(transform.position);

        _hovercraft = GetComponent<Hovercraft>();
    }
    
    void Update ()
    {
        float controllerHorizontal = Input.GetAxis("Controller" + _hovercraft.PlayerNumber + "_Horizontal_R");
        float controllerVertical = Input.GetAxis("Controller" + _hovercraft.PlayerNumber + "_Vertical_R");

        float deadzone = 0.5f;
        if (new Vector2(controllerHorizontal, controllerVertical).magnitude < deadzone)
        {
            controllerHorizontal = 0.0f;
            controllerVertical = 0.0f;
        }

        float controllerThreshold = 0.8f;
        if (Mathf.Abs(controllerHorizontal) >= controllerThreshold || Mathf.Abs(controllerVertical) >= controllerThreshold)
        {
            Camera camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
            float controllerStickDirDeg = Mathf.Atan2(-controllerVertical, controllerHorizontal) + (Mathf.PI / 2.0f);
            Quaternion desiredRotation = Quaternion.Euler(0.0f, controllerStickDirDeg, 0.0f) * camera.transform.rotation;
            float desiredRotationEuler = desiredRotation.eulerAngles.y;

            Vector3 pos = new Vector3();
            pos.x = transform.position.x + (Mathf.Cos(desiredRotationEuler) * AimArrowDistance);
            pos.y = transform.position.y;
            pos.z = transform.position.z +(Mathf.Sin(desiredRotationEuler) * AimArrowDistance);

            AimArrowGameObject.transform.position = pos;
            AimArrowGameObject.transform.LookAt(transform.position);
        }
    }
}
