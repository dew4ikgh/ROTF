namespace ROTF.Server.DTOs
{
    public class PlaceBuildingDto
    {
        public int ServerId { get; set; }
        public int OwnerId { get; set; }
        public string BuildingType { get; set; } = string.Empty;
        public float PositionX { get; set; }
        public float PositionY { get; set; }
        public float PositionZ { get; set; }
    }
}