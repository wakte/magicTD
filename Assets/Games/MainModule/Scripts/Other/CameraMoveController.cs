using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//当前场景如果没有虚拟相机则无法挂载该脚本
[RequireComponent(typeof(CinemachineVirtualCamera))]
public class CameraMoveController : MonoBehaviour
{
    // Fix编码

    #region 字段
    [SerializeField]
    private float size_min = 3.2f;
    [SerializeField]
    private float size_max = 6;

    private CinemachineVirtualCamera _camera;
    //鼠标在3d场景的位置
    private Vector3 mousePosition;

#if UNITY_ANDROID || UNITY_IPHONE
    private float touchDistance;
#endif

    #endregion

    #region 生命周期
    private void Awake()
    {
        _camera = GetComponent<CinemachineVirtualCamera>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            mousePosition = Input.mousePosition;
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 detal = Input.mousePosition - mousePosition;

#if UNITY_STANDALONE//判断运行环境（PC端）
            transform.position -= detal * Time.deltaTime;
#elif UNITY_ANDROID || UNITY_IPHONE//移动端环境判断
            transform.position -= detal * Time.deltaTime * 0.2f;
#endif
            mousePosition = Input.mousePosition;
        }


#if UNITY_STANDALONE
        _camera.m_Lens.OrthographicSize -= Input.mouseScrollDelta.y * Time.deltaTime * 30;

#elif UNITY_ANDROID || UNITY_IPHONE
        if (Input.touchCount == 2) 
        {
            // 移动端缩放逻辑（双指缩放）
            float distance = Vector2.Distance(Input.GetTouch(0).position, Input.GetTouch(1).position);
            if (touchDistance == 0)
            {
                touchDistance = distance;
            }

            float detal = distance - touchDistance;
            _camera.m_Lens.OrthographicSize -= detal * Time.deltaTime * 0.2f;
            touchDistance = distance;
        } 
#endif
        //设置缩放范围
        _camera.m_Lens.OrthographicSize = Mathf.Clamp(_camera.m_Lens.OrthographicSize, size_min, size_max);
    }

    #endregion




}
