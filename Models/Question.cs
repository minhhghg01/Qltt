using Qltt.Models;

public class Question
{
    public int QuestionId { get; set; }
    public int TestId { get; set; }
    public string Content { get; set; }
    public string OptionA { get; set; }
    public string OptionB { get; set; }
    public string OptionC { get; set; }
    public string OptionD { get; set; }
    public string CorrectAnswer { get; set; }

    // Navigation property
    public virtual Test Test { get; set; }
} 