using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class TriggerEvent : UnityEvent<Collider> { }//3D��ײ��
[Serializable]
public class TriggerEvent2D : UnityEvent<Collider2D> { }//2D��ײ��

public class TriggerEventListener : MonoBehaviour
{
    // Fix����

    public TriggerEvent onTriggerEnter;//������ײ��
    public TriggerEvent onTriggerStay;//����ײ����
    public TriggerEvent onTriggerExit;//�˳���ײ��

    public TriggerEvent2D onTriggerEnter2D;
    public TriggerEvent2D onTriggerStay2D;
    public TriggerEvent2D onTriggerExit2D;

    private void OnTriggerEnter(Collider other)
    {
        onTriggerEnter?.Invoke(other);
    }

    private void OnTriggerStay(Collider other)
    {
        onTriggerStay?.Invoke(other);
    }

    private void OnTriggerExit(Collider other)
    {
        onTriggerExit?.Invoke(other);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        onTriggerEnter2D?.Invoke(collision);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        onTriggerStay2D?.Invoke(collision);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        onTriggerExit2D?.Invoke(collision);
    }

}
