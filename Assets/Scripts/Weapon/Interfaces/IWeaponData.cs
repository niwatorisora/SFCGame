using UnityEngine;

/// <summary>
/// 武器データの基本インターフェース
/// ScriptableObjectや他のデータソースを統一的に扱う
/// </summary>
public interface IWeaponData
{
    string WeaponName { get; }
    GameObject BulletPrefab { get; }
    float MuzzleVelocity { get; }
    float Damage { get; }
    float FireRate { get; }
    int MagazineSize { get; }
    float ReloadTime { get; }
}
