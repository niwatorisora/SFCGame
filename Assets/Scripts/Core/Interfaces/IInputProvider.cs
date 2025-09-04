using UnityEngine;

/// <summary>
/// 入力処理の抽象化インターフェース
/// Strategy Patternの一部として、異なる入力方式（キーボード、ゲームパッド、AI等）を
/// 統一的に扱えるようにする
/// 
/// SOLID原則：
/// - DIP: 具体的な入力システムではなく、抽象化に依存
/// - OCP: 新しい入力方式を既存コードを変更せずに追加可能
/// </summary>
public interface IInputProvider
{
    /// <summary>
    /// 水平方向の入力値を取得 (-1.0f ～ 1.0f)
    /// </summary>
    float GetHorizontalInput();
    
    /// <summary>
    /// 垂直方向の入力値を取得 (-1.0f ～ 1.0f)
    /// </summary>
    float GetVerticalInput();
    
    /// <summary>
    /// ジャンプ入力の状態を取得
    /// </summary>
    bool GetJumpInput();

    /// <summary>
    /// マウスのX軸の入力値を取得
    /// </summary>
    float GetMouseXInput();

    /// <summary>
    /// マウスのY軸の入力値を取得
    /// </summary>
    float GetMouseYInput();

    /// <summary>
    /// 発射入力の状態を取得
    /// </summary>
    bool GetFireInput();

    /// <summary>
    /// リロード入力の状態を取得
    /// </summary>
    bool GetReloadInput();
}
