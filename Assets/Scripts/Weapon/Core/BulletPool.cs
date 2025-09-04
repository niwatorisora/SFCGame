using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 弾丸のオブジェクトプールシステム
/// パフォーマンス向上のため弾丸オブジェクトを再利用する
/// </summary>
public class BulletPool : MonoBehaviour
{
    public static BulletPool Instance { get; private set; }
    
    [Header("Pool Settings")]
    [SerializeField] private int defaultPoolSize = 50;
    [SerializeField] private Transform poolParent; // プールオブジェクトの親
    
    // プレハブタイプごとのプール
    private Dictionary<GameObject, Queue<GameObject>> bulletPools;
    private Dictionary<GameObject, GameObject> activeBullets; // アクティブな弾丸の追跡
    
    void Awake()
    {
        // シングルトンパターン
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializePools();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void InitializePools()
    {
        bulletPools = new Dictionary<GameObject, Queue<GameObject>>();
        activeBullets = new Dictionary<GameObject, GameObject>();
        
        // プール用の親オブジェクトを作成
        if (poolParent == null)
        {
            GameObject poolParentObj = new GameObject("PoolContainer");
            poolParentObj.transform.SetParent(transform);
            poolParent = poolParentObj.transform;
        }
    }
    
    /// <summary>
    /// プールから弾丸を取得
    /// </summary>
    public GameObject GetBullet(GameObject bulletPrefab)
    {
        if (bulletPrefab == null)
        {
            Debug.LogError("BulletPool: bulletPrefabがnullです");
            return null;
        }
        
        // プールが存在しない場合は作成
        if (!bulletPools.ContainsKey(bulletPrefab))
        {
            CreatePool(bulletPrefab);
        }
        
        var pool = bulletPools[bulletPrefab];
        GameObject bullet;
        
        if (pool.Count > 0)
        {
            // プールから取得
            bullet = pool.Dequeue();
            bullet.SetActive(true);
            bullet.transform.SetParent(null); // プールの親から外す
        }
        else
        {
            // プールが空の場合は新規作成
            bullet = Instantiate(bulletPrefab);
            Debug.Log($"BulletPool: プールが空のため新規作成 - {bulletPrefab.name}");
        }
        
        // アクティブな弾丸として追跡
        activeBullets[bullet] = bulletPrefab;
        
        return bullet;
    }
    
    /// <summary>
    /// 弾丸をプールに戻す
    /// </summary>
    public void ReturnBullet(GameObject bullet)
    {
        if (bullet == null) return;
        
        // アクティブ追跡から削除し、元のプレハブタイプを取得
        if (activeBullets.TryGetValue(bullet, out GameObject originalPrefab))
        {
            activeBullets.Remove(bullet);
            
            // プールに戻す
            if (bulletPools.ContainsKey(originalPrefab))
            {
                bullet.SetActive(false);
                bullet.transform.SetParent(poolParent);
                bullet.transform.position = Vector3.zero;
                bullet.transform.rotation = Quaternion.identity;
                
                // Rigidbodyをリセット
                if (bullet.TryGetComponent<Rigidbody>(out Rigidbody rb))
                {
                    rb.linearVelocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                    rb.isKinematic = false;
                }
                
                bulletPools[originalPrefab].Enqueue(bullet);
            }
            else
            {
                // プールが存在しない場合は破棄
                Destroy(bullet);
            }
        }
        else
        {
            // 追跡されていない弾丸は破棄
            Debug.LogWarning($"BulletPool: 追跡されていない弾丸を破棄 - {bullet.name}");
            Destroy(bullet);
        }
    }
    
    /// <summary>
    /// 特定のプレハブ用のプールを作成
    /// </summary>
    private void CreatePool(GameObject bulletPrefab)
    {
        var pool = new Queue<GameObject>();
        
        // プール用の親オブジェクトを作成
        GameObject prefabPoolParent = new GameObject($"Pool_{bulletPrefab.name}");
        prefabPoolParent.transform.SetParent(poolParent);
        
        // 初期プールサイズ分のオブジェクトを作成
        for (int i = 0; i < defaultPoolSize; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab, prefabPoolParent.transform);
            bullet.SetActive(false);
            pool.Enqueue(bullet);
        }
        
        bulletPools[bulletPrefab] = pool;
        Debug.Log($"BulletPool: {bulletPrefab.name}用のプールを作成 (サイズ: {defaultPoolSize})");
    }
    
    /// <summary>
    /// プールの状態をデバッグ表示
    /// </summary>
    public void LogPoolStatus()
    {
        Debug.Log("=== BulletPool Status ===");
        foreach (var kvp in bulletPools)
        {
            Debug.Log($"{kvp.Key.name}: プール内 {kvp.Value.Count}個");
        }
        Debug.Log($"アクティブな弾丸: {activeBullets.Count}個");
    }
    
    /// <summary>
    /// 全てのプールをクリア
    /// </summary>
    public void ClearAllPools()
    {
        foreach (var kvp in bulletPools)
        {
            while (kvp.Value.Count > 0)
            {
                var bullet = kvp.Value.Dequeue();
                if (bullet != null)
                {
                    Destroy(bullet);
                }
            }
        }
        
        bulletPools.Clear();
        activeBullets.Clear();
        
        Debug.Log("BulletPool: 全てのプールをクリアしました");
    }
}
