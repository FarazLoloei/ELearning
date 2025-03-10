using ELearning.SharedKernel;

namespace ELearning.Domain.Entities.CourseAggregate.Enums;

public class CourseCategory : Enumeration
{
    public static CourseCategory Programming = new CourseCategory(1, nameof(Programming));
    public static CourseCategory Design = new CourseCategory(2, nameof(Design));
    public static CourseCategory Business = new CourseCategory(3, nameof(Business));
    public static CourseCategory Marketing = new CourseCategory(4, nameof(Marketing));
    public static CourseCategory Science = new CourseCategory(5, nameof(Science));
    public static CourseCategory Mathematics = new CourseCategory(6, nameof(Mathematics));
    public static CourseCategory Languages = new CourseCategory(7, nameof(Languages));
    public static CourseCategory Arts = new CourseCategory(8, nameof(Arts));
    public static CourseCategory Health = new CourseCategory(9, nameof(Health));
    public static CourseCategory Other = new CourseCategory(10, nameof(Other));

    private CourseCategory(int id, string name) : base(id, name)
    {
    }
}