using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour, IGameOverHandler
{
    //public bool gameOver;
    public Button restartButton;
    public TextMeshProUGUI gameOverText;
    public TextMeshProUGUI EnemyCounterText;
    private PlayerController _player;
    private PlayerInput _playerInput;
    private SpawnManager _spawnManager;

    private void Awake()
    {
        _playerInput = new PlayerInput();
        _playerInput.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
        _player = FindObjectOfType<PlayerController>();
        _spawnManager = FindObjectOfType<SpawnManager>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _playerInput.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        EnemyCounterText.text = $"Enemies: {_spawnManager.enemyCount}";

        //if (gameOver)
        //{
        //    if (FindObjectOfType<Enemy>() == null)
        //        gameOverText.text = "Victory";
        //    gameOverText.gameObject.SetActive(true);
        //    restartButton.gameObject.SetActive(true);
        //    _player.Player_Input.Disable();
        //    Cursor.lockState = CursorLockMode.None;
        //    Cursor.visible = true;
        //    _playerInput.Enable();
        //}
    }

    public void RestartGame()
    {
        _playerInput.Disable();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        _playerInput.Disable();
        //_player.Player_Input.Enable();
    }

    //private void OnRestart()
    //{
    //    //gameOver = false;
    //    _playerInput.Disable();
    //    RestartGame();
    //}

    private void OnEnable()
    {
        //_playerInput.Disable();
        EventBus.Subscribe(this);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe(this);
    }

    public void HandleGameOver()
    {
        gameOverText.gameObject.SetActive(true);
        restartButton.gameObject.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        _playerInput.Enable();
        _playerInput.UI.Restart.performed += ctx => RestartGame();


        EventBus.RaiseEvent<IOnGameOverHandler>(h => h.HandleOnGameOver());
    }
}
