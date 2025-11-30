namespace API;

internal class AnswerWordService : IAnswerWordService
{
    private WordleWord _answer = new("dumpy");
    public WordleWord Answer 
    { 
        get => _answer;
        set {
            _answer = value;
            AnswerWordChanged?.Invoke(this, value);
        }
    }

    public event EventHandler<WordleWord>? AnswerWordChanged;
}
