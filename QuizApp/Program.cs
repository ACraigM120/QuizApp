using System;

namespace QuizApp
{
	/// <summary>
	/// Main Console program.
	/// </summary>
	class Program
	{
		/// <summary>
		/// Main method. Starts the quiz using the entered file name or prints a usage message and exits.
		/// </summary>
		/// <param name="args">Question file name</param>
		static void Main(string[] args)
		{
			if(args.Length < 1)
			{
				//If needed, this could easily be changed to load a default file
				Console.Out.WriteLine("Usage: QuizApp [questionFileName]");
				return;
			}
			//If needed, this could easily be changed to do multiple quizzes in sequence using a loop.
			Quiz quiz = new Quiz(new QuestionTextReader(args[0]), Console.Out, Console.In);
			quiz.RunQuiz();
		}
	}
}
