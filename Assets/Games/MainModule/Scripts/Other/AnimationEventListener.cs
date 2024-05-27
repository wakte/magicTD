using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]//用于在动画中添加事件
public class AnimationEvent : UnityEvent<string> { }

public class AnimationEventListener : MonoBehaviour
{

    public AnimationEvent animationEvent;

    public void OnAnimationEvent(string name)
    {
        animationEvent?.Invoke(name);//相当于if (animationEvent != null)
                                     //{
                                     //   animationEvent.Invoke(name);
                                     //}
    }

}
