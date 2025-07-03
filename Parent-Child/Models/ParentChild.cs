using System.ComponentModel.DataAnnotations.Schema;

namespace Parent_Child.Models
{
    public class ParentChild
    {
        public int ParentId { get; set; }
        public User Parent { get; set; }

        public int ChildId { get; set; }
        public User Child { get; set; }

        public string? Relation { get; set; } // Father, Mother, etc.
    }
}

