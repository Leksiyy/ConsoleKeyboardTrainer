namespace KeyboardTrainer.Models;

public class Statistics
{
    public int Id { get; set; }
    public int NumOfLetters { get; set; }

    public int CorrectLetters { get; set; }
    public int WrongLetters { get; set; }
    public TimeSpan Time { get; set; }
}