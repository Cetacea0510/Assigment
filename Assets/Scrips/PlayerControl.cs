using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerControl : MonoBehaviour
{

    // vận tốc chuyển động 
    // public, private, protected, internal, protected internal
    [SerializeField]
    private float moveSpeed = 5f; // 5m/s
                                  // Start is called before the first frame update
                                  // hàm chạy 1 lần duy nhất khi game bắt đầu
                                  // dùng để khởi tạo giá trị

    // biến kiểm tra hướng di chuyển
    [SerializeField]
    private bool _isMovingRight = true;

    // tham chiếu đến rigidbody2D
    private Rigidbody2D _rigidbody2D;
    // giá trị của lực nhảy
    [SerializeField]
    private float _jumpForce = 20f;

    // tham chiếu đến collider2D
    private CapsuleCollider2D _capsuleCollider2D;

    //tham chieu den animator
    private Animator _animator;

    // tạo tham chiếu đêns đạn và súng
    [SerializeField]
    private GameObject _bulletPrefab;
    [SerializeField]
    private Transform _gun;

    //tham chiếu đến file âm thanh
    [SerializeField]
    private AudioClip _coinCollectSXF; //file âm thanh
    private AudioSource _audioSource;//nguồn phát âm thanh

    //tham chiếu đên TMP để hiển thị điểm
    [SerializeField]
    private TextMeshProUGUI _scoreText;
    private static int _score = 0;

    //tham chiếu tới panel gameover 
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

        //hiển thị điểm
        _scoreText.text = _score.ToString();
        _livesText.text = _lives.ToString();
    }

    // Update is called once per frame
    // cố gắng chạy max frame rate
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
    // hàm xử lí dạn bắn
    private void Fire()
    {
        //nếu ng chs bấm phím F
        if (Input.GetKeyDown(KeyCode.F))
        {
            //tạo ra viên đạn tại vị trí súng
            var bullet = Instantiate(_bulletPrefab, _gun.position, Quaternion.identity);
            // cho viên đạn bay theo huớg nv
            var velocity = new Vector2(20f, 0);
            if(_isMovingRight == false) 
            {
                velocity.x *= -1;
            }
            bullet.GetComponent<Rigidbody2D>().velocity = velocity;
            // hủy viên đạn sau 1s
            Destroy(bullet, 0.5f);
        }
        
    }
    
    
    // xử lý điều khiển chuyển động ngang của nhân vật
    // Time.deltaTime: thời gian giữa 2 frame liên tiếp
    // 60fps: 1/60s = 0.0167s
    private void Move()
    {
        // left, right, a, d
        var horizontalInput = Input.GetAxis("Horizontal");
        // 0: không nhấn, âm: trái, dương: phải
        // điều khiển phải trái
        // x=1.5 ----> x=1.5+1=2.5
        transform.localPosition += new Vector3(horizontalInput, 0, 0)
            * moveSpeed * Time.deltaTime;
        // localPosition: vị trí tương đối so với cha
        // position: vị trí tuyệt đối so với thế giới
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
            // đứng yên
            _animator.SetBool("isRun", false);
        }
        // xoay nhân vật
        transform.localScale = _isMovingRight ?
            new Vector2(1.406425f, 1.172423f)
            : new Vector2(-1.406425f, 1.172423f);
    }

    private void Jump()
    {
        if (_isOnLadder)
        {
            return;
        }
        // kiểm tra nhân vật còn đang ở trên nền đất không
        var check = _capsuleCollider2D.IsTouchingLayers(LayerMask.GetMask("Platform"));
        if (check == false)
        {
            return;
        }
        var verticalInput = Input.GetKeyDown(KeyCode.Space) ? 1 : 0;
        if (verticalInput > 0)
        {   
            //cách 1 
            // cung cấp 1 lực đẩy lên trên
            _rigidbody2D.AddForce(new Vector2(0, _jumpForce));
            //cách 2 
            _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, _jumpForce);
        }
    }
    private void Climb()
    {
        _verticalInput = Input.GetAxis("Vertical");
        _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, _verticalInput * moveSpeed);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        //nếu chạm với xu
        if(other.gameObject.CompareTag("Coin"))
        {
            //biến mất xu
            Destroy(other.gameObject);
            //phát ra tiếng nhạc
            _audioSource.PlayOneShot(_coinCollectSXF);
            //tăng điểm
            _score += other.gameObject.GetComponent<Coin>().coinValue;
            //hiển thị điểm
            _scoreText.text = _score.ToString();

        }
        else if (other.gameObject.CompareTag("Enemy"))
        {
            //nếu va chạm với quái
            _lives -= 1;
            if (_lives > 0)
            {
                //reload game tại màn chơi hiện tại 
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                _livesText.text = _lives.ToString();
            }
            else
            {
                // hien pannel game over
                _gameOverPanel.SetActive(true);
                //dừng game
                Time.timeScale = 0;


            }
            
        }
        else if (other.gameObject.CompareTag("Boss"))
        {
            _lives -= 1;
            if (_lives > 0)
            {
                //reload game tại màn chơi hiện tại 
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                _livesText.text = _lives.ToString();
            }
            else
            {
                // hien pannel game over
                _gameOverPanel.SetActive(true);
                //dừng game
                Time.timeScale = 0;


            }
        }
        else if (other.gameObject.CompareTag("Ladder"))
        {
            _isOnLadder = true;
            _rigidbody2D.gravityScale = 0;
        }
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Item"))
        {
            //cho khổi item di chuyển lên 1 đoạn
            // sau đó đi xuống vị trí ban đầu
            //lấy vị trí hiện tại
            StartCoroutine(GoUp(other.gameObject));
        }
    }
    // ddi leen 1 đoạn
    // list 1, 2, 3, 4
    IEnumerator GoUp(GameObject _gameObject)
    {
        //lấy vị trí hiện tại
        var currentPosition = _gameObject.transform.localPosition;
        // lấy vị trí ban đầu 
        var originalPosition = currentPosition;
        //lấy vị trí hiện tại
        while (true) 
        {
            currentPosition.y += 0.01f;
            _gameObject.transform.localPosition = currentPosition;
            
            if (currentPosition.y > originalPosition.y + 1 )
            {
                break;
            }
            yield return null;
        }
        StartCoroutine(GoDown(_gameObject));
    }
    IEnumerator GoDown(GameObject _gameObject)
    {
        //lấy vị trí hiện tại
        var currentPosition = _gameObject.transform.localPosition;
        // lấy vị trí ban đầu 
        var originalPosition = currentPosition;
        //lấy vị trí hiện tại
        while (true)
        {
            currentPosition.y -= 0.01f;
            _gameObject.transform.localPosition = currentPosition;

            if (currentPosition.y < originalPosition.y - 1 )
            {
                break;
            }
            yield return null;
        }
        //hiện item secret
        _gameObject.transform.GetChild(0).gameObject.SetActive(true);
        // ẩn item hiện tại 
        _gameObject.GetComponent<SpriteRenderer>().enabled = false;

    }

    //lấy điểm số 
    public int GetScore()
    {
        return _score;
    }
}
