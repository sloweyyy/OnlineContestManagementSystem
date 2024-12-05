public class QuarterlyContestStatusCountResponse
{
  public int Year { get; set; }
  public string Quarter { get; set; }
  public string Status { get; set; }
  public int ContestCount { get; set; }
}
public class QuarterlyContestDataResponse
{
  public string Quarter { get; set; }
  public int OnBoarding { get; set; }
  public int ComingSoon { get; set; }
  public int Ended { get; set; }
}