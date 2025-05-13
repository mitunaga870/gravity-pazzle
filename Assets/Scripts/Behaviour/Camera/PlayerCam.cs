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
        
        private Vector3? _playerPos = null;
        private GravType? _gravType = null;

        private void Update()
        {
            // プレイヤーの位置が設定されていない場合は何もしない
            if (_playerPos == null || _gravType == null)
                return;
            
            // プレイヤーの位置と重力の種類を取得
            var playerPos = _playerPos.Value;
            var gravType = _gravType.Value;
            
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
        }
        
        /**
         * プレイヤーの位置を設定する
         */
        public void SetPlayerPosAndGrav(Vector3 playerPos, GravType gravType)
        {
            _playerPos = playerPos;
            _gravType = gravType;
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
