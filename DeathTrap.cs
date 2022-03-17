using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathTrap : MonoBehaviour
{
    private GameManager _gameManager;
    // Start is called before the first frame update
    void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //_gameManager.gameOver = true;
            EventBus.RaiseEvent<IGameOverHandler>(h => h.HandleGameOver());
            Debug.Log("Game Over (spikes)");
        }
        else
        {
            Destroy(collision.gameObject);
            Debug.Log("Destroyed (spikes)");
        }
    }
}
