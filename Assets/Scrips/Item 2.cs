using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item2 : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem _particleSystem;
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            //tao hieu ung no
            var ps = Instantiate(_particleSystem, transform.position, Quaternion.identity);
            // xoa hieu ung sau 1s
            Destroy(ps.gameObject, 1f);
            // bien mat khoi
            Destroy(gameObject, 0.5f);
        }
        
    }
}
