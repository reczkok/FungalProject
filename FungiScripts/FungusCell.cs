using System;
using System.Collections;
using System.Collections.Generic;
using FungiScripts;
using UnityEngine;
using UnityEngine.Tilemaps;
using static FungiScripts.FungiType;
public abstract class FungusCell : AnyCell
{
    private int x, y;
    private HashSet<FungiType> connections;
    private bool active;
    protected float resourceAmount;
    private bool checkedForResources;
    protected bool isLandlocked;

    protected FungusCell(int x, int y, FungiType? initialType = null)
    {
        this.x = x;
        this.y = y;
        resourceAmount = 1000f;
        active = true;
        isLandlocked = false;
        checkedForResources = false;
        connections = new HashSet<FungiType>();
        if(initialType != null){connections.Add((FungiType)initialType);}
    }
    
    protected FungusCell(int x, int y, float initResource, FungiType? initialType = null)
    {
        this.x = x;
        this.y = y;
        resourceAmount = initResource;
        active = true;
        isLandlocked = false;
        checkedForResources = false;
        connections = new HashSet<FungiType>();
        if(initialType != null){connections.Add((FungiType)initialType);}
    }

    public Vector3Int GetCoordsAsVector()
    {
        return new Vector3Int(x, y);
    }
    
    public bool IsLandlocked()
    {
        return isLandlocked;
    }

    protected bool SeesNoResource()
    {
        return checkedForResources;
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
    
    public void AddConnection(FungiType type)
    {
        connections.Add(type);
    }
    
    public HashSet<FungiType> GetConnections()
    {
        return connections;
    }

    public ExpansionDirection Scout(Tilemap enviorement)
    {
        if (checkedForResources) return ExpansionDirection.None;
        var resourceDir = SeekResourceShortsight(enviorement);
        if (resourceDir != ExpansionDirection.None) return resourceDir;
        checkedForResources = true;
        return resourceDir;

    }

    public bool HarvestResource(ResourceCell source)
    {
        var harvestAmount = 1f;
        var resource = source.Harvest(harvestAmount);
        resourceAmount += resource;
        return resource < harvestAmount;
    }
    
    public float GetResourceAmount()
    {
        return resourceAmount;
    }

    public bool Consume()
    {
        if (!active) return true;
        if (resourceAmount < 0.1f)
        {
            resourceAmount = 0;
            return false;
        }
        resourceAmount -= 0.1f;
        return true;   
    }

    public abstract float Share();
    public void AddResource(float amount)
    {
        resourceAmount += amount;
    }
    
    protected ExpansionDirection SeesResource(Tilemap enviorement)
    {
        var left = FungusNetwork.DetermineChildPosition(this, ExpansionDirection.Left);
        left.z = 1;
        var right = FungusNetwork.DetermineChildPosition(this, ExpansionDirection.Right);
        right.z = 1;
        var topRight = FungusNetwork.DetermineChildPosition(this, ExpansionDirection.TopRight);
        topRight.z = 1;
        var topLeft = FungusNetwork.DetermineChildPosition(this, ExpansionDirection.TopLeft);
        topLeft.z = 1;
        var bottomRight = FungusNetwork.DetermineChildPosition(this, ExpansionDirection.BottomRight);
        bottomRight.z = 1;
        var bottomLeft = FungusNetwork.DetermineChildPosition(this, ExpansionDirection.BottomLeft);
        bottomLeft.z = 1;
        
        var hasLeft = enviorement.HasTile(left);
        var hasRight = enviorement.HasTile(right);
        var hasTopRight = enviorement.HasTile(topRight);
        var hasTopLeft = enviorement.HasTile(topLeft);
        var hasBottomRight = enviorement.HasTile(bottomRight);
        var hasBottomLeft = enviorement.HasTile(bottomLeft);
        
        if (!hasLeft && !hasRight && !hasTopRight && !hasTopLeft && !hasBottomRight && !hasBottomLeft)
        {
            return ExpansionDirection.None;
        }
        

        if (hasLeft) return ExpansionDirection.Left;
        if (hasRight) return ExpansionDirection.Right;
        if (hasTopRight) return ExpansionDirection.TopRight;
        if (hasTopLeft) return ExpansionDirection.TopLeft;
        if (hasBottomRight) return ExpansionDirection.BottomRight;
        if (hasBottomLeft) return ExpansionDirection.BottomLeft;
        return ExpansionDirection.None;
    }

    protected ExpansionDirection SeekResourceShortsight(Tilemap enviorement)
    {
        var center = FungusNetwork.DetermineChildPosition(this, ExpansionDirection.Center);
        center.z = 1;
        var hasCenter = enviorement.HasTile(center);
        return hasCenter ? ExpansionDirection.Center : ExpansionDirection.None;
    }

    public abstract Tuple<ExpansionDirection, FungusCell> DecideDevelopement(Tilemap enviorement);
    
}
