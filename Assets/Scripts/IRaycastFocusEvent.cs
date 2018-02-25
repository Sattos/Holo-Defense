using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BlockingType
{
    NotBlocking,        //place tower normally
    BlockingButton,     //dont place, dont click button
    BlockingPlacement,  //dont place, click button
    OtherTower          //dont place, click other tower
}

public interface IRaycastFocusEvent {

    BlockingType BlockingType
    {
        get;
        set;
    }

    void Deactivate();

    void Activate();

    void Click();

}
