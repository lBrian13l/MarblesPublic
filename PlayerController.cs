using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, IOnGameOverHandler
{
    GameManager gameManager;
    [SerializeField] float speed;
    [SerializeField] float speedLimit;
    float horizontalInput;
    float verticalInput;
    Rigidbody playerRb;
    bool isOnGround;
    [SerializeField] float jumpForce;
    private GameObject _focalPoint;
    GameObject ball;
    readonly float ballRadius = 2.5f;
    Vector3 steepVector;
    [SerializeField] float cameraRotationSpeed;
    float mouseHorizontalInput;
    float mouseVerticalInput;
    private Vector3 _normalizedVerticalMovementVector;
    private Vector3 _normalizedMovementVector;
    public PlayerInput Player_Input;
    private Vector2 _moveDirectionInput;
    private Vector2 _lookDirection;
    private float _rotationX;
    private float _rotationY;
    private Quaternion _rotationMovement;
    [SerializeField] float _attackPower;
    private bool _attackedRecently;
    private bool _attackCooldown;
    public float Health;
    private GameObject _powerupIndicator;
    private const float Epsilon = 0.00001f;
    private Vector3 _playerVelocity;
    public float AttackCooldown;
    private AttackCooldownIcon _attackCooldownIcon;

    private void Awake()
    {
        _focalPoint = GameObject.Find("Focal Point");

        Player_Input = new PlayerInput();

        Player_Input.Player.Jump.performed += ctx => OnJump();
        Player_Input.Player.Move.performed += ctx => OnMove();
        Player_Input.Player.Look.performed += ctx => OnLook();
        Player_Input.Player.Attack.performed += ctx => OnAttack();
    }

    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        playerRb = GetComponent<Rigidbody>();
        ball = transform.Find("Ball").gameObject;
        _powerupIndicator = transform.Find("Powerup Indicator").gameObject;
        _attackCooldownIcon = FindObjectOfType<AttackCooldownIcon>();
    }

    // Update is called once per frame
    void Update()
    {
        //verticalInput = Input.GetAxis("Vertical");
        //horizontalInput = Input.GetAxis("Horizontal");

        _moveDirectionInput = Player_Input.Player.Move.ReadValue<Vector2>();
        _lookDirection = Player_Input.Player.Look.ReadValue<Vector2>();

        //if (!gameManager.gameOver)
        //{
            //if (isOnGround)
                Move();
            //OnJump();
            if (!_attackedRecently)
                SpeedLimit();
            RotateBall();
        //}
        //else
        //{
        //    _powerupIndicator.SetActive(false);
        //    ball.GetComponent<Renderer>().enabled = false;
        //}

        if (Health <= 0 || transform.position.y < -5)
        {
            EventBus.RaiseEvent<IGameOverHandler>(h => h.HandleGameOver());
        }

        //if (Health <= 0)
        //{
        //    gameManager.gameOver = true;
        //    Debug.Log("Game over (health low)");
        //}
        //else if (transform.position.y < -5)
        //{
        //    gameManager.gameOver = true;
        //    Debug.Log("Game over (dropped from map)");
        //}
    }

    private void LateUpdate()
    {
        RotateCamera();
        _playerVelocity = playerRb.velocity;
    }

    public void HandleOnGameOver()
    {
        GetComponent<SphereCollider>().enabled = false;
        _powerupIndicator.SetActive(false);
        GetComponent<Rigidbody>().useGravity = false;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        ball.GetComponent<Renderer>().enabled = false;
        Player_Input.Disable();
        Health = 0;
    }

    private void OnAttack()
    {
        if (!_attackCooldown)
        {
            _attackedRecently = true;
            _attackCooldown = true;
            _normalizedVerticalMovementVector = new Vector3(_focalPoint.transform.forward.x, 0f, _focalPoint.transform.forward.z).normalized;
            playerRb.AddForce(_normalizedVerticalMovementVector * _attackPower, ForceMode.Impulse);
            _attackCooldownIcon.Attacked();
            StartCoroutine("c_AttackCooldown");
            StartCoroutine("c_SpeedLimitDisabled");
        }
    }

    IEnumerator c_AttackCooldown()
    {
        yield return new WaitForSeconds(AttackCooldown);
        _attackCooldown = false;
    }

    IEnumerator c_SpeedLimitDisabled()
    {
        yield return new WaitForSeconds(0.5f);
        _attackedRecently = false;
    }

    private void OnMove()
    {
        _moveDirectionInput = Player_Input.Player.Move.ReadValue<Vector2>();
    }

    void Move()
    {
        _normalizedMovementVector = new Vector3(_moveDirectionInput.x, 0, _moveDirectionInput.y);
        _normalizedVerticalMovementVector = new Vector3(_focalPoint.transform.forward.x, 0f, _focalPoint.transform.forward.z).normalized;
        _rotationMovement = Quaternion.FromToRotation(Vector3.forward, _normalizedVerticalMovementVector);
        _normalizedMovementVector = _rotationMovement * _normalizedMovementVector;
        playerRb.AddForce(_normalizedMovementVector * speed * Time.deltaTime);

        //    if (playerRb.velocity.magnitude < speedLimit)
        //    {
        //        _normalizedVerticalMovementVector = new Vector3(focalPoint.transform.forward.x, 0f, focalPoint.transform.forward.z).normalized;
        //        _normalizedMovementVector = (focalPoint.transform.right * horizontalInput + _normalizedVerticalMovementVector * verticalInput).normalized;
        //        playerRb.AddForce(_normalizedMovementVector * speed * Time.deltaTime);
        //    }
    }

    private void OnLook()
    {
        _lookDirection = Player_Input.Player.Look.ReadValue<Vector2>();
    }

    void RotateCamera()
    {
        _rotationX += _lookDirection.x;
        _rotationY += _lookDirection.y;

        _focalPoint.transform.eulerAngles = new Vector3(-_rotationY * cameraRotationSpeed, _rotationX * cameraRotationSpeed, 0);

        if (_rotationY > 50)
            _rotationY = 50;
        else if (_rotationY < -35)
            _rotationY = -35;

        //    mouseHorizontalInput += Input.GetAxis("Mouse X");
        //    mouseVerticalInput -= Input.GetAxis("Mouse Y");
        //    if (mouseVerticalInput > 10)
        //        mouseVerticalInput = 10;
        //    else if (mouseVerticalInput < -10)
        //        mouseVerticalInput = -10;

        //    focalPoint.transform.eulerAngles = new Vector3(mouseVerticalInput * cameraRotationSpeed, mouseHorizontalInput * cameraRotationSpeed, 0);
    }

    private void OnJump()
    {
        if (isOnGround)
        {
            playerRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void RotateBall()
    {
        Vector3 movement = playerRb.velocity * Time.deltaTime;
        Vector3 rotationAxis = Vector3.Cross(Vector3.up + steepVector, movement).normalized;
        movement -= (Vector3.up + steepVector) * Vector3.Dot(movement, (Vector3.up + steepVector));
        float distance = movement.magnitude;
        float angle = distance * (180 / Mathf.PI) / ballRadius;
        ball.transform.Rotate(rotationAxis * angle, Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Gem") && !transform.Find("Powerup Indicator").gameObject.activeInHierarchy)
        {
            Destroy(other.gameObject);
            _powerupIndicator.SetActive(true);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isOnGround = true;
        }

        if (collision.gameObject.CompareTag("Enemy"))
        {
            Vector3 toEnemyVector = collision.transform.position - transform.position;
            Vector3 toEnemyVelocity = Vector3.Project(_playerVelocity, toEnemyVector);
            if (AreCodirected(toEnemyVector, toEnemyVelocity))
            {
                int multiplier = (int)(toEnemyVelocity.magnitude / 10f);
                Damage(collision, multiplier);
                //Debug.Log($"Speed: {toEnemyVelocity.magnitude}, multiplier: {multiplier}");
            }
        }
    }

    private bool AreCodirected(Vector3 vector1, Vector3 vector2)
    {
        return Vector3.Dot(vector1.normalized, vector2.normalized) > 1 - Epsilon;
    }

    private void Damage(Collision collision, int multiplier)
    {
        collision.gameObject.GetComponent<Enemy>().Health -= 10 * multiplier;
    }

    private void OnCollisionExit(Collision collision)
    {
        steepVector = Vector3.zero;
        if (collision.gameObject.CompareTag("Ground"))
        {
            isOnGround = false;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        steepVector = Vector3.zero;
        for (int i = 0; i < collision.contactCount; i++)
        {
            if (!collision.gameObject.CompareTag("Ground"))
            {
                steepVector += collision.GetContact(i).normal;
            }
        }
        //if (!isOnGround)
        //{
        //    Move();
        //}
    }

    void SpeedLimit()
    {
        if (playerRb.velocity.magnitude > speedLimit)
        {
            playerRb.velocity = playerRb.velocity.normalized * speedLimit;
        }

        //if (playerRb.velocity.x > speedLimit)
        //    playerRb.velocity = new Vector3(speedLimit, 0, playerRb.velocity.z);

        //else if (playerRb.velocity.x < -speedLimit)
        //    playerRb.velocity = new Vector3(-speedLimit, 0, playerRb.velocity.z);

        //if (playerRb.velocity.z > speedLimit)
        //    playerRb.velocity = new Vector3(playerRb.velocity.x, 0, speedLimit);

        //else if (playerRb.velocity.z < -speedLimit)
        //    playerRb.velocity = new Vector3(playerRb.velocity.x, 0, -speedLimit);
    }

    private void OnEnable()
    {
        Player_Input.Enable();
        EventBus.Subscribe(this);
    }

    private void OnDisable()
    {
        Player_Input.Disable();
        EventBus.Unsubscribe(this);
    }
}
