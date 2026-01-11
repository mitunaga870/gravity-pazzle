using UnityEngine;
using LitMotion;
using LitMotion.Extensions; // DoRotateやDoLocalRotateを使うために必要

public class ContinuousRotation : MonoBehaviour
{
    void Start()
    {
       //なんでこの角度なのか謎。ワールド座標なんかな？A to Bへ9秒
       LMotion.Create(new Vector3(25, 0, 0), new Vector3(25, 360, 0), 9f)
           //-1は無限ループらしい、RESTRTは最初から戻すみたいな
           .WithLoops(-1, LoopType.Restart)
           // 上記の動作をこのオブジェクトの transform.position.y にバインド
           .BindToEulerAngles(this.transform)
           // 破壊時に消すように
           .AddTo(this);
    }
}