using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class ScoreManagerment : MonoBehaviour
{
    [SerializeField] private GameObject _achivementCanvas;
    [SerializeField] private TextMeshProUGUI _highScoreText;
    [SerializeField] private TextMeshProUGUI _currentScoreText;
    [SerializeField] private TextMeshProUGUI _minTime;

    GameData _gameData;
    PlayerControl _playerControl;

    // Start is called before the first frame update
    private void Start()
    {
        _playerControl = FindFirstObjectByType<PlayerControl>();
        _achivementCanvas.SetActive(false);
    }
    // sau khi giết boss
    // đọc dữ liệu từ file
    // hiển thị màn hình thành tích
    // ghi dữ liệu vào file 

    // Update is called once per frame
    void Update()
    {
        // thay vì giết boss , nhấn phím S để thực hiện 
        if (Input.GetKeyUp(KeyCode.S)) 
        { 
            ReadDataFromFile();
            ShowData();
            WriteDataFile();
        }
    }
    public void KillBoss()
    {
        _achivementCanvas.SetActive(true);
        ReadDataFromFile();
        ShowData();
        WriteDataFile();
    }
    void ReadDataFromFile()
    {
        // đọc dư liệu từ file
        _gameData = DataManager.ReadData();
        if (_gameData == null)
        {
            _gameData = new GameData()
            {
                score = 0,
                time = 99999999 // chua chs bao gio
            };
        }
    }
    void ShowData()
    {
        //hiển thị dữ liệu lên màn hình
        var score = _playerControl.GetScore();// điểm hiện tại
        //điểm cao nhất

        var maxScore = Mathf.Max(score, _gameData.score);
        _highScoreText.text = $"High Score: {maxScore}";
        _currentScoreText.text = $"Current Score: {score}";

        //hiển thị màn hình thành tích 
        _achivementCanvas.SetActive(true);

        //cập nhật dữ liệu
        _gameData.score = maxScore;
    }
    void WriteDataFile()
    {
        // ghi dữ liệu vào file
        DataManager.SaveData(_gameData);
    }
}
