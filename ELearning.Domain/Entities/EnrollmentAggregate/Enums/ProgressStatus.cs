using ELearning.SharedKernel;

namespace ELearning.Domain.Entities.EnrollmentAggregate.Enums;

public class ProgressStatus : Enumeration
{
    public static ProgressStatus NotStarted = new ProgressStatus(1, nameof(NotStarted));
    public static ProgressStatus InProgress = new ProgressStatus(2, nameof(InProgress));
    public static ProgressStatus Completed = new ProgressStatus(3, nameof(Completed));

    private ProgressStatus(int id, string name) : base(id, name)
    {
    }
}