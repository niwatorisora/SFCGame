using System.Collections;
using UnityEngine;

/// <summary>
/// 武器の基本クラス
/// Template Methodパターンで共通処理を実装し、具体的な武器で特殊処理をオーバーライド可能
/// </summary>
public abstract class WeaponBase : MonoBehaviour, IWeapon
{
    [Header("Weapon Setup")]
    [SerializeField] protected Transform firePoint;
    [SerializeField] protected WeaponDataSO weaponData;
    
    [Header("Debug")]
    [SerializeField] protected bool enableDebugLogs = true;
    
    [Header("Audio")]
    [SerializeField] protected AudioSource audioSource;
    
    // IWeapon実装
    public int CurrentAmmo => currentAmmo;
    public bool IsReloading => isReloading;
    
    // 内部状態
    protected int currentAmmo;
    protected bool isReloading;
    protected float lastFireTime;
    protected bool isInitialized = false;
    
    #region Unity Events
    
    protected virtual void Awake()
    {
        ValidateComponents();
        SetupAudioSource();
    }
    
    protected virtual void Start()
    {
        if (weaponData != null && firePoint != null)
        {
            Initialize(firePoint, weaponData);
        }
    }
    
    #endregion
    
    #region IWeapon Implementation
    
    public virtual void Initialize(Transform firePoint, IWeaponData weaponData)
    {
        if (firePoint == null)
        {
            Debug.LogError($"WeaponBase: firePointがnullです - {name}");
            return;
        }
        
        if (weaponData == null)
        {
            Debug.LogError($"WeaponBase: weaponDataがnullです - {name}");
            return;
        }
        
        this.firePoint = firePoint;
        this.weaponData = weaponData as WeaponDataSO;
        
        // 初期弾薬設定
        currentAmmo = weaponData.MagazineSize;
        isReloading = false;
        lastFireTime = 0f;
        isInitialized = true;
        
        LogDebug($"武器初期化完了: {weaponData.WeaponName} 弾薬: {currentAmmo}");
    }
    
    public virtual void Fire()
    {
        if (!CanFire())
        {
            // 弾切れの場合は弾切れ音を再生
            if (currentAmmo <= 0 && !isReloading)
            {
                PlaySound(weaponData.EmptySound);
                LogDebug("弾切れ: 空撃ち音再生");
            }
            LogDebug($"発射不可: リロード中={isReloading}, 弾薬={currentAmmo}, クールダウン={!IsCooldownComplete()}");
            return;
        }
        
        // 発射前処理（オーバーライド可能）
        OnBeforeFire();
        
        // 発射音再生
        PlaySound(weaponData.FireSound);
        
        // 弾丸生成と発射
        FireBullet();
        
        // 弾薬消費と時間記録
        currentAmmo--;
        lastFireTime = Time.time;
        
        // 発射後処理（オーバーライド可能）
        OnAfterFire();
        
        LogDebug($"発射: {weaponData.WeaponName} 残弾: {currentAmmo}");
    }
    
    public virtual void Reload()
    {
        if (!CanReload())
        {
            LogDebug($"リロード不可: 既にリロード中={isReloading}, 弾薬満タン={currentAmmo >= weaponData.MagazineSize}");
            return;
        }
        
        StartCoroutine(ReloadCoroutine());
    }
    
    public virtual bool CanFire()
    {
        return isInitialized 
            && !isReloading 
            && currentAmmo > 0 
            && IsCooldownComplete();
    }
    
    #endregion
    
    #region Protected Methods (継承先でオーバーライド可能)
    
    /// <summary>
    /// 発射前の処理（アニメーション、エフェクト等）
    /// </summary>
    protected virtual void OnBeforeFire()
    {
        // 継承先で実装
    }
    
    /// <summary>
    /// 発射後の処理（リコイル、エフェクト等）
    /// </summary>
    protected virtual void OnAfterFire()
    {
        // 継承先で実装
    }
    
    /// <summary>
    /// リロード開始時の処理
    /// </summary>
    protected virtual void OnReloadStart()
    {
        PlaySound(weaponData.ReloadStartSound);
        LogDebug("リロード開始音再生");
    }
    
    /// <summary>
    /// リロード完了時の処理
    /// </summary>
    protected virtual void OnReloadComplete()
    {
        PlaySound(weaponData.ReloadCompleteSound);
        LogDebug("リロード完了音再生");
    }
    
    #endregion
    
    #region Private Methods
    
    private void ValidateComponents()
    {
        if (firePoint == null)
        {
            // 子オブジェクトからFirePointを探す
            Transform foundFirePoint = transform.Find("FirePoint");
            if (foundFirePoint != null)
            {
                firePoint = foundFirePoint;
                LogDebug($"FirePointを自動検出: {foundFirePoint.name}");
            }
            else
            {
                Debug.LogWarning($"WeaponBase: FirePointが設定されていません - {name}");
            }
        }
    }
    
    private void SetupAudioSource()
    {
        if (audioSource == null)
        {
            // AudioSourceコンポーネントを探す
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                // AudioSourceが無い場合は自動で追加
                audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.playOnAwake = false;
                audioSource.spatialBlend = 1.0f; // 3D空間音響
                LogDebug("AudioSourceを自動追加");
            }
        }
    }
    
    private void FireBullet()
    {
        if (weaponData.BulletPrefab == null)
        {
            Debug.LogError($"WeaponBase: BulletPrefabが設定されていません - {weaponData.WeaponName}");
            return;
        }
        
        // BulletPoolから弾丸を取得
        GameObject bullet = BulletPool.Instance.GetBullet(weaponData.BulletPrefab);
        if (bullet == null)
        {
            Debug.LogError($"WeaponBase: 弾丸の取得に失敗 - {weaponData.WeaponName}");
            return;
        }
        
        // 発射位置・回転を設定
        bullet.transform.position = firePoint.position;
        bullet.transform.rotation = firePoint.rotation;
        
        // 初速を計算（前方向 + 重力の影響を考慮）
        Vector3 velocity = firePoint.forward * weaponData.MuzzleVelocity;
        
        // 弾丸を初期化
        IBullet bulletComponent = bullet.GetComponent<IBullet>();
        if (bulletComponent != null)
        {
            bulletComponent.Initialize(weaponData.Damage, velocity);
        }
        else
        {
            Debug.LogError($"WeaponBase: 弾丸にIBulletが実装されていません - {bullet.name}");
        }
    }
    
    private bool CanReload()
    {
        return isInitialized 
            && !isReloading 
            && currentAmmo < weaponData.MagazineSize;
    }
    
    private bool IsCooldownComplete()
    {
        float fireInterval = 60f / weaponData.FireRate; // RPMを秒間隔に変換
        return Time.time >= lastFireTime + fireInterval;
    }
    
    private IEnumerator ReloadCoroutine()
    {
        isReloading = true;
        OnReloadStart();
        
        LogDebug($"リロード開始: {weaponData.WeaponName} {weaponData.ReloadTime}秒");
        
        yield return new WaitForSeconds(weaponData.ReloadTime);
        
        currentAmmo = weaponData.MagazineSize;
        isReloading = false;
        OnReloadComplete();
        
        LogDebug($"リロード完了: {weaponData.WeaponName} 残弾: {currentAmmo}");
    }
    
    /// <summary>
    /// 音響効果を再生
    /// </summary>
    protected void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
    
    private void LogDebug(string message)
    {
        if (enableDebugLogs)
        {
            Debug.Log($"[Weapon] {message}");
        }
    }
    
    #endregion
    
    #region Debug Methods
    
    [ContextMenu("Debug: Fire Weapon")]
    private void DebugFire()
    {
        Fire();
    }
    
    [ContextMenu("Debug: Reload Weapon")]
    private void DebugReload()
    {
        Reload();
    }
    
    [ContextMenu("Debug: Show Status")]
    private void DebugShowStatus()
    {
        Debug.Log($"=== {name} Status ===");
        Debug.Log($"初期化済み: {isInitialized}");
        Debug.Log($"残弾: {currentAmmo}/{(weaponData != null ? weaponData.MagazineSize : 0)}");
        Debug.Log($"リロード中: {isReloading}");
        Debug.Log($"発射可能: {CanFire()}");
        Debug.Log($"クールダウン完了: {IsCooldownComplete()}");
    }
    
    #endregion
}
