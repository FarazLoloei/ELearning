using ELearning.SharedKernel;

namespace ELearning.Domain.Entities.CourseAggregate.Enums;

public class CourseLevel : Enumeration
{
    public static CourseLevel Beginner = new CourseLevel(1, nameof(Beginner));
    public static CourseLevel Intermediate = new CourseLevel(2, nameof(Intermediate));
    public static CourseLevel Advanced = new CourseLevel(3, nameof(Advanced));
    public static CourseLevel AllLevels = new CourseLevel(4, nameof(AllLevels));

    private CourseLevel(int id, string name) : base(id, name)
    {
    }
}