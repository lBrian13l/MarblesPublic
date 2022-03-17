using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour, IOnGameOverHandler
{
    GameManager gameManager;
    public GameObject prefabPowerup;
    public GameObject prefabEnemy;
    float spawnRange = 45f;
    public int enemyCount;
    int powerupCount;
    Enemy[] enemies;
    GameObject player;
    private GameObject[] _powerups;
    private int _enemiesWithoutIndicator;
    [SerializeField] int _enemiesOnFirstWave;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        player = GameObject.Find("Player");
        SpawnEnemies();
        SpawnPowerup();
    }

    // Update is called once per frame
    void Update()
    {
        _powerups = GameObject.FindGameObjectsWithTag("Gem");
        powerupCount = _powerups.Length;
        enemies = FindObjectsOfType<Enemy>();

        _enemiesWithoutIndicator = 0;
        foreach (Enemy enemy in enemies)
        {
            if (!enemy.transform.Find("Powerup Indicator").gameObject.activeInHierarchy)
            {
                _enemiesWithoutIndicator++;
            }
        }

        if (powerupCount > _enemiesWithoutIndicator && !player.transform.Find("Powerup Indicator").gameObject.activeInHierarchy)
        {
            player.transform.Find("Powerup Indicator").gameObject.SetActive(true);
            FinishWave();
        }
        else if (powerupCount >= _enemiesWithoutIndicator && player.transform.Find("Powerup Indicator").gameObject.activeInHierarchy)
        {
            FinishWave();
        }

        if (powerupCount == 0)
        {
            foreach (Enemy enemy in enemies)
            {
                if (!enemy.transform.Find("Powerup Indicator").gameObject.activeInHierarchy)
                {
                    Destroy(enemy.gameObject);
                    Debug.Log("Enemy Destroyed");
                }
                else
                {
                    enemy.transform.Find("Powerup Indicator").gameObject.SetActive(false);
                    //Debug.Log("Disabled");
                }
            }

            if (player.transform.Find("Powerup Indicator").gameObject.activeInHierarchy)
            {
                player.transform.Find("Powerup Indicator").gameObject.SetActive(false);
                //SpawnPowerup();
            }
            else
            {
                //gameManager.gameOver = true;
                Debug.Log("Game over (no gem)");
                EventBus.RaiseEvent<IGameOverHandler>(h => h.HandleGameOver());
            }
        }

        //if (gameManager.gameOver == true)
        //    gameObject.SetActive(false);
    }

    private void LateUpdate()
    {
        powerupCount = GameObject.FindGameObjectsWithTag("Gem").Length;
        if (powerupCount == 0)
        {
            SpawnPowerup();
        }
    }

    public void HandleOnGameOver()
    {
        //gameObject.SetActive(false);
    }

    private void FinishWave()
    {
        foreach (Enemy enemy in enemies)
        {
            if (!enemy.transform.Find("Powerup Indicator").gameObject.activeInHierarchy)
            {
                enemy.transform.Find("Powerup Indicator").gameObject.SetActive(true);
            }
        }
        if (_powerups.Length != 0)
        {
            foreach (GameObject powerup in _powerups)
                Destroy(powerup);
        }
        powerupCount = 0;
    }

    void SpawnPowerup()
    {
        enemyCount = FindObjectsOfType<Enemy>().Length;
        Debug.Log(enemyCount);
        Vector3 spawnLocation;
        for (int i = 0; i < enemyCount; i++)
        {
            spawnLocation = new Vector3(Random.Range(-spawnRange, spawnRange), 3f, Random.Range(-spawnRange, spawnRange));
            Instantiate(prefabPowerup, spawnLocation, prefabPowerup.transform.rotation);
        }
    }

    void SpawnEnemies()
    {
        Vector3 spawnLocation;
        for (int i = 0; i < _enemiesOnFirstWave; i++)
        {
            spawnLocation = new Vector3(Random.Range(-spawnRange, spawnRange), 3f, Random.Range(-spawnRange, spawnRange));
            Instantiate(prefabEnemy, spawnLocation, prefabEnemy.transform.rotation);
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
