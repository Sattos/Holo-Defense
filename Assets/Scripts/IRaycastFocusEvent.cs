using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRaycastFocusEvent {

    bool BlockPlacement
    {
        get;
        set;
    }

    ObjectPlacer.ObjectsToPlace BlockingType
    {
        get;
        set;
    }

    void Deactivate();

    void Activate();

}
