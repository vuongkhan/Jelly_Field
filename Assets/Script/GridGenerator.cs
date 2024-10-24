using UnityEngine;
using System.Collections.Generic;

public class BlockManager : MonoBehaviour
{
    public GameObject jellyPrefab; // Prefab của thạch
    public Sprite blockPrefab; // Sprite của ô
    public int width = 5; // Số ô theo chiều rộng
    public int height = 5; 
    public float blockSize = 1.0f; 
    public float depth = -1.0f; 
    public int jellyCount = 5; 

    private List<Vector2Int> availablePositions = new List<Vector2Int>(); // Danh sách các vị trí trống còn lại

    void Start()
    {
        CreateGrid();
        CreateRandomJellies();
    }

    // Hàm tạo lưới
    void CreateGrid()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 position = new Vector3(x * blockSize, y * blockSize, depth); // Tạo vị trí trên mặt phẳng XY
                CreateBlock(position); // Tạo ô tại vị trí đó
                availablePositions.Add(new Vector2Int(x, y)); // Thêm vị trí này vào danh sách các vị trí có thể tạo thạch
            }
        }
    }

    // Hàm tạo thạch (Jelly) tại vị trí ngẫu nhiên
    void CreateRandomJellies()
    {
        for (int i = 0; i < jellyCount; i++)
        {
            if (availablePositions.Count == 0) break; // Nếu hết vị trí trống thì dừng lại

            int randomIndex = Random.Range(0, availablePositions.Count); // Chọn ngẫu nhiên vị trí trống
            Vector2Int randomPosition = availablePositions[randomIndex]; // Lấy vị trí ngẫu nhiên
            availablePositions.RemoveAt(randomIndex); // Xóa vị trí này khỏi danh sách

            // Tạo thạch tại vị trí đó
            Vector3 position = new Vector3(randomPosition.x * blockSize, randomPosition.y * blockSize, depth);
            CreateJelly(position);
        }
    }

    void CreateBlock(Vector3 position)
    {
        GameObject block = new GameObject("Block"); // Tạo một đối tượng mới cho ô trống
        block.transform.position = position; // Đặt vị trí cho đối tượng
        SpriteRenderer renderer = block.AddComponent<SpriteRenderer>();
        renderer.sprite = blockPrefab; // Gán Sprite từ blockPrefab

        // Thêm BoxCollider cho block (đối với môi trường 3D)
        BoxCollider collider = block.AddComponent<BoxCollider>();

        // Đặt Layer cho block (ví dụ: Layer có chỉ số 8, bạn có thể thay đổi theo Layer trong Unity)
        block.layer = LayerMask.NameToLayer("BlockLayer");

        // Đặt block làm con của BlockManager (hoặc một đối tượng mẹ khác nếu cần)
        block.transform.parent = this.transform;
    }

    void CreateJelly(Vector3 position)
    {
        GameObject jellyInstance = Instantiate(jellyPrefab, position + new Vector3(0, 0, -0.1f), Quaternion.identity);
        jellyInstance.transform.position = position + new Vector3(0, 0, -0.5f); // Đặt viên thạch hơi chìm vào ô

        // Xóa script JellyDrag nếu nó tồn tại
        JellyDrag jellyDrag = jellyInstance.GetComponent<JellyDrag>();
        if (jellyDrag != null)
        {
            Destroy(jellyDrag); // Xóa script JellyDrag khỏi đối tượng Jelly
        }

        // Đặt jellyInstance làm con của BlockManager (hoặc một đối tượng mẹ khác nếu cần)
        jellyInstance.transform.parent = this.transform;
    }
}
