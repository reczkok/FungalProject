using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceCell : AnyCell
{
    private int x, y;
    private bool active;
    private float resourceAmount;
    
    public ResourceCell(int x, int y, float resourceAmount)
    {
        this.x = x;
        this.y = y;
        this.resourceAmount = resourceAmount;
        active = true;
    }
    
    public Vector3Int GetCoordsAsVector()
    {
        return new Vector3Int(x, y, 1);
    }

    public float GetResourceAmount()
    {
        return resourceAmount;
    }
    
    public float Harvest(float amount)
    {
        if (resourceAmount < amount)
        {
            var consumed = resourceAmount;
            resourceAmount = 0;
            return consumed;
        }
        resourceAmount -= amount;
        return amount;
    }
    
    public void Deactivate()
    {
        active = false;
    }
    
    public void Activate()
    {
        active = true;
    }
    
    public bool IsActive()
    {
        return active;
    }
}
