using UnityEngine;

/// <summary>
/// 武器の基本インターフェース
/// Strategy Patternの一部として、異なる武器タイプを統一的に扱う
/// </summary>
public interface IWeapon
{
    /// <summary>
    /// 発射処理
    /// </summary>
    void Fire();
    
    /// <summary>
    /// リロード処理
    /// </summary>
    void Reload();
    
    /// <summary>
    /// 発射可能かどうか判定
    /// </summary>
    bool CanFire();
    
    /// <summary>
    /// 武器の初期化
    /// </summary>
    /// <param name="firePoint">発射位置</param>
    /// <param name="weaponData">武器データ</param>
    void Initialize(Transform firePoint, IWeaponData weaponData);
    
    /// <summary>
    /// 現在の残弾数
    /// </summary>
    int CurrentAmmo { get; }
    
    /// <summary>
    /// リロード中かどうか
    /// </summary>
    bool IsReloading { get; }
}
