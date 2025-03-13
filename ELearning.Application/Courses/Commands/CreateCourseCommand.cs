using ELearning.Application.Common.Model;
using MediatR;

namespace ELearning.Application.Courses.Commands;

public class CreateCourseCommand : IRequest<Result>
{
    public string Title { get; set; }

    public string Description { get; set; }

    public int CategoryId { get; set; }

    public int LevelId { get; set; }

    public decimal Price { get; set; }

    public int DurationHours { get; set; }

    public int DurationMinutes { get; set; }
}