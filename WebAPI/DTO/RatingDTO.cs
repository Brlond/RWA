namespace WebAPI.DTO
{
    public class RatingDTO
    {
        public int? Id { get; set; }
        public int Score { get; set; }
        public string UserName { get; set; }
        public int PostId { get; set; }
    }
}
