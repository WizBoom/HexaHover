using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Hovercraft))]
public class HovercraftMovement : MonoBehaviour 
{
    [Range(0.0f, 30.0f)]
    public float ForwardThrust = 2.0f;
    [Range(0.0f, 300.0f)]
    public float KeyboardTurnRate = 1.0f;
    [Range(0.0f, 12.0f)]
    public float MaxControllerTurnRate = 0.5f;
    [Range(0.0f, 10.0f)]
    public float BoostImpulse = 1.0f;

    private Rigidbody _rb;
    private Hovercraft _hovercraft;

    private bool _usingController;
    private bool _rotationLocked = true; // Gets unlocked when raycast down fails

    [Range(0.0f, 3.0f)]
    public float BoostCooldownValue = 0.8f;
    private float _boostCooldownRemaining = 0.0f; // If larger than zero, we're still cooling down and therefore can't boost again yet

    private void Start()
    {
        _hovercraft = GetComponent<Hovercraft>();
        _usingController = (_hovercraft.PlayerNumber > 1);

        _rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        float maxRaycastDistance = 1.0f;
        _rotationLocked = Physics.Raycast(transform.position, Vector3.down, maxRaycastDistance);

        if (_rotationLocked)
        {
            LockXZRotation();
        }

        /*
            Since controller input is so different from keyboard input, handle them seperately.
            Controller left stick controls ship rotation directly, 
            whereas keyboard A & D rotate the ship
        */
        if (_usingController)
        {
            float controllerHorizontal = Input.GetAxis("Controller" + _hovercraft.PlayerNumber + "_Horizontal_L");
            float controllerVertical = Input.GetAxis("Controller" + _hovercraft.PlayerNumber + "_Vertical_L");

            float deadzone = 0.5f;
            if (new Vector2(controllerHorizontal, controllerVertical).magnitude < deadzone)
            {
                controllerHorizontal = 0.0f;
                controllerVertical = 0.0f;
            }

            // NOTE: Only use controller input if the stick is past a certain point
            float controllerThreshold = 0.8f;
            if (Mathf.Abs(controllerHorizontal) >= controllerThreshold || Mathf.Abs(controllerVertical) >= controllerThreshold)
            {
                // Reduce spin induced by other player if this player is inputing rotation
                _rb.angularVelocity *= 0.01f;

                Camera camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
                float controllerStickDirDeg = Mathf.Rad2Deg * (Mathf.Atan2(controllerVertical, controllerHorizontal) + (Mathf.PI / 2.0f));
                Quaternion desiredRotation = Quaternion.Euler(0.0f, controllerStickDirDeg, 0.0f) * camera.transform.rotation;
                transform.rotation = Quaternion.RotateTowards(transform.rotation, desiredRotation, MaxControllerTurnRate);
            }
        }
        else // Not using controller; using keyboard
        {
            bool keyboardLeft = Input.GetButton("Keyboard" + _hovercraft.PlayerNumber + "_Left");
            bool keyboardRight = Input.GetButton("Keyboard" + _hovercraft.PlayerNumber + "_Right");

            if (keyboardLeft || keyboardRight)
            {
                //float desiredRotation = keyboardRight ? 90.0f : -90.0f;
                float rotationAmount = KeyboardTurnRate * Time.fixedDeltaTime;
                transform.rotation *= Quaternion.Euler(0.0f, keyboardRight ? rotationAmount : -rotationAmount, 0.0f);
            }
        }

        // Forward/backward movement
        float forwardThrustInput = GetThrustInput();
        float forwardThrust = forwardThrustInput * ForwardThrust;

        Vector3 velDirNormalized = forwardThrustInput > 0.0f ? transform.forward : _rb.velocity.normalized;
        Vector3 thrust = (velDirNormalized * forwardThrust);

        _rb.AddForce(thrust, ForceMode.Acceleration);

        // Boosting
        _boostCooldownRemaining = Mathf.Max(_boostCooldownRemaining - Time.fixedDeltaTime, 0.0f);
        if (_boostCooldownRemaining == 0.0f)
        {
            bool boostInput;
            if (_usingController) boostInput = Input.GetButtonDown("Controller" + _hovercraft.PlayerNumber + "_Boost");
            else boostInput = Input.GetButton("Keyboard" + _hovercraft.PlayerNumber + "_Boost");

            if (boostInput)
            {
                _rb.AddForce(transform.forward * BoostImpulse, ForceMode.Impulse);
                _boostCooldownRemaining = BoostCooldownValue;
                GetComponent<Visuals_HoverCraft>().PlayBoostSound();
            }
        }
    }

    private void LockXZRotation()
    {
        transform.eulerAngles = new Vector3(0.0f, transform.eulerAngles.y, 0.0f);
    }

    private void OnGUI()
    {
       //GUILayout.Space(75 * _hovercraft.PlayerNumber);
       //GUI.color = new Color(0.9f, 0.2f, 0.2f);
       //GUILayout.Label("Player " + _hovercraft.PlayerNumber + ":");
       //GUI.color = Color.white;
       //GUILayout.Label("   using " + (_usingController ? "controller" : "keyboard"));
       //GUILayout.Label("   rotation " + (_rotationLocked? "locked" : "unlocked"));
    }

    public float GetThrustInput()
    {
        if (_usingController)
        {
            return Input.GetAxis("Controller" + _hovercraft.PlayerNumber + "_Thrust");
        }
        else
        {
            return Input.GetButton("Keyboard" + _hovercraft.PlayerNumber + "_Thrust") ? 1.0f : 0.0f;
        }
    }
}