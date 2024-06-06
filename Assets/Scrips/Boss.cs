using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Boss : MonoBehaviour
{
    [SerializeField]
    private static float _health;
    [SerializeField] private Slider _healthSlider;
    [SerializeField] private ParticleSystem _explosionPS;
    [SerializeField]
    private float leftBoundary;
    [SerializeField]
    private float rightBoundary;
    [SerializeField]
    private float moveSpeed = 1f;
    // gia su quai  sang phai la true
    private bool _isMovingRight = true;

    private TextMeshProUGUI _scoreText;
    private void Start()
    {
        _health = 100f;
        _healthSlider.maxValue = _health;
    }
    private void Update()
    {
        // lay vi tri hien tai cua quai
        var currentPosition = transform.localPosition;
        if (currentPosition.x < leftBoundary)
        {
            //neu vtri hien tai cua quai < leftBoundary
            //di chuyen sang phai
            _isMovingRight = true;
        }
        else if (currentPosition.x > rightBoundary)
        {
            //neu vtri hien tai cua quai > rightBoundary
            //di chuyen sang phai
            _isMovingRight = false;
        }
        //di chuyen ngang
        var direction = _isMovingRight ? Vector3.right : Vector3.left;
        transform.Translate(direction * moveSpeed * Time.deltaTime);

        // scale hien tai
        var currenScale = transform.localScale;
        if (
            (_isMovingRight == true && currenScale.x < 0) ||
            (_isMovingRight == false && currenScale.x > 0)
           )

        {
            currenScale.x *= -1f;
        }
        transform.localScale = currenScale;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Bullet"))
        {
            //hủy viên đạn 
            Destroy(other.gameObject);
            _health -= 20f;
            _healthSlider.value = _health;
            if (_health <= 0)
            {
                //tạo hiệu ứng nổ 
                var ps = Instantiate(_explosionPS, gameObject.transform.localPosition, Quaternion.identity);
                Destroy(gameObject);

                // hien len canvas
                FindFirstObjectByType<ScoreManagerment>().KillBoss();
            }
        }
    }

}
