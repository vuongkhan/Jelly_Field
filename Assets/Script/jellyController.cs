using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JellyManager : MonoBehaviour
{
    private bool hasScaled = false;
    public GameObject jellyPrefab; // Prefab của khối jelly bạn sẽ kéo vào từ Inspector
    public Transform slot;         // Vị trí của ô chứa jelly (đối tượng cha)
    public int jellyCount = 4;     // Số lượng jelly ban đầu (4 Jelly)
    public float jellySpacing = 1.0f;  // Khoảng cách giữa các jelly
    public bool isSingle = false;
    private Dictionary<int, GameObject> jellyInstances = new Dictionary<int, GameObject>(); // Lưu lại các jelly với số thứ tự

    // Danh sách màu sắc cố định
    private List<Color> jellyColors = new List<Color>
    {
        Color.blue,      // Xanh dương
        new Color(0.5f, 0.5f, 1f), // Xanh nhạt
        new Color(0.5f, 0f, 0.5f), // Tím
        Color.yellow,    // Vàng
        Color.magenta,   // Hồng
        Color.red        // Đỏ
    };

    void Start()
    {
        // Khi bắt đầu trò chơi, tạo jelly theo số lượng được chỉ định
        UpdateJellies(jellyCount);
    }

    public void UpdateJellies(int count)
    {
        // Xóa các jelly hiện tại trước khi tạo mới
        ClearExistingJellies();

        // Đảm bảo rằng số lượng Jelly không vượt quá 4
        int maxCount = Mathf.Min(count, 4); // Chỉ cho phép tối đa 4 phần tử

        // Danh sách lưu màu sắc đã sử dụng
        List<Color> usedColors = new List<Color>();

        // Tạo tất cả jelly trước để đảm bảo không gặp lỗi khi tham chiếu
        for (int i = 0; i < maxCount; i++)
        {
            // Tạo jelly từ prefab
            GameObject jelly = Instantiate(jellyPrefab, slot);

            // Đặt tên cho jelly theo thứ tự (Jelly 1, Jelly 2, Jelly 3,...)
            jelly.name = "Jelly " + (i + 1).ToString(); // Đặt tên bắt đầu từ Jelly 1

            // Nếu chỉ có một Jelly, scale cả hai trục X và Y
            if (maxCount == 1)
            {
                jelly.transform.localScale = new Vector3(1.5f, 1.5f, 0.5f); // Ví dụ scale cả x và y
            }
            else
            {
                // Đặt kích thước cho jelly theo số lượng
                jelly.transform.localScale = CalculateScale(maxCount);
            }

            // Sắp xếp vị trí dựa trên số lượng jelly
            Vector3 position = CalculatePosition(i, maxCount); // Dựa trên chỉ số i
            jelly.transform.localPosition = position;

            // Chọn màu ngẫu nhiên từ danh sách màu cố định mà không cho phép trùng màu
            Color jellyColor;
            do
            {
                jellyColor = jellyColors[Random.Range(0, jellyColors.Count)];
            } while (usedColors.Contains(jellyColor)); // Kiểm tra xem màu đã được sử dụng chưa

            usedColors.Add(jellyColor); // Thêm màu vào danh sách màu đã sử dụng
            jelly.GetComponent<Renderer>().material.color = jellyColor; // Gán màu cho jelly

            // Thêm jelly vào từ điển với khóa là chỉ số (i)
            jellyInstances[i] = jelly;

            // Gán jellyID cho JellyMesh
            JellyMesh jellyScript = jelly.GetComponent<JellyMesh>();
            if (jellyScript != null)
            {
                jellyScript.jellyID = i + 1; // Gán jellyID dựa trên chỉ số i, bắt đầu từ 1
            }
        }

        // Bây giờ tất cả các jelly đã được tạo, chúng ta mới thiết lập connectObject
        for (int i = 0; i < maxCount; i++)
        {
            // Lấy script của Jelly để gán connectObject
            JellyMesh jellyScript = jellyInstances[i].GetComponent<JellyMesh>();

            if (jellyScript != null)
            {
                // Thiết lập connectObject theo cặp: 0 với 1, 1 với 0, 2 với 3, 3 với 2
                if (i % 2 == 0 && i + 1 < maxCount)
                {
                    jellyScript.connectObject = jellyInstances[i + 1]; // Phần tử chẵn kết nối với phần tử kế tiếp
                }
                else if (i % 2 != 0)
                {
                    jellyScript.connectObject = jellyInstances[i - 1]; // Phần tử lẻ kết nối với phần tử trước
                }
                else
                {
                    jellyScript.connectObject = null; // Trong trường hợp số phần tử là lẻ, phần tử cuối cùng không có cặp
                }
            }
        }
    }

    // Tính toán kích thước jelly dựa trên số lượng jelly
    private Vector3 CalculateScale(int count)
    {
        float scale = 1f / Mathf.Sqrt(count);  // Tính scale dựa trên căn bậc hai của số lượng
        return new Vector3(scale, scale, scale);
    }

    // Tính toán vị trí dựa trên số lượng jelly và vị trí của chúng
    private Vector3 CalculatePosition(int index, int count)
    {
        float offset = jellySpacing;  // Khoảng cách giữa các jelly, đã điều chỉnh bằng jellySpacing

        // Sử dụng vị trí trung tâm nếu chỉ có 1 jelly
        if (count == 1) return Vector3.zero;

        // Tính toán vị trí dựa trên số lượng jelly
        int rows = Mathf.CeilToInt(Mathf.Sqrt(count));  // Số hàng
        int cols = Mathf.CeilToInt((float)count / rows);  // Số cột

        // Xác định hàng và cột hiện tại dựa trên chỉ số jelly
        int row = index / cols;
        int col = index % cols;

        // Tính toán vị trí cho jelly, tăng khoảng cách giữa chúng
        return new Vector3((col - (cols - 1) / 2f) * offset, (row - (rows - 1) / 2f) * offset, 0);
    }

    // Xóa các jelly hiện tại trước khi tạo mới
    private void ClearExistingJellies()
    {
        foreach (GameObject jelly in jellyInstances.Values)
        {
            Destroy(jelly);  // Xóa jelly đã tạo trước đó
        }
        jellyInstances.Clear();  // Xóa từ điển
    }

    public void CheckJellyStatus()
    {
        // Kiểm tra sự tồn tại của tất cả các Jelly
        bool anyJellyExists = false;

        foreach (var jelly in jellyInstances.Values)
        {
            if (jelly != null)
            {
                anyJellyExists = true;
                break; // Nếu tìm thấy một Jelly, thoát khỏi vòng lặp
            }
        }

        // Nếu không còn Jelly nào tồn tại
        if (!anyJellyExists)
        {
            Debug.Log("No Jellies exist. Destroying parent object.");
            Destroy(slot.gameObject); // Hủy đối tượng cha (slot)
            return;
        }

        // Các logic kiểm tra và scale thông thường
        bool jelly1Exists = jellyInstances.ContainsKey(0) && jellyInstances[0] != null;
        bool jelly2Exists = jellyInstances.ContainsKey(1) && jellyInstances[1] != null;
        bool jelly3Exists = jellyInstances.ContainsKey(2) && jellyInstances[2] != null;
        bool jelly4Exists = jellyInstances.ContainsKey(3) && jellyInstances[3] != null;

        int existingJellyCount = (jelly1Exists ? 1 : 0) + (jelly2Exists ? 1 : 0) +
                                 (jelly3Exists ? 1 : 0) + (jelly4Exists ? 1 : 0);

        if (existingJellyCount == 1 && !isSingle)
        {
            Debug.Log("Only one Jelly exists. Scaling on both X and Y axis.");
            GameObject singleJelly = null;

            if (jelly1Exists)
                singleJelly = jellyInstances[0];
            else if (jelly2Exists)
                singleJelly = jellyInstances[1];
            else if (jelly3Exists)
                singleJelly = jellyInstances[2];
            else if (jelly4Exists)
                singleJelly = jellyInstances[3];

            if (singleJelly != null)
            {
                JellyMesh jellyScript = singleJelly.GetComponent<JellyMesh>();
                if (jellyScript != null)
                {
                    singleJelly.transform.localScale = new Vector3(1.0f, 1.0f, 0.5f);
                    Vector3 newLocalPosition = singleJelly.transform.localPosition;
                    newLocalPosition.y = 0;
                    singleJelly.transform.localPosition = newLocalPosition;

                    Debug.Log("Scaled the single Jelly on X and Y axis, and set local Y position to 0.");
                    isSingle = true;
                }
            }
        }
        else if (!hasScaled)
        {
            if (!jelly1Exists && !jelly2Exists && jelly3Exists && jelly4Exists)
            {
                Debug.Log("Jelly 1 and Jelly 2 do not exist. Scaling Jelly 3 and Jelly 4.");

                JellyMesh jelly3Script = jellyInstances[2].GetComponent<JellyMesh>();
                JellyMesh jelly4Script = jellyInstances[3].GetComponent<JellyMesh>();

                if (jelly3Script != null && jelly4Script != null)
                {
                    jelly3Script.inverse = true;
                    jelly4Script.inverse = true;
                    jelly3Script.Resize(0.5f, "y");
                    jelly4Script.Resize(0.5f, "y");
                    Debug.Log("Scaled Jelly 3 and Jelly 4 on Y axis.");
                    hasScaled = true;
                }
            }
            else if (!jelly3Exists && !jelly4Exists && jelly1Exists && jelly2Exists)
            {
                Debug.Log("Jelly 3 and Jelly 4 do not exist. Scaling Jelly 1 and Jelly 2.");

                JellyMesh jelly1Script = jellyInstances[0].GetComponent<JellyMesh>();
                JellyMesh jelly2Script = jellyInstances[1].GetComponent<JellyMesh>();

                if (jelly1Script != null && jelly2Script != null)
                {
                    jelly1Script.Resize(0.5f, "y");
                    jelly2Script.Resize(0.5f, "y");
                    Debug.Log("Scaled Jelly 1 and Jelly 2 on Y axis.");
                    hasScaled = true;
                }
            }
        }
    }

    // Kiểm tra trạng thái Jelly trong Update nếu cần
    void Update()
    {
        CheckJellyStatus();
    }
}
