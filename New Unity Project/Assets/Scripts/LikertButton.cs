using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class UnityIntEvent : UnityEvent<int> { }


public class LikertButton : VRButton
{
    public int likertValue = 0;
    public UnityIntEvent OnSelected = new UnityIntEvent();

    public override void Action(Controller side)
    {
        OnSelected.Invoke(likertValue);
    }
}
