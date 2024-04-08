namespace iRLeagueApiCore.Common.Models;

[DataContract]
public sealed class Interval : IComparable, IComparable<Interval>
{
    public Interval()
    {
        Time = TimeSpan.Zero;
        Laps = 0;
    }

    public Interval(TimeSpan interval)
    {
        Time = interval.Subtract(TimeSpan.FromDays(interval.Days));
        Laps = interval.Days;
    }

    [DataMember]
    public TimeSpan Time { get; set; }
    [DataMember]
    public int Laps { get; set; }

    int IComparable<Interval>.CompareTo(Interval? other)
    {
        return ((IComparable)this).CompareTo(other);
    }

    int IComparable.CompareTo(object? obj)
    {
        if (obj is Interval other)
        {
            return Time.Add(TimeSpan.FromDays(Laps)).CompareTo(other.Time.Add(TimeSpan.FromDays(other.Laps)));
        }
        return 1;
    }
}
