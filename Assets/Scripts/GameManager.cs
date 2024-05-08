using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _ballReference;
    [SerializeField]
    private GameObject _shieldReference;
    [SerializeField]
    private GameObject _brickPatternReference;

    private Ball _ball;
    private Shield _shield;
    private BrickPattern _brickPattern;
    private bool _waitForPlayerToFire;
    private Vector3 _ballFirePos;

    private void Awake()
    {
        Application.targetFrameRate = 60;
    }

    private void Start()
    {
        _ball = _ballReference.GetComponent<Ball>();
        _shield = _shieldReference.GetComponent<Shield>();
        _brickPattern = _brickPatternReference.GetComponent<BrickPattern>();
        _ballFirePos = Vector3.zero;
        _ballFirePos.y = _ball.transform.position.y;
    }
    private void Update()
    {
        if(_ball.GetHardResetBall()) // Case where lives are 0 and the ball has hard reset
        {
            _brickPattern.RestartLevel(); // Brick pattern then restarts the level
            _waitForPlayerToFire = true;
        }

        if(_ball.GetSoftReset()) // Case where ball hits fail trigger
        {
            _waitForPlayerToFire = true;
        }
        
        if(_brickPattern.IsLevelTransition()) // Case where brick pattern is going to the next section of the level
        {
            _ball.SoftResetBall(); // Ball then does a soft reset, as it does when it hits the fail trigger
            _waitForPlayerToFire = true;
        }

        if(_brickPattern.IsLevelOver()) // Case brick pattern is at the end of the file, or rather the level is over
        {
            _ball.HardResetBall(); // Ball does a hard reset
            _waitForPlayerToFire = true;
        }

        if(!_waitForPlayerToFire) return; // If any of those cases become true, then the ball is moved into "Fire Position" and will wait for the player to press space

        _ballFirePos.x = _shield.transform.position.x;
        _ballFirePos.z = _shield.transform.position.z + _ball.transform.localScale.z + 0.1f;

        _ball.transform.position = _ballFirePos; // Setting ball to follow the shield until space is pressed

        if(Input.GetKeyDown(KeyCode.Space))
        {
            _ball.StartBall();
            _waitForPlayerToFire = false;
        }    
    }
}
