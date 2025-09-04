using UnityEngine;

/// <summary>
/// 地上移動システムの具象実装
/// Strategy Patternの具象戦略として実装
/// 
/// 学習ポイント：
/// - 単一責任原則：移動処理のみを担当
/// - 設定可能なパラメータにより、異なるキャラクターに再利用可能
/// - 物理演算を適切に使用したリアルな移動感
/// </summary>
public class GroundMovementSystem : MonoBehaviour, IMovementSystem
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private float groundCheckDistance = 0.1f;
    [SerializeField] private LayerMask groundLayer = 1;
    
    private Rigidbody targetRigidbody;
    private bool isGrounded;
    
    public void Initialize(Rigidbody rigidbody)
    {
        targetRigidbody = rigidbody;
        
        // Rigidbodyの設定を最適化
        if (targetRigidbody != null)
        {
            targetRigidbody.freezeRotation = true; // 不要な回転を防ぐ
        }
    }
    
    void FixedUpdate()
    {
        CheckGrounded();
    }
    
    public void Move(Vector3 moveInput, float deltaTime)
    {
        if (targetRigidbody == null) return;

        // 現在のTransformの前方向・右方向を使って移動ベクトルを計算
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        // Y軸方向は0にして、水平面上の移動に限定
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        // 入力に応じて移動方向を決定
        Vector3 moveDirection = (forward * moveInput.z + right * moveInput.x).normalized;

        // 移動速度を適用
        Vector3 movement = moveDirection * moveSpeed;

        // 現在の垂直速度を保持しつつ、水平移動を適用
        Vector3 newVelocity = new Vector3(movement.x, targetRigidbody.linearVelocity.y, movement.z);
        targetRigidbody.linearVelocity = newVelocity;
    }
    
    public void Jump()
    {
        if (targetRigidbody == null || !isGrounded) return;
        
        // 垂直方向にのみジャンプ力を適用
        targetRigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }
    
    /// <summary>
    /// 地面との接触判定
    /// より正確な判定のためRaycastを使用
    /// </summary>
    private void CheckGrounded()
    {
        if (targetRigidbody == null) return;
        
        Vector3 rayStart = transform.position;
        isGrounded = Physics.Raycast(rayStart, Vector3.down, groundCheckDistance, groundLayer);
        
        // デバッグ用の視覚化（開発時のみ）
        Debug.DrawRay(rayStart, Vector3.down * groundCheckDistance, isGrounded ? Color.green : Color.red);
    }
}
