using System.Collections;
using UnityEngine;

/// <summary>
/// 弾丸の基本クラス
/// RigidbodyとRaycastを組み合わせて判定抜けを防ぐ
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class BulletBase : MonoBehaviour, IBullet
{
    [Header("Bullet Settings")]
    [SerializeField] private float lifetime = 5f;           // 弾丸の生存時間
    [SerializeField] private LayerMask collisionLayers = -1; // 衝突対象レイヤー
    
    private Rigidbody bulletRigidbody;
    private float damage;
    private Vector3 previousPosition;
    private bool hasCollided = false;
    private Coroutine lifetimeCoroutine;
    
    // IBullet実装
    public float Damage => damage;
    
    void Awake()
    {
        bulletRigidbody = GetComponent<Rigidbody>();
        
        // Rigidbodyの設定
        bulletRigidbody.useGravity = true;
        bulletRigidbody.linearDamping = 0f;
        bulletRigidbody.angularDamping = 0f;
    }
    
    void Start()
    {
        previousPosition = transform.position;
        
        // 生存時間タイマー開始
        lifetimeCoroutine = StartCoroutine(LifetimeCoroutine());
    }
    
    void FixedUpdate()
    {
        if (!hasCollided)
        {
            CheckCollisionWithRaycast();
            previousPosition = transform.position;
        }
    }
    
    /// <summary>
    /// 前フレームから現フレームまでの移動をRaycastで確認
    /// </summary>
    private void CheckCollisionWithRaycast()
    {
        Vector3 direction = transform.position - previousPosition;
        float distance = direction.magnitude;
        
        // 最小移動距離以上の場合のみチェック
        if (distance > 0.01f)
        {
            if (Physics.Raycast(previousPosition, direction.normalized, out RaycastHit hit, distance, collisionLayers))
            {
                OnHitTarget(hit);
            }
        }
    }
    
    /// <summary>
    /// 衝突処理
    /// </summary>
    private void OnHitTarget(RaycastHit hit)
    {
        if (hasCollided) return;
        
        hasCollided = true;
        
        // デバッグログ出力
        Debug.Log($"弾丸衝突: {hit.collider.name} 距離: {hit.distance:F2}m ダメージ: {damage}");
        
        // 衝突位置に移動
        transform.position = hit.point;
        
        // 物理を停止
        bulletRigidbody.linearVelocity = Vector3.zero;
        bulletRigidbody.isKinematic = true;
        
        // プールに戻す
        ReturnToPool();
    }
    
    /// <summary>
    /// 弾丸の初期化
    /// </summary>
    public void Initialize(float bulletDamage, Vector3 velocity)
    {
        damage = bulletDamage;
        hasCollided = false;
        previousPosition = transform.position;
        
        // 物理設定をリセット
        bulletRigidbody.isKinematic = false;
        bulletRigidbody.linearVelocity = velocity;
        
        // 生存時間タイマー開始
        if (lifetimeCoroutine != null)
        {
            StopCoroutine(lifetimeCoroutine);
        }
        lifetimeCoroutine = StartCoroutine(LifetimeCoroutine());
    }
    
    /// <summary>
    /// プールに戻す
    /// </summary>
    public void ReturnToPool()
    {
        if (lifetimeCoroutine != null)
        {
            StopCoroutine(lifetimeCoroutine);
            lifetimeCoroutine = null;
        }
        
        // BulletPoolが存在する場合はプールに戻す
        if (BulletPool.Instance != null)
        {
            BulletPool.Instance.ReturnBullet(gameObject);
        }
        else
        {
            // プールが無い場合は破棄
            Destroy(gameObject);
        }
    }
    
    /// <summary>
    /// 生存時間管理
    /// </summary>
    private IEnumerator LifetimeCoroutine()
    {
        yield return new WaitForSeconds(lifetime);
        
        if (!hasCollided)
        {
            Debug.Log($"弾丸の生存時間終了: {name}");
            ReturnToPool();
        }
    }
    
    /// <summary>
    /// 物理的な衝突のバックアップ（Raycastで検出されなかった場合）
    /// </summary>
    void OnTriggerEnter(Collider other)
    {
        if (!hasCollided && IsValidTarget(other))
        {
            // Raycastで検出されなかった衝突のバックアップ
            RaycastHit hit = new RaycastHit();
            // hit情報を手動で構築
            OnHitTarget(hit);
        }
    }
    
    private bool IsValidTarget(Collider collider)
    {
        // レイヤーマスクでフィルタリング
        return (collisionLayers.value & (1 << collider.gameObject.layer)) != 0;
    }
}
