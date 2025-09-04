using UnityEngine;

/// <summary>
/// 移動処理の抽象化インターフェース
/// Strategy Patternの一部として、異なる移動方式（地上、空中、水中等）を
/// 統一的に扱えるようにする
/// 
/// SOLID原則：
/// - SRP: 移動に関する処理のみを責務とする
/// - DIP: 具体的な移動実装ではなく、抽象化に依存
/// - OCP: 新しい移動方式を既存コードを変更せずに追加可能
/// </summary>
public interface IMovementSystem
{
    /// <summary>
    /// 移動処理を実行
    /// </summary>
    /// <param name="moveInput">移動方向の入力ベクトル</param>
    /// <param name="deltaTime">経過時間</param>
    void Move(Vector3 moveInput, float deltaTime);
    
    /// <summary>
    /// ジャンプ処理を実行
    /// </summary>
    void Jump();
    
    /// <summary>
    /// 移動システムの初期化
    /// </summary>
    /// <param name="rigidbody">制御対象のRigidbody</param>
    void Initialize(Rigidbody rigidbody);
}
