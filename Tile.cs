namespace FarmGame.Entities
{
    public enum TileType
    {
        Grass,
        Dirt,
        Plowed,
        Water,
        Tree
    }

    public enum CropStage
    {
        None,
        Seedling,
        Growing,
        Ready
    }

    public struct Tile
    {
        public TileType  Type;
        public CropStage Crop;
        public float     GrowTimer;     // seconds since seeded
        public float     GrowDuration;  // seconds to reach Ready

        public bool IsWalkable => Type != TileType.Water && Type != TileType.Tree;
        public bool CanPlow    => Type == TileType.Grass || Type == TileType.Dirt;
        public bool CanSeed    => Type == TileType.Plowed && Crop == CropStage.None;
        public bool CanHarvest => Crop == CropStage.Ready;

        public static Tile Default => new Tile
        {
            Type         = TileType.Grass,
            Crop         = CropStage.None,
            GrowTimer    = 0f,
            GrowDuration = 20f   // default 20 s for testing (adjust to days in full game)
        };
    }
}
