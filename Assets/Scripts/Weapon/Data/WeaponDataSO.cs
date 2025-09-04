using UnityEngine;

/// <summary>
/// 武器データのScriptableObject実装
/// デザイナーが簡単に武器を作成・調整できるようにする
/// </summary>
[CreateAssetMenu(fileName = "WeaponData", menuName = "Weapons/WeaponData")]
public class WeaponDataSO : ScriptableObject, IWeaponData
{
    [Header("Basic Settings")]
    [SerializeField] private string weaponName = "New Weapon";
    [SerializeField] private GameObject bulletPrefab;
    
    [Header("Ballistics")]
    [SerializeField] private float muzzleVelocity = 400f;    // 初速 m/s
    [SerializeField] private float damage = 25f;
    
    [Header("Fire Rate")]
    [SerializeField] private float fireRate = 600f;          // RPM (rounds per minute)
    
    [Header("Magazine")]
    [SerializeField] private int magazineSize = 30;
    [SerializeField] private float reloadTime = 2.5f;
    
    [Header("Audio Effects")]
    [SerializeField] private AudioClip fireSound;           // 発射音
    [SerializeField] private AudioClip reloadStartSound;    // リロード開始音
    [SerializeField] private AudioClip reloadCompleteSound; // リロード完了音
    [SerializeField] private AudioClip emptySound;          // 弾切れ音
    
    // IWeaponData実装
    public string WeaponName => weaponName;
    public GameObject BulletPrefab => bulletPrefab;
    public float MuzzleVelocity => muzzleVelocity;
    public float Damage => damage;
    public float FireRate => fireRate;
    public int MagazineSize => magazineSize;
    public float ReloadTime => reloadTime;
    
    // Audio Effects実装
    public AudioClip FireSound => fireSound;
    public AudioClip ReloadStartSound => reloadStartSound;
    public AudioClip ReloadCompleteSound => reloadCompleteSound;
    public AudioClip EmptySound => emptySound;
    
    void OnValidate()
    {
        // エディタでの値検証
        muzzleVelocity = Mathf.Max(0f, muzzleVelocity);
        damage = Mathf.Max(0f, damage);
        fireRate = Mathf.Max(1f, fireRate);
        magazineSize = Mathf.Max(1, magazineSize);
        reloadTime = Mathf.Max(0.1f, reloadTime);
    }
}
