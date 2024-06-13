using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerControl : MonoBehaviour
{
    // Vận tốc chuyển động 
    [SerializeField]
    private float moveSpeed = 5f; // 5m/s

    // Biến kiểm tra hướng di chuyển
    [SerializeField]
    private bool _isMovingRight = true;

    // Tham chiếu đến rigidbody2D
    private Rigidbody2D _rigidbody2D;
    // Giá trị của lực nhảy
    [SerializeField]
    private float _jumpForce = 5f;

    // Lực nhảy thêm khi nhảy lên pad
    [SerializeField]
    private float _padJumpForce = 10f;
    private bool _isOnPad = false;

    // Tham chiếu đến collider2D
    private CapsuleCollider2D _capsuleCollider2D;

    // Tham chiếu đến animator
    private Animator _animator;

    // Tạo tham chiếu đến đạn và súng
    [SerializeField]
    private GameObject _bulletPrefab;
    [SerializeField]
    private Transform _gun;

    // Tham chiếu đến file âm thanh
    [SerializeField]
    private AudioClip _coinCollectSXF; // File âm thanh
    private AudioSource _audioSource; // Nguồn phát âm thanh

    // Tham chiếu đến TMP để hiển thị điểm
    [SerializeField]
    private TextMeshProUGUI _scoreText;
    private static int _score = 0;

    // Tham chiếu tới panel gameover 
    [SerializeField]
    private GameObject _gameOverPanel;
    private static int _lives = 3;
    [SerializeField]
    private TextMeshProUGUI _livesText;

    private bool _isOnLadder = false;
    private float _verticalInput;

    void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _capsuleCollider2D = GetComponent<CapsuleCollider2D>();
        _animator = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();

        // Hiển thị điểm
        _scoreText.text = _score.ToString();
        _livesText.text = _lives.ToString();
        _rigidbody2D.gravityScale = 5; // Đảm bảo trọng lực bắt đầu ở giá trị đúng
    }

    // Update được gọi mỗi khung hình
    void Update()
    {
        Move();
        Jump();
        Fire();
        if (_isOnLadder)
        {
            Climb();
        }
    }

    private void Fire()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            _animator.SetTrigger("attack"); // Kích hoạt hoạt ảnh tấn công

            var bullet = Instantiate(_bulletPrefab, _gun.position, Quaternion.identity);
            var velocity = new Vector2(20f, 0);
            if (_isMovingRight == false)
            {
                velocity.x *= -1;
            }
            bullet.GetComponent<Rigidbody2D>().velocity = velocity;
            Destroy(bullet, 0.5f);
        }
    }

    private void Move()
    {
        var horizontalInput = Input.GetAxis("Horizontal");
        transform.localPosition += new Vector3(horizontalInput, 0, 0) * moveSpeed * Time.deltaTime;

        if (horizontalInput > 0)
        {
            _isMovingRight = true;
            _animator.SetBool("isRun", true);
        }
        else if (horizontalInput < 0)
        {
            _isMovingRight = false;
            _animator.SetBool("isRun", true);
        }
        else
        {
            _animator.SetBool("isRun", false);
        }

        transform.localScale = _isMovingRight ?
            new Vector2(1.406425f, 1.172423f) :
            new Vector2(-1.406425f, 1.172423f);
    }

    private void Jump()
    {
        if (_isOnLadder)
        {
            return;
        }

        var check = _capsuleCollider2D.IsTouchingLayers(LayerMask.GetMask("Platform"));
        if (check == false)
        {
            return;
        }

        var verticalInput = Input.GetKeyDown(KeyCode.Space) ? 1 : 0;
        if (verticalInput > 0)
        {
            float jumpPower = _isOnPad ? _jumpForce + _padJumpForce : _jumpForce;
            _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, jumpPower);
            _animator.SetBool("isJumping", true); // Kích hoạt hoạt ảnh nhảy
        }
        else
        {
            _animator.SetBool("isJumping", false); // Tắt hoạt ảnh nhảy
        }
    }

    private void Climb()
    {
        _verticalInput = Input.GetAxis("Vertical");
        _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, _verticalInput * moveSpeed);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Coin"))
        {
            Destroy(other.gameObject);
            _audioSource.PlayOneShot(_coinCollectSXF);
            _score += other.gameObject.GetComponent<Coin>().coinValue;
            _scoreText.text = _score.ToString();
        }
        else if (other.gameObject.CompareTag("Enemy") || other.gameObject.CompareTag("Boss") || other.gameObject.CompareTag("Trap"))
        {
            _lives -= 1;
            if (_lives > 0)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                _livesText.text = _lives.ToString();
            }
            else
            {
                _gameOverPanel.SetActive(true);
                Time.timeScale = 0;
            }
        }
        else if (other.gameObject.CompareTag("Ladder"))
        {
            _isOnLadder = true;
            _rigidbody2D.gravityScale = 0;
        }
        else if (other.gameObject.CompareTag("Pad"))
        {
            _isOnPad = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Ladder"))
        {
            _isOnLadder = false;
            _rigidbody2D.gravityScale = 1;
        }
        else if (other.gameObject.CompareTag("Pad"))
        {
            _isOnPad = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Item"))
        {
            StartCoroutine(GoUp(other.gameObject));
        }
    }

    IEnumerator GoUp(GameObject _gameObject)
    {
        var currentPosition = _gameObject.transform.localPosition;
        var originalPosition = currentPosition;

        while (true)
        {
            currentPosition.y += 0.01f;
            _gameObject.transform.localPosition = currentPosition;

            if (currentPosition.y > originalPosition.y + 1)
            {
                break;
            }
            yield return null;
        }
        StartCoroutine(GoDown(_gameObject));
    }

    IEnumerator GoDown(GameObject _gameObject)
    {
        var currentPosition = _gameObject.transform.localPosition;
        var originalPosition = currentPosition;

        while (true)
        {
            currentPosition.y -= 0.01f;
            _gameObject.transform.localPosition = currentPosition;

            if (currentPosition.y < originalPosition.y - 1)
            {
                break;
            }
            yield return null;
        }

        _gameObject.transform.GetChild(0).gameObject.SetActive(true);
        _gameObject.GetComponent<SpriteRenderer>().enabled = false;
    }

    public int GetScore()
    {
        return _score;
    }
}
