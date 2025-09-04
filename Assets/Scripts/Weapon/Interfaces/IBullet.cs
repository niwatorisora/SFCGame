using UnityEngine;

/// <summary>
/// 弾丸の基本インターフェース
/// 異なる弾丸タイプを統一的に扱うためのインターフェース
/// </summary>
public interface IBullet
{
    /// <summary>
    /// 弾丸の初期化
    /// </summary>
    /// <param name="damage">ダメージ値</param>
    /// <param name="velocity">初速ベクトル</param>
    void Initialize(float damage, Vector3 velocity);
    
    /// <summary>
    /// 弾丸をプールに戻す
    /// </summary>
    void ReturnToPool();
    
    /// <summary>
    /// 弾丸のダメージ値
    /// </summary>
    float Damage { get; }
}
