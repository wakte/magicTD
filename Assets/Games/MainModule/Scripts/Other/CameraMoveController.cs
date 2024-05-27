using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//��ǰ�������û������������޷����ظýű�
[RequireComponent(typeof(CinemachineVirtualCamera))]
public class CameraMoveController : MonoBehaviour
{
    // Fix����

    #region �ֶ�
    [SerializeField]
    private float size_min = 3.2f;
    [SerializeField]
    private float size_max = 6;

    private CinemachineVirtualCamera _camera;
    //�����3d������λ��
    private Vector3 mousePosition;

#if UNITY_ANDROID || UNITY_IPHONE
    private float touchDistance;
#endif

    #endregion

    #region ��������
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

#if UNITY_STANDALONE//�ж����л�����PC�ˣ�
            transform.position -= detal * Time.deltaTime;
#elif UNITY_ANDROID || UNITY_IPHONE//�ƶ��˻����ж�
            transform.position -= detal * Time.deltaTime * 0.2f;
#endif
            mousePosition = Input.mousePosition;
        }


#if UNITY_STANDALONE
        _camera.m_Lens.OrthographicSize -= Input.mouseScrollDelta.y * Time.deltaTime * 30;

#elif UNITY_ANDROID || UNITY_IPHONE
        if (Input.touchCount == 2) 
        {
            // �ƶ��������߼���˫ָ���ţ�
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
        //�������ŷ�Χ
        _camera.m_Lens.OrthographicSize = Mathf.Clamp(_camera.m_Lens.OrthographicSize, size_min, size_max);
    }

    #endregion




}
