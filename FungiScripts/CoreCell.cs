using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using FungiScripts;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class CoreCell : FungusCell
{
    public CoreCell(int x, int y, FungiType? initialType = null) : base(x, y, initialType) { }
    
    public CoreCell(int x, int y, float initResource, FungiType? initialType = null) : base(x, y, initResource, initialType) { }
    
    public override float Share()
    {
        if (!(resourceAmount > 1f)) return 0;
        if (resourceAmount < 10f)
        {
            var shareLow = resourceAmount / 2;
            resourceAmount -= shareLow;
            return shareLow;
        }
        var share = resourceAmount * 0.8f;
        resourceAmount -= share;
        return share;
    }

    public override Tuple<ExpansionDirection, FungusCell> DecideDevelopement(Tilemap enviorement)
    {
        bool hasLeft = enviorement.HasTile(FungusNetwork.DetermineChildPosition(this, ExpansionDirection.Left));
        bool hasRight = enviorement.HasTile(FungusNetwork.DetermineChildPosition(this, ExpansionDirection.Right));
        bool hasTopRight = enviorement.HasTile(FungusNetwork.DetermineChildPosition(this, ExpansionDirection.TopRight));
        bool hasTopLeft = enviorement.HasTile(FungusNetwork.DetermineChildPosition(this, ExpansionDirection.TopLeft));
        bool hasBottomRight = enviorement.HasTile(FungusNetwork.DetermineChildPosition(this, ExpansionDirection.BottomRight));
        bool hasBottomLeft = enviorement.HasTile(FungusNetwork.DetermineChildPosition(this, ExpansionDirection.BottomLeft));

        if (!hasLeft && !hasRight && !hasTopRight && !hasTopLeft && !hasBottomRight && !hasBottomLeft)
        {
            var randomRoot = Random.Range(0, 6);
            Vector3Int dir;
            var childResourceFirst = resourceAmount;
            resourceAmount -= childResourceFirst;
            switch (randomRoot)
            {
                case 0:
                    dir = FungusNetwork.DetermineChildPosition(this, ExpansionDirection.Left);
                    return new Tuple<ExpansionDirection, FungusCell>(ExpansionDirection.Left, new CoreCell(dir.x, dir.y, childResourceFirst));
                case 1:
                    dir = FungusNetwork.DetermineChildPosition(this, ExpansionDirection.Right);
                    return new Tuple<ExpansionDirection, FungusCell>(ExpansionDirection.Right, new CoreCell(dir.x, dir.y, childResourceFirst));
                case 2:
                    dir = FungusNetwork.DetermineChildPosition(this, ExpansionDirection.TopRight);
                    return new Tuple<ExpansionDirection, FungusCell>(ExpansionDirection.TopRight, new CoreCell(dir.x, dir.y, childResourceFirst));
                case 3:
                    dir = FungusNetwork.DetermineChildPosition(this, ExpansionDirection.TopLeft);
                    return new Tuple<ExpansionDirection, FungusCell>(ExpansionDirection.TopLeft, new CoreCell(dir.x, dir.y, childResourceFirst));
                case 4:
                    dir = FungusNetwork.DetermineChildPosition(this, ExpansionDirection.BottomRight);
                    return new Tuple<ExpansionDirection, FungusCell>(ExpansionDirection.BottomRight, new CoreCell(dir.x, dir.y, childResourceFirst));
                case 5:
                    dir = FungusNetwork.DetermineChildPosition(this, ExpansionDirection.BottomLeft);
                    return new Tuple<ExpansionDirection, FungusCell>(ExpansionDirection.BottomLeft, new CoreCell(dir.x, dir.y, childResourceFirst));
                default:
                    throw new Exception("Random number out of range");
            }
        }
        
        int neighbourCount = 0;
        float deactivationProb = 0f;
        float leftProb = 0f;
        float rightProb = 0f;
        float topRightProb = 0f;
        float topLeftProb = 0f;
        float bottomRightProb = 0f;
        float bottomLeftProb = 0f;

        if (hasLeft)
        {
            rightProb += FungiParameters.directOppositeChance;
            topRightProb += FungiParameters.broadOppositeChance;
            bottomRightProb += FungiParameters.broadOppositeChance;
            neighbourCount++;
        }
        if (hasRight)
        {
            leftProb += FungiParameters.directOppositeChance;
            topLeftProb += FungiParameters.broadOppositeChance;
            bottomLeftProb += FungiParameters.broadOppositeChance;
            neighbourCount++;
        }
        if (hasTopRight)
        {
            bottomLeftProb += FungiParameters.directOppositeChance;
            leftProb += FungiParameters.broadOppositeChance;
            bottomRightProb += FungiParameters.broadOppositeChance;
            neighbourCount++;
        }
        if (hasTopLeft)
        {
            bottomRightProb += FungiParameters.directOppositeChance;
            rightProb += FungiParameters.broadOppositeChance;
            bottomLeftProb += FungiParameters.broadOppositeChance;
            neighbourCount++;
        }
        if (hasBottomRight)
        {
            topLeftProb += FungiParameters.directOppositeChance;
            leftProb += FungiParameters.broadOppositeChance;
            topRightProb += FungiParameters.broadOppositeChance;
            neighbourCount++;
        }
        if (hasBottomLeft)
        {
            topRightProb += FungiParameters.directOppositeChance;
            rightProb += FungiParameters.broadOppositeChance;
            topLeftProb += FungiParameters.broadOppositeChance;
            neighbourCount++;
        }
        
        deactivationProb = neighbourCount * 0.1f;
        if (deactivationProb > FungiParameters.maxNeighborBeforeStarvation) deactivationProb = 0.95f;
        if (deactivationProb <= FungiParameters.safeNeighbor) deactivationProb = 0.01f;
        if (Random.Range(0f, 1f) < deactivationProb)
        {
            if (!(neighbourCount < 2) || !SeesNoResource())
                return new Tuple<ExpansionDirection, FungusCell>(ExpansionDirection.None, null);
            if(resourceAmount < 0.5f)
                return new Tuple<ExpansionDirection, FungusCell>(ExpansionDirection.None, null);
            
            var childResourceExploratory = resourceAmount;
            resourceAmount -= childResourceExploratory;
            if (hasLeft)
            {
                var dir = FungusNetwork.DetermineChildPosition(this, ExpansionDirection.Right);
                return new Tuple<ExpansionDirection, FungusCell>(ExpansionDirection.Right, new ExplorationCell(dir.x, dir.y, childResourceExploratory));
            }
            if (hasRight)
            {
                var dir = FungusNetwork.DetermineChildPosition(this, ExpansionDirection.Left);
                return new Tuple<ExpansionDirection, FungusCell>(ExpansionDirection.Left, new ExplorationCell(dir.x, dir.y, childResourceExploratory));
            }
            if (hasTopRight)
            {
                var dir = FungusNetwork.DetermineChildPosition(this, ExpansionDirection.BottomLeft);
                return new Tuple<ExpansionDirection, FungusCell>(ExpansionDirection.BottomLeft, new ExplorationCell(dir.x, dir.y, childResourceExploratory));
            }
            if (hasTopLeft)
            {
                var dir = FungusNetwork.DetermineChildPosition(this, ExpansionDirection.BottomRight);
                return new Tuple<ExpansionDirection, FungusCell>(ExpansionDirection.BottomRight, new ExplorationCell(dir.x, dir.y, childResourceExploratory));
            }
            if (hasBottomRight)
            {
                var dir = FungusNetwork.DetermineChildPosition(this, ExpansionDirection.TopLeft);
                return new Tuple<ExpansionDirection, FungusCell>(ExpansionDirection.TopLeft, new ExplorationCell(dir.x, dir.y, childResourceExploratory));
            }
            if (hasBottomLeft)
            {
                var dir = FungusNetwork.DetermineChildPosition(this, ExpansionDirection.TopRight);
                return new Tuple<ExpansionDirection, FungusCell>(ExpansionDirection.TopRight, new ExplorationCell(dir.x, dir.y, childResourceExploratory));
            }
        }

        if (hasRight)
        {
            rightProb = 0f;
        }
        if (hasLeft)
        {
            leftProb = 0f;
        }
        if (hasTopRight)
        {
            topRightProb = 0f;
        }
        if (hasTopLeft)
        {
            topLeftProb = 0f;
        }
        if (hasBottomRight)
        {
            bottomRightProb = 0f;
        }
        if (hasBottomLeft)
        {
            bottomLeftProb = 0f;
        }

        float total = leftProb + rightProb + topRightProb + topLeftProb + bottomRightProb + bottomLeftProb;

        if (total < 0.1f) return new Tuple<ExpansionDirection, FungusCell>(ExpansionDirection.None, null);

        var childResource = resourceAmount * 0.9f;
        resourceAmount -= childResource;
        
        float random = Random.Range(0f, total);

        if (random < leftProb)
        {
            var dir = FungusNetwork.DetermineChildPosition(this, ExpansionDirection.Left);
            return new Tuple<ExpansionDirection, FungusCell>(ExpansionDirection.Left, new CoreCell(dir.x, dir.y, childResource));
        }
        if (random < leftProb + rightProb)
        {
            var dir = FungusNetwork.DetermineChildPosition(this, ExpansionDirection.Right);
            return new Tuple<ExpansionDirection, FungusCell>(ExpansionDirection.Right, new CoreCell(dir.x, dir.y, childResource));
        }
        if (random < leftProb + rightProb + topRightProb)
        {
            var dir = FungusNetwork.DetermineChildPosition(this, ExpansionDirection.TopRight);
            return new Tuple<ExpansionDirection, FungusCell>(ExpansionDirection.TopRight, new CoreCell(dir.x, dir.y, childResource));
        }
        if (random < leftProb + rightProb + topRightProb + topLeftProb)
        {
            var dir = FungusNetwork.DetermineChildPosition(this, ExpansionDirection.TopLeft);
            return new Tuple<ExpansionDirection, FungusCell>(ExpansionDirection.TopLeft, new CoreCell(dir.x, dir.y, childResource));
        }
        if (random < leftProb + rightProb + topRightProb + topLeftProb + bottomRightProb)
        {
            var dir = FungusNetwork.DetermineChildPosition(this, ExpansionDirection.BottomRight);
            return new Tuple<ExpansionDirection, FungusCell>(ExpansionDirection.BottomRight, new CoreCell(dir.x, dir.y, childResource));
        }
        if (random < leftProb + rightProb + topRightProb + topLeftProb + bottomRightProb + bottomLeftProb)
        {
            var dir = FungusNetwork.DetermineChildPosition(this, ExpansionDirection.BottomLeft);
            return new Tuple<ExpansionDirection, FungusCell>(ExpansionDirection.BottomLeft, new CoreCell(dir.x, dir.y, childResource));
        }
        return new Tuple<ExpansionDirection, FungusCell>(ExpansionDirection.None, null);
    }
}
