using UnityEngine;

/// <summary>
/// 標準的なキーボード・マウス入力を提供する具象クラス
/// Strategy Patternの具象戦略として実装
/// 
/// 学習ポイント：
/// - インターフェースの実装により、入力方式を簡単に変更可能
/// - 将来的にゲームパッド用、AI用の入力プロバイダーを追加可能
/// - テスト時にはモック入力プロバイダーを使用可能
/// </summary>
public class StandardInputProvider : MonoBehaviour, IInputProvider
{
    [Header("Input Settings")]
    [SerializeField] private string horizontalAxis = "Horizontal";
    [SerializeField] private string verticalAxis = "Vertical";
    [SerializeField] private string jumpButton = "Jump";
    
    public float GetHorizontalInput()
    {
        return Input.GetAxis(horizontalAxis);
    }
    
    public float GetVerticalInput()
    {
        return Input.GetAxis(verticalAxis);
    }
    
    public bool GetJumpInput()
    {
        return Input.GetButtonDown(jumpButton);
    }


    public float GetMouseXInput()
    {
        return Input.GetAxis("Mouse X");
    }

    public float GetMouseYInput()
    {
        return Input.GetAxis("Mouse Y");
    }
}
