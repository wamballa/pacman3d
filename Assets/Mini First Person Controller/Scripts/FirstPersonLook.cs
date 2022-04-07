using UnityEngine;
using TMPro;

public class FirstPersonLook : MonoBehaviour
{
    public TMP_Text debugAngle;

    [SerializeField]
    Transform character;
    [SerializeField]
    PlayerController playerController;

    public float sensitivity = 2;
    public float smoothing = 1.5f;

    Vector2 velocity;
    Vector2 frameVelocity;

    // Rotations
    Quaternion upDirection = Quaternion.Euler(0, 0, 0);
    Quaternion downDirection = Quaternion.Euler(0, -180, 0);
    Quaternion leftDirection = Quaternion.Euler(0, -90, 0);
    Quaternion rightDirection = Quaternion.Euler(0, 90, 0);

    void Reset()
    {
    }

    void Start()
    {
        // Lock the mouse cursor to the game screen.
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.lockState = CursorLockMode.None;
    }

    void Update()
    {
        // Get smooth velocity.
        Vector2 mouseDelta = new Vector2(Input.GetAxisRaw("Mouse X"), 0);
        Vector2 rawFrameVelocity = Vector2.Scale(mouseDelta, Vector2.one * sensitivity);
        frameVelocity = Vector2.Lerp(frameVelocity, rawFrameVelocity, 1 / smoothing);
        velocity += frameVelocity;

        // Rotate camera left-right from velocity.
        transform.localRotation = Quaternion.AngleAxis(velocity.x, Vector3.up);

        Vector3 targetDir = character.forward;
        Vector3 forward = transform.forward;
        float angle = Vector3.SignedAngle(targetDir, forward, Vector3.up);

        //print("targetDir / forward " + character.localEulerAngles + " " + transform.localEulerAngles);
        //print("delta " + angle);


        bool isPlayerMoving = playerController.isPlayerMoving();

        if (!isPlayerMoving)
        {
            debugAngle.text = angle.ToString();
            if (angle > 70f && angle < 110f)
            {
                if (playerController.CheckIfCanTurnRight())
                {
                    //print("Turn Right");
                    Quaternion targetRotation = character.rotation * rightDirection;
                    character.eulerAngles = targetRotation.eulerAngles;
                }
            }
            else if (angle < -160 || angle > 160)
            {
                if (playerController.CheckIfCanTurnRound())
                {
                    //print("Turn 180");
                    Quaternion targetRotation = character.rotation * leftDirection;
                    targetRotation = targetRotation * leftDirection;
                    character.eulerAngles = targetRotation.eulerAngles;
                }
            }
            else if (angle < -70f && angle > -110f)
            {
                if (playerController.CheckIfCanTurnLeft())
                {
                    //print("Turn Left");
                    Quaternion targetRotation = character.rotation * leftDirection;
                    character.eulerAngles = targetRotation.eulerAngles;
                }
            }
        }
    }

    private void LateUpdate()
    {
        float height = transform.position.y;
        Vector3 newPos = character.position;
        newPos.y = height;
        transform.position = newPos;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        //Gizmos.DrawRay(transform.position, transform.forward * 2);
        Vector3 newPos = transform.position + transform.forward;
        Gizmos.DrawWireSphere(newPos, 1);
    }
}
