using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]//�����ڶ���������¼�
public class AnimationEvent : UnityEvent<string> { }

public class AnimationEventListener : MonoBehaviour
{

    public AnimationEvent animationEvent;

    public void OnAnimationEvent(string name)
    {
        animationEvent?.Invoke(name);//�൱��if (animationEvent != null)
                                     //{
                                     //   animationEvent.Invoke(name);
                                     //}
    }

}
