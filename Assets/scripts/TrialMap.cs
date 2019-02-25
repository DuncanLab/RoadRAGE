using CsvHelper.Configuration;

namespace Assets.scripts
{
    class TrialMap : ClassMap<GameData.Trial>
    {
        public TrialMap()
        {
            Map(m => m.TrialId).Index(0).Name("TrialId");
            Map(m => m.TrialName).Index(1).Name("TrialName");
            Map(m => m.Timer.ElapsedMilliseconds).Index(2).Name("Timer (in ms)");
            Map(m => m.TrackResources).Index(3).Name("TrackResources");
            Map(m => m.TrackPoints).Index(4).Name("TrackPoints");
            Map(m => m.TrackPickups).Index(5).Name("TrackPickups");
            Map(m => m.KeepPointsAfterResourceDepletion).Index(6).Name("KeepPointsAfterResourceDepletion");
            Map(m => m.LockByTime).Index(7).Name("LockByTime (in ms)");
            Map(m => m.TimeAllotted).Index(8).Name("TimeAllotted (in ms)");
            Map(m => m.PointsCollected).Index(9).Name("PointsCollected");
        }
    }
}
