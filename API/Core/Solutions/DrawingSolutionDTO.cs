namespace API;

public record DrawingSolutionDTO(
DrawingValidation drawingValidation,
List<CategorySolutionResult> categorySolutions)
{
}
