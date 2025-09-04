using UnityEngine;

/// <summary>
/// プレイヤーコントローラー - Composite Patternの実装
/// 各システムを組み合わせて、プレイヤーの全体的な動作を調整する
/// 
/// SOLID原則の適用：
/// - SRP: プレイヤーの制御調整のみを責務とする
/// - OCP: 新しいシステムを既存コードを変更せずに追加可能
/// - DIP: 具象クラスではなく、インターフェースに依存
/// - LSP: インターフェースの実装により置換可能性を保証
/// 
/// デザインパターン：
/// - Composite: 複数のシステムを組み合わせて一つの機能を実現
/// - Strategy: 実行時に異なる戦略（入力方式、移動方式）を使用可能
/// </summary>
public class PlayerController : MonoBehaviour
{
    private Rigidbody playerRigidbody;
    private IInputProvider inputProvider;
    private IMovementSystem movementSystem;
    private WeaponHolder weaponHolder;

    [SerializeField] private float playerLookSpeed = 2.0f; // プレイヤーの回転速度

    void Start()
    {
        InitializeSystems();
    }

    void Update()
    {
        HandleInput();
    }
    private void InitializeSystems()
    {
        playerRigidbody = GetComponent<Rigidbody>();
        inputProvider = GetComponent<StandardInputProvider>();
        movementSystem = GetComponent<GroundMovementSystem>();
        movementSystem.Initialize(playerRigidbody);
        
        // WeaponHolderを取得（同じGameObjectまたは子オブジェクトから）
        weaponHolder = GetComponent<WeaponHolder>();
        if (weaponHolder == null)
        {
            weaponHolder = GetComponentInChildren<WeaponHolder>();
        }
        
        if (weaponHolder != null)
        {
            Debug.Log("[Player] WeaponHolder統合完了");
        }
    }
    private void HandleInput()
    {
        // 移動入力の処理
        float horizontalInput = inputProvider.GetHorizontalInput();
        float verticalInput = inputProvider.GetVerticalInput();
        Vector3 moveInput = new Vector3(horizontalInput, 0f, verticalInput);

        // 移動システムに移動指示を送信
        movementSystem.Move(moveInput, Time.deltaTime);

        // プレイヤーの水平回転を処理
        HandlePlayerRotation();

        // ジャンプ入力の処理
        if (inputProvider.GetJumpInput())
        {
            movementSystem.Jump();
        }
    }
    private void HandlePlayerRotation()
    {
        float mouseX = inputProvider.GetMouseXInput() * playerLookSpeed;
        transform.Rotate(Vector3.up * mouseX);
    }
}
