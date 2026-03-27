namespace PRSC_Player_Auction_System
{
    /// <summary>
    /// Unified Player model.
    /// IsSold and CurrentPrice are computed properties so DatabaseHelper
    /// can read/write them while the grid uses Status and SoldPrice.
    /// </summary>
    public class Player
    {
        public int     Id           { get; set; }
        public string  Name         { get; set; } = "";
        public string  Position     { get; set; } = "";
        public string  SkillLevel   { get; set; } = "Medium";
        public decimal BasePrice    { get; set; }
        public decimal SoldPrice    { get; set; }
        public string  AssignedTeam { get; set; } = "—";
        public string  Status       { get; set; } = "Available";
        public string  VideoPath    { get; set; } = "";

        // ── Compatibility props used by DatabaseHelper ──────────────
        public bool IsSold
        {
            get => Status == "Sold";
            set => Status = value ? "Sold" : "Available";
        }

        public decimal CurrentPrice
        {
            get => SoldPrice > 0 ? SoldPrice : BasePrice;
            set => SoldPrice = value;
        }
    }
}
