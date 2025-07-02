//namespace Parent_Child.DTOs
//{
//    public class ChildStatisticsDto
//    {
//        public int TaskCompletionRate { get; set; }
//        public int RewardsCompletionRate { get; set; }
//        public int TotalTasks { get; set; }
//        public int CompletedTasks { get; set; }
//        public int RewardsReceived { get; set; }
//        public int RewardsRedeemed { get; set; }
//        public string TaskByRelation { get; set; }
//        public int TaskByRelationCount { get; set; }
//        public int TaskByRelationCompletionRate { get; set; }

//    }
//}


namespace Parent_Child.DTOs
{
    public class RelationTaskStatsDto
    {
        public string Relation { get; set; }
        public int TasksCount { get; set; }
        public int TasksCompleted { get; set; }
        public int CompletionRate { get; set; }
    }

    public class ChildStatisticsDto
    {
        public int TaskCompletionRate { get; set; }
        public int RewardsCompletionRate { get; set; }
        public int TotalTasks { get; set; }
        public int CompletedTasks { get; set; }
        public int RewardsReceived { get; set; }
        public int RewardsRedeemed { get; set; }

        public List<RelationTaskStatsDto> TasksByRelations { get; set; }
    }
}
