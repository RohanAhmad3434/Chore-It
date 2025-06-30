//using System.ComponentModel.DataAnnotations;
//using System.ComponentModel.DataAnnotations.Schema;

//public class Reward
//{
//    [Key]
//    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // ✅ Auto-increment
//    public int Id { get; set; }

//    [Required]
//    public string Title { get; set; }

//    public string Description { get; set; }
//}


using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Reward
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string Title { get; set; }
    public string Description { get; set; }

    // ✅ NEW: Assigned to child user
    public int? AssignedToId { get; set; }    // FK (nullable if unassigned)
    public User? AssignedTo { get; set; }
}
