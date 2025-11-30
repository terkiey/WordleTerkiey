using CoreObjects;

namespace API;

internal interface IAnswerWordService
{
    WordleWord Answer { get; set; }

    event EventHandler<WordleWord>? AnswerWordChanged;
}
