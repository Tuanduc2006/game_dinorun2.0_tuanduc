using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Giao dien UI")]
    public GameObject startMenuPanel;
    public GameObject gameOverPanel;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highScoreText;

    [Header("He thong Diem & Toc Do")]
    public float score;
    public float scoreMultiplier = 10f;

    public float gameSpeed = 1f;
    public float speedIncreaseRate = 0.02f;
    public float maxGameSpeed = 3f;

    [Header("Trang thai Game")]
    public bool isGameStarted = false;
    private bool isGameOver = false;

    // BIẾN STATIC CỰC QUAN TRỌNG: Ghi nhớ việc có bỏ qua Menu khi chơi lại không
    public static bool skipStartMenu = false;

    void Start()
    {
        isGameOver = false;
        score = 0;
        gameSpeed = 1f;

        // Ẩn bảng Game Over
        if (gameOverPanel != null) gameOverPanel.SetActive(false);

        // KIỂM TRA BIẾN GHI NHỚ
        if (skipStartMenu)
        {
            // NẾU BẤM RETRY TỪ TRƯỚC -> Vào thẳng game luôn
            isGameStarted = true;
            if (startMenuPanel != null) startMenuPanel.SetActive(false); // Tắt Menu
            Time.timeScale = 1f; // Cho chạy thời gian ngay lập tức
        }
        else
        {
            // NẾU LÀ LẦN ĐẦU TIÊN MỞ GAME -> Hiện Menu
            isGameStarted = false;
            if (startMenuPanel != null) startMenuPanel.SetActive(true); // Bật Menu
            Time.timeScale = 0f; // Đóng băng thời gian
        }

        UpdateHighScoreDisplay();
    }

    void Update()
    {
        // Chỉ cộng điểm và tăng tốc nếu game đã bấm Start và chưa chết
        if (isGameStarted && !isGameOver)
        {
            score += Time.deltaTime * scoreMultiplier;
            if (scoreText != null) scoreText.text = "DIEM: " + Mathf.FloorToInt(score).ToString();

            if (gameSpeed < maxGameSpeed)
            {
                gameSpeed += speedIncreaseRate * Time.deltaTime;
            }
        }
    }

    // Hàm này gắn vào nút START
    public void StartGame()
    {
        isGameStarted = true;
        if (startMenuPanel != null) startMenuPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    // Hàm này gọi khi đâm chướng ngại vật
    public void GameOver()
    {
        isGameOver = true;
        Time.timeScale = 0f;
        CheckAndSaveHighScore();
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
    }

    // Hàm này gắn vào nút RETRY
    public void RestartGame()
    {
        // BẬT CỜ BỎ QUA MENU CHO LẦN LOAD SAU
        skipStartMenu = true;

        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void CheckAndSaveHighScore()
    {
        int currentScoreInt = Mathf.FloorToInt(score);
        int savedHighScore = PlayerPrefs.GetInt("HighScore", 0);

        if (currentScoreInt > savedHighScore)
        {
            PlayerPrefs.SetInt("HighScore", currentScoreInt);
            PlayerPrefs.Save();
            UpdateHighScoreDisplay();
        }
    }

    private void UpdateHighScoreDisplay()
    {
        if (highScoreText != null)
        {
            int savedHighScore = PlayerPrefs.GetInt("HighScore", 0);
            highScoreText.text = "LS: " + savedHighScore.ToString();
        }
    }
}