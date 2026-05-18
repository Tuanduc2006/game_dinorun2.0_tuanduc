using UnityEngine;

public class DinoController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;

    [Header("Game Manager")]
    public GameManager gameManager;

    [Header("Cai dat Nhay")]
    public float jumpForce = 10f;
    public float jumpTime = 0.35f;
    private float jumpTimeCounter;
    private bool isJumping;

    [Header("Kiem tra Cham dat")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask whatIsGround;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        // Tự động tìm GameManager nếu lỡ quên kéo thả ngoài Inspector
        if (gameManager == null)
        {
            gameManager = FindFirstObjectByType<GameManager>();
        }
    }

    void Update()
    {
        // 1. NẾU GAME CHƯA BẮT ĐẦU: Không cho phép làm gì cả (Chặn lỗi bấm Start mà khủng long nhảy)
        if (gameManager != null && !gameManager.isGameStarted) return;

        // 2. NẾU ĐÃ CHẾT: Không cho nhảy nữa
        if (anim.GetBool("isDead")) return;

        // Kiểm tra xem khủng long có đang chạm đất không
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);

        // KHI CHẠM ĐẤT: Tắt hiệu ứng nhảy để chuyển về hiệu ứng chạy
        if (isGrounded && rb.linearVelocity.y <= 0)
        {
            anim.SetBool("isJumping", false);
        }

        // KHI BẤM NHẢY: Lấy đà bật lên và bật hiệu ứng nhảy
        if (Input.GetMouseButtonDown(0) && isGrounded)
        {
            isJumping = true;
            jumpTimeCounter = jumpTime;
            rb.linearVelocity = Vector2.up * jumpForce;

            anim.SetBool("isJumping", true);
        }

        // KHI GIỮ CHUỘT/CHẠM: Giúp nhảy cao hơn một chút
        if (Input.GetMouseButton(0) && isJumping)
        {
            if (jumpTimeCounter > 0)
            {
                rb.linearVelocity = Vector2.up * jumpForce;
                jumpTimeCounter -= Time.deltaTime;
            }
            else
            {
                isJumping = false;
            }
        }

        // KHI THẢ TAY RA: Ngừng việc nhảy cao thêm
        if (Input.GetMouseButtonUp(0))
        {
            isJumping = false;
        }
    }

    // HÀM XỬ LÝ VA CHẠM: Tự động chạy khi chạm vào chướng ngại vật
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Obstacle"))
        {
            // Bật hiệu ứng chết (chuyển sang ảnh dino_hit)
            anim.SetBool("isDead", true);

            // Báo cho GameManager đóng băng game
            if (gameManager != null)
            {
                gameManager.GameOver();
            }
        }
    }
}