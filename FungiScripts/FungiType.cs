namespace FungiScripts
{
    public enum FungiType
    {
        RootCell,
        Right,
        Left,
        TopRight,
        TopLeft,
        BottomRight,
        BottomLeft
    }
    
    public enum ExpansionDirection
    {
        Right,
        Left,
        TopRight,
        TopLeft,
        BottomRight,
        BottomLeft,
        Center,
        None
    }

    public static class Helpers
    {
        public static ExpansionDirection GetOppositeDirection(ExpansionDirection dir)
        {
            return dir switch
            {
                ExpansionDirection.Right => ExpansionDirection.Left,
                ExpansionDirection.Left => ExpansionDirection.Right,
                ExpansionDirection.TopRight => ExpansionDirection.BottomLeft,
                ExpansionDirection.TopLeft => ExpansionDirection.BottomRight,
                ExpansionDirection.BottomRight => ExpansionDirection.TopLeft,
                ExpansionDirection.BottomLeft => ExpansionDirection.TopRight,
                _ => ExpansionDirection.None
            };
        }
        
        public static FungiType ExpansionDirectionToFungiType(ExpansionDirection dir)
        {
            return dir switch
            {
                ExpansionDirection.Right => FungiType.Right,
                ExpansionDirection.Left => FungiType.Left,
                ExpansionDirection.TopRight => FungiType.TopRight,
                ExpansionDirection.TopLeft => FungiType.TopLeft,
                ExpansionDirection.BottomRight => FungiType.BottomRight,
                ExpansionDirection.BottomLeft => FungiType.BottomLeft,
                _ => FungiType.RootCell
            };
        }
        
        public static ExpansionDirection FungiTypeToExpansionDirection(FungiType dir)
        {
            return dir switch
            {
                FungiType.Right => ExpansionDirection.Right,
                FungiType.Left => ExpansionDirection.Left,
                FungiType.TopRight => ExpansionDirection.TopRight,
                FungiType.TopLeft => ExpansionDirection.TopLeft,
                FungiType.BottomRight => ExpansionDirection.BottomRight,
                FungiType.BottomLeft => ExpansionDirection.BottomLeft,
                _ => ExpansionDirection.None
            };
        }
    }
}