using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface AnyCell
{
    Vector3Int GetCoordsAsVector();
    void Deactivate();
    void Activate();
    bool IsActive();
}
