# 銃システム実装ガイド

## 実装完了

✅ 弾丸からRaycastを出すシステム
✅ オブジェクトプール
✅ 武器データ管理（ScriptableObject）
✅ プレイヤーコントローラー統合
✅ 基本的な射撃・リロードシステム

## セットアップ手順

### 1. BulletPoolの配置
1. シーンに空のGameObjectを作成し、`BulletPool`と命名
2. `BulletPool.cs`スクリプトをアタッチ

### 2. 弾丸プレハブの作成
1. 3Dオブジェクト（Sphere）を作成
2. スケールを小さく調整（例：0.05, 0.05, 0.05）
3. `Rigidbody`コンポーネントを追加
4. `BulletBase.cs`スクリプトをアタッチ
5. Prefab化してAssets/Weapon_Resources/Prefabsに保存

### 3. 武器データの作成
1. Projectウィンドウで右クリック
2. Create > Weapons > WeaponData を選択
3. パラメータを設定：
   - Weapon Name: 武器名
   - Bullet Prefab: 上で作成した弾丸プレハブ
   - Muzzle Velocity: 初速（例：400）
   - Damage: ダメージ（例：25）
   - Fire Rate: 発射レート（例：600 RPM）
   - Magazine Size: マガジンサイズ（例：30）
   - Reload Time: リロード時間（例：2.5秒）

### 4. 武器プレハブの設定
1. 銃のモデルプレハブを選択
2. 子オブジェクトとして空のGameObject「FirePoint」を作成
3. FirePointを銃口の位置に配置
4. 武器プレハブに`RifleWeapon.cs`をアタッチ
5. Fire Point と Weapon Data を設定

### 5. プレイヤーへの統合
1. Playerオブジェクトに`WeaponHolder.cs`をアタッチ
2. 子オブジェクト「WeaponAttachPoint」を作成（自動作成される）
3. Current Weaponに武器プレハブを設定

## 操作方法

- **射撃**: マウス左クリック
- **リロード**: Rキー

## デバッグ機能

### コンテキストメニュー（右クリック）
- WeaponBase: "Debug: Fire Weapon", "Debug: Reload Weapon", "Debug: Show Status"
- WeaponHolder: "Debug: Show Weapon Status", "Debug: Test Fire", "Debug: Test Reload"

### コンソールログ
すべての射撃、リロード、衝突がデバッグログに出力されます：
```
[Weapon] 発射: M4_Rifle 残弾: 29
弾丸衝突: Cube 距離: 15.23m ダメージ: 25
[Weapon] リロード開始: M4_Rifle 2.5秒
[Weapon] リロード完了: M4_Rifle 残弾: 30
```

## 新しい武器の追加方法

1. WeaponDataSOを新規作成
2. 武器モデルにFirePointを追加
3. WeaponBase継承クラス（RifleWeapon等）をアタッチ
4. 必要に応じて特殊動作をオーバーライド

## 将来の拡張ポイント

- ✨ 武器切り替えシステム（WeaponHolderに準備済み）
- ✨ ダメージシステム（IDamageableインターフェース）
- ✨ エフェクト・アニメーション
- ✨ UI統合
- ✨ 弾種別の特殊効果

## トラブルシューティング

### よくある問題

1. **弾丸が発射されない**
   - BulletPoolがシーンに配置されているか確認
   - WeaponDataのBulletPrefabが設定されているか確認
   - FirePointの位置と向きを確認

2. **衝突判定が働かない**
   - 弾丸にRigidbodyがアタッチされているか確認
   - ターゲットにColliderがあるか確認
   - BulletBaseのLayerMask設定を確認

3. **入力が反応しない**
   - PlayerControllerにStandardInputProviderがアタッチされているか確認
   - WeaponHolderが正しく統合されているか確認

## パフォーマンス

- オブジェクトプールにより弾丸のInstantiate/Destroyを削減
- Raycastによる精密な衝突判定
- 設定可能なプールサイズで調整可能
