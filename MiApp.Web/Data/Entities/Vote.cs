namespace MiApp.Web.Data.Entities
{
    public class Vote
    {
        public int Id { get; set; }

        public AppUserEntity AppUser { get; set; }

        public EventVoteEntity EventVote { get; set; }

        public EventVoteOptionEntity EventVoteOption { get; set; }

    }
}