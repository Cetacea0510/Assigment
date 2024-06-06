using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyControl : MonoBehaviour
{
    [SerializeField]
    private float leftBoundary;
    [SerializeField]
    private float rightBoundary;
    [SerializeField]
    private float moveSpeed = 1f;
    // gia su quai  sang phai la true
    private bool _isMovingRight = true;

    private TextMeshProUGUI _scoreText;
    //private int _score = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
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
    // nêú trúng đạn thì chết
    //private void OnCollisionEnter2D(Collision2D other)
    //{
    //    if (other.gameObject.CompareTag("Bullet"))
    //    {
    //        //nếu chạm với viên đạn thì biến mất
    //        Destroy(gameObject);
    //        //viên đạn biến mất
    //        Destroy(other.gameObject);
    //    }
    //}

    // hàm này sd khi có trigger cho quái
    private void OnTriggerEnter2D(Collider2D other )
    {
        if (other.gameObject.CompareTag("Bullet"))
        {
            //nếu chạm với viên đạn thì biến mất
            Destroy(gameObject);

            //viên đạn biến mất
            Destroy(other.gameObject);
            
        }
        
    }
}
