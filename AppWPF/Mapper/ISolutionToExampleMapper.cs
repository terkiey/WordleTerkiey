using API;

namespace AppWPF;

public interface ISolutionToExampleMapper
{
    SolutionExampleVM MapSolutionToExample(Solution solution);
}
