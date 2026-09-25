namespace ROTF.Server.DTOs
{
    public class UpdateStatsRequest
    {
        public int Id { get; set; }
        public float Health { get; set; }
        public float Stamina { get; set; }
    }

    public class SavePositionRequest
    {
        public int Id { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
    }
}