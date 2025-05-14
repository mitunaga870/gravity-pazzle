using Behaviour.Player.Abstract;
using Lib.Logic.Gravity;
using Lib.State.Interface.Gravity;
using UnityEngine;

namespace Behaviour.Camera
{
    /**
     * プレイヤーに追従するカメラ
     */
    public class PlayerCam : MonoBehaviour
    {
        private const float Threshold = 0.01f;
        private const float Sensitivity = 5f;
        private const float MaxPitch = 85f;
        
        private Transform? _playerTrans = null;
        private GravType? _gravType = null;
        
        private Vector3 _prevPos = Vector3.zero;
        
        private float _pitch = 0f;
        
        private void Awake()
        {
            // マウスがはみ出さないようにする
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            // プレイヤーの位置が設定されていない場合は何もしない
            if (_playerTrans == null || _gravType == null)
                return;
            
            // プレイヤーの位置と重力の種類を取得
            var playerPos = _playerTrans.position;
            var gravType = _gravType.Value;
            var playerTrans = _playerTrans;
            
            // マウスの動きに合わせてカメラを回転させる
            var mouseX = Mathf.Clamp(
                Input.GetAxis("Mouse X") * Sensitivity * -1,
                MaxPitch*-1,
                MaxPitch
                );
            var mouseY = Mathf.Clamp(
                Input.GetAxis("Mouse Y") * Sensitivity * -1,
                MaxPitch*-1,
                MaxPitch
                );
            
            // 閾値以上の動きがあった場合にカメラを回転させる
            if (Mathf.Abs(mouseY) > Threshold)
            {
                // 軸をカメラの回転角分だけ回転させる
                var rotatedAxis = GravUtils.GetGravRightUnit(gravType,transform);
                
                // ピッチを積算
                var unClampedPitch = _pitch + mouseY;
                // ピッチを制限
                _pitch = Mathf.Clamp(unClampedPitch, MaxPitch * -1, MaxPitch);
                // 超えた場合、その分を引く
                var excessPitch = unClampedPitch - _pitch;
                // 変化量計算
                var deltaPitch = mouseY - excessPitch;

                Debug.Log( $"deltaPitch: {deltaPitch}, " +
                          $"unClampedPitch: {unClampedPitch}, " +
                          $"excessPitch: {excessPitch}, " +
                          $"pitch: {_pitch}");


                // カメラの回転を更新
                transform.RotateAround(
                    playerPos,
                    rotatedAxis,
                    deltaPitch
                );
            }

            if (Mathf.Abs(mouseX) > Threshold)
            {
                
                // カメラの回転を更新
                transform.RotateAround(
                    playerPos,
                    GravUtils.GetGravDirectionUnit(gravType),
                    mouseX
                );
            }
        }
        
        /**
         * プレイヤーの位置を設定する
         */
        public void SetPlayerPosAndGrav(Transform playerTrans, GravType gravType)
        {
            if (_prevPos == Vector3.zero)
                // 初期位置設定
                _prevPos = playerTrans.position;
            
            // 変位量を計算
            var deltaPos = playerTrans.position - _prevPos;
            // カメラの位置を更新
            transform.position += deltaPos;
                
            _playerTrans = playerTrans;
            _gravType = gravType;
            _prevPos = _playerTrans.position;
        }

        /**
         * カメラの先のオブジェクトを取得する
         */
        public GameObject GetCameraTarget()
        {
            var camTransform = this.transform;
            var originalPos = camTransform.position;
            var direction = camTransform.forward;

            // カメラの先にあるオブジェクトを取得
            return Physics.Raycast(originalPos, direction, out var hit) ?
                // ヒットしたオブジェクトを返す
                hit.collider.gameObject :
                // ヒットしなかった場合はnullを返す
                null;
        }
    }
}
