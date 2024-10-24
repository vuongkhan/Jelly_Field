using UnityEngine;
using TMPro; // Thư viện cho TextMeshPro
using UnityEngine.SceneManagement; // Thư viện để quản lý các scene

public class UIManager : MonoBehaviour
{
    public LevelManager levelManager; // Tham chiếu đến LevelManager
    public TextMeshProUGUI conditionText; // Tham chiếu đến Text hiển thị điều kiện
    public TextMeshProUGUI scoreText; // Tham chiếu đến Text hiển thị điểm
    public GameObject nextLevelPanel; // Tham chiếu đến bảng Next Level
    private GameManager gameManager; // Tham chiếu đến GameManager
    public GameObject gameOverPanel;

    private void Start()
    {
        // Lấy GameManager từ scene
        gameManager = FindObjectOfType<GameManager>();
        // Cập nhật UI khi bắt đầu
        UpdateUI();
        nextLevelPanel.SetActive(false); // Ẩn bảng Next Level khi bắt đầu
    }

    // Phương thức để cập nhật UI
    public void UpdateUI()
    {
        // Cập nhật điều kiện cấp độ
        conditionText.text = GetConditionsText();
        // Cập nhật điểm cho các màu jelly, không bao gồm màu đỏ
        scoreText.text = GetScoreText();

        // Kiểm tra xem có đạt điều kiện không để hiển thị bảng Next Level
        CheckConditionsAndShowNextLevel();
    }

    // Phương thức để lấy điều kiện hiện tại
    private string GetConditionsText()
    {
        // Khởi tạo chuỗi điều kiện
        string conditions = "Điều kiện:\n";
        // Duyệt qua từng điều kiện trong levelManager
        foreach (var condition in levelManager.levelConditions)
        {
            // Thêm thông tin điều kiện vào chuỗi
            conditions += $"{condition.color}: {condition.requiredAmount}\n";
        }
        return conditions; // Trả về chuỗi điều kiện
    }

    // Phương thức để lấy điểm hiện tại cho từng màu, không bao gồm màu đỏ
    private string GetScoreText()
    {
        // Khởi tạo chuỗi điểm
        string scoreInfo = "Điểm hiện tại:\n";

        // Hiển thị điểm cho từng màu Jelly, bỏ qua màu đỏ
        foreach (var condition in levelManager.levelConditions)
        {
            // Bỏ qua màu đỏ
            if (condition.color == JellyColor.Red) continue;

            // Lấy điểm cho từng màu từ GameManager
            int score = gameManager.GetScoreForColor(condition.color);
            // Thêm thông tin điểm vào chuỗi
            scoreInfo += $"{condition.color}: {score}\n";
        }

        return scoreInfo; // Trả về chuỗi điểm
    }

    // Kiểm tra các điều kiện và hiển thị bảng Next Level nếu đạt
    private void CheckConditionsAndShowNextLevel()
    {
        foreach (var condition in levelManager.levelConditions)
        {
            // Bỏ qua màu đỏ
            if (condition.color == JellyColor.Red) continue;

            int score = gameManager.GetScoreForColor(condition.color);
            if (score < condition.requiredAmount)
            {
                nextLevelPanel.SetActive(false); // Ẩn bảng nếu chưa đủ điều kiện
                return; // Nếu có điều kiện chưa đủ, thoát phương thức
            }
        }
        // Nếu tất cả điều kiện đã đạt (không tính màu đỏ), hiển thị bảng Next Level
        nextLevelPanel.SetActive(true);
    }

    // Phương thức để chuyển sang cấp độ tiếp theo
    public void LoadNextLevel(string nextLevelName)
    {
        // Chuyển đến scene có tên là nextLevelName
        SceneManager.LoadScene(nextLevelName);
    }
    public void ExitGame()
    {
        Application.Quit();
    }
    public void GameOver()
    {
        gameOverPanel.SetActive(true);
    }
}
