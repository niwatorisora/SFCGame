using UnityEngine;

/// <summary>
/// ライフル武器の具象実装
/// WeaponBaseを継承してライフル特有の動作を実装
/// </summary>
public class RifleWeapon : WeaponBase
{
    [Header("Rifle Specific")]
    [SerializeField] private bool enableFullAuto = true;      // フルオート射撃
    [SerializeField] private float recoilAmount = 1.0f;       // リコイル量（将来用）
    
    protected override void OnBeforeFire()
    {
        base.OnBeforeFire();
        
        // ライフル特有の発射前処理
        LogDebug("ライフル発射準備");
    }
    
    protected override void OnAfterFire()
    {
        base.OnAfterFire();
        
        // ライフル特有の発射後処理（リコイル等）
        ApplyRecoil();
    }
    
    protected override void OnReloadStart()
    {
        base.OnReloadStart();
        LogDebug("ライフルリロード開始");
    }
    
    protected override void OnReloadComplete()
    {
        base.OnReloadComplete();
        LogDebug("ライフルリロード完了");
    }
    
    /// <summary>
    /// リコイル処理（現在は簡易実装）
    /// </summary>
    private void ApplyRecoil()
    {
        // 将来的にカメラの揺れやプレイヤーの向きに影響を与える
        LogDebug($"リコイル適用: {recoilAmount}");
    }
    
    private void LogDebug(string message)
    {
        if (enableDebugLogs)
        {
            Debug.Log($"[Rifle] {message}");
        }
    }
}
