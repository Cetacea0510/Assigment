using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Gate : MonoBehaviour
{
    [SerializeField] private GameObject _loadingCanvas;
    [SerializeField] private Slider _slider;
    [SerializeField] private TextMeshProUGUI _progressText;
    private float _progress = 0;

    // Thêm biến để giữ màn hình đích
    [SerializeField] private int targetSceneIndex;

    private void Start()
    {
        _loadingCanvas.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // Hiển thị màn hình loading
            _loadingCanvas.SetActive(true);
            _slider.value = _progress;
            _progressText.text = _progress + "%";
            StartCoroutine(LoadScene());
        }
    }

    IEnumerator LoadScene()
    {
        while (_progress < 100)
        {
            _progress += 1;
            _slider.value = _progress;
            _progressText.text = _progress + "%";
            yield return new WaitForSeconds(0.1f);
        }
        // Chuyển màn hình
        SceneManager.LoadScene(targetSceneIndex);
    }
}
