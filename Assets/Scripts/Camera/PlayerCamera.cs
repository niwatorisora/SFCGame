using UnityEngine;

namespace SFCGame.Camera
{
    /// <summary>
    /// プレイヤーのカメラ制御スクリプト
    /// 一人称視点カメラの垂直方向（上下）の回転を処理します。
    /// </summary>
    public class PlayerCamera : MonoBehaviour
    {
        [SerializeField] private Transform playerBody; // プレイヤーのTransformをInspectorから設定
        [SerializeField] private float lookSpeed = 2.0f; // カメラの回転速度
        [SerializeField] private float lookXLimit = 90.0f; // 上下の見上げ制限角度

        private IInputProvider inputProvider; // 入力プロバイダー
        private float rotationX = 0; // X軸周りの回転角度（垂直方向）

        void Start()
        {
            if (playerBody == null)
            {
                Debug.LogError("PlayerCamera: playerBodyが設定されていません。");
                enabled = false; // スクリプトを無効にする
                return;
            }

            inputProvider = playerBody.GetComponent<IInputProvider>(); // プレイヤーオブジェクトからIInputProviderを取得
            if (inputProvider == null)
            {
                Debug.LogError("PlayerCamera: playerBodyにIInputProviderが見つかりません。");
                enabled = false; // スクリプトを無効にする
                return;
            }

            // カーソルをロックし、非表示にする
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        void LateUpdate()
        {
            HandleVerticalLook();
        }

        private void HandleVerticalLook()
        {
            // マウスのY軸入力に基づいて垂直方向の回転を計算
            rotationX += -inputProvider.GetMouseYInput() * lookSpeed;
            // 垂直方向の回転を制限
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);

            // カメラのローカル回転を設定 (PlayerCamera自身が回転)
            transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        }
    }
}
