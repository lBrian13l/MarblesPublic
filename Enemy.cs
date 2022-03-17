using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IOnGameOverHandler
{
    GameManager gameManager;
    GameObject[] gems;
    Rigidbody enemyRb;
    [SerializeField] float speed;
    public GameObject Player;
    bool isOnGround;
    Vector3 steepVector;
    readonly float ballRadius = 2.5f;
    GameObject ball;
    public float Health;
    private Vector3 _enemyVelocity;
    private const float Epsilon = 0.00001f;
    private bool _gameOver;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        enemyRb = GetComponent<Rigidbody>();
        Player = GameObject.Find("Player");
        ball = transform.Find("Ball").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < -5 || Health <= 0)
        {
            Destroy(gameObject);
        }



        if (!_gameOver)
            Move();
        RotateBall();
    }

    private void LateUpdate()
    {
        _enemyVelocity = enemyRb.velocity;
    }

    public void HandleOnGameOver()
    {
        _gameOver = true;
    }

    void Move()
    {
        if (!transform.Find("Powerup Indicator").gameObject.activeInHierarchy && isOnGround)
        {
            MoveToPowerup();
        }
        else if (isOnGround)
        {
            MoveToPlayer();
        }
    }

    void MoveToPowerup()
    {
        gems = GameObject.FindGameObjectsWithTag("Gem");
        float distanceToPowerup = 999f;
        if (gems.Length == 0)
        {

        }
        else
        {
            Vector3 toPowerup = Vector3.zero;
            foreach (GameObject gem in gems)
            {
                if ((gem.transform.position - transform.position).magnitude < distanceToPowerup)
                {
                    distanceToPowerup = (gem.transform.position - transform.position).magnitude;
                    toPowerup = (gem.transform.position - transform.position).normalized;
                }
            }
            enemyRb.AddForce(toPowerup * speed * Time.deltaTime, ForceMode.Force);
        }
    }

    void MoveToPlayer()
    {
        enemyRb.AddForce((Player.transform.position - transform.position).normalized * speed * Time.deltaTime, ForceMode.Force);
    }

    void RotateBall()
    {
        Vector3 movement = enemyRb.velocity * Time.deltaTime;
        Vector3 rotationAxis = Vector3.Cross(Vector3.up + steepVector, movement).normalized;
        float distance = movement.magnitude;
        float angle = distance * (180 / Mathf.PI) / ballRadius;
        ball.transform.Rotate(rotationAxis * angle, Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Gem") && !transform.Find("Powerup Indicator").gameObject.activeInHierarchy)
        {
            Destroy(other.gameObject);
            transform.Find("Powerup Indicator").gameObject.SetActive(true);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isOnGround = true;
        }

        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Enemy"))
        {
            Vector3 toEnemyVector = collision.transform.position - transform.position;
            Vector3 toEnemyVelocity = Vector3.Project(_enemyVelocity, toEnemyVector);
            if (AreCodirected(toEnemyVector, toEnemyVelocity))
            {
                int multiplier = (int)(toEnemyVelocity.magnitude / 10f);
                if (collision.gameObject.CompareTag("Enemy"))
                {
                    collision.gameObject.GetComponent<Enemy>().Health -= 10 * multiplier;
                }
                else
                {
                    Player.GetComponent<PlayerController>().Health -= 10 * multiplier;
                }
                //Debug.Log($"Speed: {toEnemyVelocity.magnitude}, multiplier: {multiplier}");
            }
        }
    }

    private bool AreCodirected(Vector3 vector1, Vector3 vector2)
    {
        return Vector3.Dot(vector1.normalized, vector2.normalized) > 1 - Epsilon;
    }

    private void OnCollisionExit(Collision collision)
    {
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
    }

    private void OnEnable()
    {
        EventBus.Subscribe(this);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe(this);
    }
}
