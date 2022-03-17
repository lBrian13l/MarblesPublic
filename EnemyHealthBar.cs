using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyHealthBar : MonoBehaviour
{
    private Image _healthBar;
    private Enemy _enemy;
    private const float MaxHealth = 100f;
    [SerializeField] TMP_Text _healthCount;

    // Start is called before the first frame update
    void Start()
    {
        _healthBar = GetComponent<Image>();
        _enemy = GetComponentInParent<Enemy>();
    }

    // Update is called once per frame
    void Update()
    {
        _healthBar.fillAmount = _enemy.Health / MaxHealth;
        _healthCount.text = _enemy.Health.ToString();
    }
}
