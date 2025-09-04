using UnityEngine;

/// <summary>
/// プレイヤーの武器管理システム
/// プレイヤーの子オブジェクトとして武器を管理し、入力を処理する
/// 将来的な武器切り替えシステムに対応
/// </summary>
public class WeaponHolder : MonoBehaviour
{
    [Header("Weapon Setup")]
    [SerializeField] private Transform weaponAttachPoint;      // 武器装着位置
    [SerializeField] private WeaponBase currentWeapon;         // 現在装備中の武器
    
    [Header("Input")]
    [SerializeField] private bool enableWeaponInput = true;    // 武器入力の有効/無効
    
    [Header("Debug")]
    [SerializeField] private bool enableDebugLogs = true;
    
    // 将来の武器切り替えシステム用（現在は未実装）
    // private List<WeaponBase> availableWeapons;
    // private int currentWeaponIndex = 0;
    
    private IInputProvider inputProvider;
    private bool isInitialized = false;
    
    #region Unity Events
    
    void Awake()
    {
        FindComponents();
    }
    
    void Start()
    {
        Initialize();
    }
    
    void Update()
    {
        if (isInitialized && enableWeaponInput)
        {
            HandleWeaponInput();
        }
    }
    
    #endregion
    
    #region Initialization
    
    private void FindComponents()
    {
        // IInputProviderを取得（同じGameObjectまたは親から）
        inputProvider = GetComponent<IInputProvider>();
        if (inputProvider == null)
        {
            inputProvider = GetComponentInParent<IInputProvider>();
        }
        
        if (inputProvider == null)
        {
            Debug.LogError($"WeaponHolder: IInputProviderが見つかりません - {name}");
        }
        
        // 武器装着ポイントが設定されていない場合は自動検索
        if (weaponAttachPoint == null)
        {
            weaponAttachPoint = transform.Find("WeaponAttachPoint");
            if (weaponAttachPoint == null)
            {
                // デフォルトの装着ポイントを作成
                GameObject attachPoint = new GameObject("WeaponAttachPoint");
                attachPoint.transform.SetParent(transform);
                attachPoint.transform.localPosition = Vector3.zero;
                weaponAttachPoint = attachPoint.transform;
                LogDebug("WeaponAttachPointを自動作成しました");
            }
        }
    }
    
    private void Initialize()
    {
        if (inputProvider == null)
        {
            Debug.LogError($"WeaponHolder: 初期化失敗 - IInputProviderが必要です");
            return;
        }
        
        // 現在の武器を装着ポイントに配置（プレハブの場合は実体化）
        if (currentWeapon != null)
        {
            WeaponBase weaponPrefab = currentWeapon;
            currentWeapon = null; // 一旦クリアしてからAttachWeaponを呼ぶ
            AttachWeapon(weaponPrefab);
        }
        
        isInitialized = true;
        LogDebug($"WeaponHolder初期化完了 - 武器: {(currentWeapon != null ? currentWeapon.name : "なし")}");
    }
    
    #endregion
    
    #region Input Handling
    
    private void HandleWeaponInput()
    {
        if (currentWeapon == null) return;
        
        // 発射入力
        if (inputProvider.GetFireInput())
        {
            currentWeapon.Fire();
        }
        
        // リロード入力
        if (inputProvider.GetReloadInput())
        {
            currentWeapon.Reload();
        }
    }
    
    #endregion
    
    #region Weapon Management
    
    /// <summary>
    /// 武器を装着する（プレハブから実体化）
    /// </summary>
    public void AttachWeapon(WeaponBase weaponPrefab)
    {
        if (weaponPrefab == null)
        {
            Debug.LogError("WeaponHolder: 装着しようとした武器がnullです");
            return;
        }
        
        // 現在の武器を取り外し
        if (currentWeapon != null)
        {
            DetachCurrentWeapon();
        }
        
        // プレハブから実体を作成
        GameObject weaponInstance = Instantiate(weaponPrefab.gameObject, weaponAttachPoint);
        currentWeapon = weaponInstance.GetComponent<WeaponBase>();
        
        // 位置とローテーションを設定
        currentWeapon.transform.localPosition = Vector3.zero;
        currentWeapon.transform.localRotation = Quaternion.identity;
        
        LogDebug($"武器装着: {currentWeapon.name}");
        
        // 武器の状態をデバッグ表示
        ShowWeaponStatus();
    }
    
    /// <summary>
    /// 現在の武器を取り外す（実体を破棄）
    /// </summary>
    public void DetachCurrentWeapon()
    {
        if (currentWeapon != null)
        {
            LogDebug($"武器取り外し: {currentWeapon.name}");
            
            // 実体化された武器を破棄
            if (Application.isPlaying)
            {
                Destroy(currentWeapon.gameObject);
            }
            else
            {
                DestroyImmediate(currentWeapon.gameObject);
            }
            
            currentWeapon = null;
        }
    }
    
    /// <summary>
    /// 現在装備中の武器を取得
    /// </summary>
    public WeaponBase GetCurrentWeapon()
    {
        return currentWeapon;
    }
    
    /// <summary>
    /// 武器が装備されているかチェック
    /// </summary>
    public bool HasWeapon()
    {
        return currentWeapon != null;
    }
    
    #endregion
    
    #region Future Weapon Switching (準備中)
    
    // 将来実装予定：武器切り替えシステム
    /*
    public void SwitchWeapon(int weaponIndex)
    {
        // 武器切り替えロジック
    }
    
    public void AddWeapon(WeaponBase weapon)
    {
        // 武器をインベントリに追加
    }
    
    public void RemoveWeapon(WeaponBase weapon)
    {
        // 武器をインベントリから削除
    }
    */
    
    #endregion
    
    #region Debug Methods
    
    [ContextMenu("Debug: Show Weapon Status")]
    private void ShowWeaponStatus()
    {
        if (currentWeapon == null)
        {
            Debug.Log("WeaponHolder: 武器が装備されていません");
            return;
        }
        
        Debug.Log($"=== WeaponHolder Status ===");
        Debug.Log($"装備武器: {currentWeapon.name}");
        Debug.Log($"残弾: {currentWeapon.CurrentAmmo}");
        Debug.Log($"リロード中: {currentWeapon.IsReloading}");
        Debug.Log($"発射可能: {currentWeapon.CanFire()}");
    }
    
    [ContextMenu("Debug: Test Fire")]
    private void DebugTestFire()
    {
        if (currentWeapon != null)
        {
            currentWeapon.Fire();
        }
        else
        {
            Debug.Log("WeaponHolder: 武器が装備されていません");
        }
    }
    
    [ContextMenu("Debug: Test Reload")]
    private void DebugTestReload()
    {
        if (currentWeapon != null)
        {
            currentWeapon.Reload();
        }
        else
        {
            Debug.Log("WeaponHolder: 武器が装備されていません");
        }
    }
    
    private void LogDebug(string message)
    {
        if (enableDebugLogs)
        {
            Debug.Log($"[WeaponHolder] {message}");
        }
    }
    
    #endregion
}
