using iRLeagueManager.Web.ViewModels;

namespace iRLeagueManager.Web.Data;

public class CountedVote
{
    public int Count { get; set; }
    public VoteViewModel Vote { get; set; }

    public CountedVote(VoteViewModel vote)
    {
        Vote = vote;
    }
}
