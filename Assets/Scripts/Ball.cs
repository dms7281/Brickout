using TMPro;
using UnityEngine;

public class Ball : MonoBehaviour
{
    private Vector3 _ballVelocity;
    private Vector3 _initBallVelocity = new Vector3(0f, 0f, -3f); // Initial velocity of the ball
    private Rigidbody _ballRigidBody;
    private int _lives;
    private bool _hardResetBall; // Flag indicating if a hard reset of the ball is needed
    private bool _softResetBall; // Flag indicating if a soft reset of the ball is needed
    private bool _failTriggerHit; // Flag indicating if the fail trigger was hit

    [SerializeField]
    private TextMeshProUGUI _livesLeftUI;

    private void Start()
    {
        _ballRigidBody = GetComponent<Rigidbody>(); // Get the Rigidbody component
        _initBallVelocity = new Vector3(0.04f, 0f, -3f); // Set initial velocity (slight X component)
        _lives = 0; // Initialize lives to 0
    }

    private void FixedUpdate()
    {
        // Move the ball based on its velocity every physics update
        transform.position += _ballVelocity * Time.fixedDeltaTime;
        _ballRigidBody.MovePosition(transform.position); // Move the Rigidbody to match the ball's position
    }

    private void Update()
    {
        // If no lives left, perform hard reset
        if (_lives < 1)
        {
            HardResetBall();
            _hardResetBall = true;
        }
        else
        {
            _hardResetBall = false;
        }

        // If fail trigger was hit, perform soft reset
        if (_failTriggerHit)
        {
            _softResetBall = true;
            _failTriggerHit = false;
        }
        else
        {
            _softResetBall = false;
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        // If collided with fail trigger, decrement lives and update UI
        if (collision.gameObject.CompareTag("Fail Trigger"))
        {
            _lives--;
            _livesLeftUI.text = _lives.ToString();
            _failTriggerHit = true; // Set fail trigger hit flag
        }

        // Reflect the ball's velocity off the collision surface
        _ballVelocity = Vector3.Reflect(_ballVelocity, collision.GetContact(0).normal);
        _ballVelocity.y = 0f; // The balls y velocity is manually set to 0, because there was sometimes weird collisions with the bricks when they were falling 

        // Adjust velocity magnitude depending on what the ball collided with
        if (collision.gameObject.CompareTag("Brick"))
        {
            _ballVelocity = _ballVelocity.normalized * 3.2f;
        }
        else if (collision.gameObject.CompareTag("Shield"))
        {
            _ballVelocity = _ballVelocity.normalized * 2.9f;
        }

        _ballVelocity = _ballVelocity.normalized * 3.05f;
    }

    // Set the balls velocity to be the initial velocity
    public void StartBall()
    {
        _ballVelocity = _initBallVelocity;
    }

    // Perform hard reset of the ball, which includes the soft reset and reseting lives
    public void HardResetBall()
    {
        _lives = 4;
        _livesLeftUI.text = _lives.ToString();
        SoftResetBall();
    }

    // Perform soft reset of the ball, which includes reseting velocity to zero
    public void SoftResetBall()
    {
        _ballVelocity = Vector3.zero;
    }

    // Getter for hard reset flag
    public bool GetHardResetBall()
    {
        return _hardResetBall;
    }

    // Getter for soft reset flag
    public bool GetSoftReset()
    {
        return _softResetBall;
    }
}
