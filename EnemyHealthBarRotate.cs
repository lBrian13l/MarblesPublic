using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealthBarRotate : MonoBehaviour
{
    private Enemy _enemy;
    private GameObject _mainCamera;
    private Vector3 _toPlayer;
    private Quaternion _rotationToPlayer;

    // Start is called before the first frame update
    void Start()
    {
        _mainCamera = GameObject.Find("Main Camera");
        _enemy = GetComponentInParent<Enemy>();
    }

    // Update is called once per frame
    void Update()
    {
        _toPlayer = _enemy.transform.position - _mainCamera.transform.position;
        _rotationToPlayer = Quaternion.LookRotation(_toPlayer);
        transform.rotation = _rotationToPlayer;
    }
}
