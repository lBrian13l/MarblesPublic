using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBar : MonoBehaviour
{
    private Image _healthBar;
    private PlayerController _player;
    private const float MaxHealth = 100f;
    [SerializeField] TMP_Text _healthCount;

    // Start is called before the first frame update
    void Start()
    {
        _healthBar = GetComponent<Image>();
        _player = FindObjectOfType<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        _healthBar.fillAmount = _player.Health / MaxHealth;
        _healthCount.text = _player.Health.ToString();
    }
}
