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
        private const float xOffset = 5f;
        private const float yOffset = 2f;
        
        private Transform? _playerTrans = null;
        private GravType? _gravType = null;
        
        private Vector3 _prevPos = Vector3.zero;
        
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
            var playerPosOffset = playerPos + new Vector3(xOffset, yOffset, 0);
            var gravType = _gravType.Value;
            var playerTrans = _playerTrans;
            
            // マウスの動きに合わせてカメラを回転させる
            var mouseX = Input.GetAxis("Mouse X");
            var mouseY = Input.GetAxis("Mouse Y");
            
            // 閾値以上の動きがあった場合にカメラを回転させる
            if (Mathf.Abs(mouseX) > Threshold)
            {
                // 軸をカメラの回転角分だけ回転させる
                var rotatedAxis = GravUtils.GetGravRightUnit(gravType,transform);
                // カメラの回転を更新
                transform.RotateAround(
                    playerPos,
                    rotatedAxis,
                    mouseY * Sensitivity * -1
                );
                
                //　回転軸を表示
                Debug.DrawRay(playerPos, rotatedAxis, Color.red);
            }

            if (Mathf.Abs(mouseY) > Threshold)
                // カメラの回転を更新
                transform.RotateAround(
                    playerPos,
                    GravUtils.GetGravDirectionUnit(gravType),
                    mouseX * Sensitivity * -1
                );

            Debug.Log(transform.rotation);
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
