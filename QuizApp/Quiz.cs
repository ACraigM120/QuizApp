using System;
using System.IO;

namespace QuizApp
{
	/// <summary>
	/// This class is responsible for administering the quiz.
	/// </summary>
	public class Quiz
	{
		private IQuestionReader questionReader;
		private TextWriter writer;
		private TextReader reader;

		/// <summary>
		/// Constructor. Creates a Quiz using the provided IQuestionReader.
		/// </summary>
		/// <param name="inQuestionReader">An IQuestionReader to read questions from a file.</param>
		/// <param name="inWriter">A TextWriter for outputting questions to the user.</param>
		/// <param name="inReader">A TextReader for reading user answers.</param>
		public Quiz(IQuestionReader inQuestionReader, TextWriter inWriter, TextReader inReader)
		{
			if(inQuestionReader == null)
			{
				throw new ArgumentNullException("QuestionReader is not allowed to be null");
			}
			if(inWriter == null)
			{
				throw new ArgumentNullException("Writer is not allowed to be null");
			}
			if(inReader == null)
			{
				throw new ArgumentNullException("Reader is not allowed to be null");
			}

			QuestionReader = inQuestionReader;
			Writer = inWriter;
			Reader = inReader;
		}

		/// <summary>
		/// The IQuestionReader. Can be changed using set.
		/// </summary>
		/// <exception cref="ArgumentNullException">Thrown if set to null</exception>
		public IQuestionReader QuestionReader 
		{ 
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("QuestionReader is not allowed to be null");
				}

				questionReader = value;
			}
		}

		/// <summary>
		/// The TextWriter used to print output. Can be changed using set.
		/// </summary>
		/// <exception cref="ArgumentNullException">Thrown if set to null</exception>
		public TextWriter Writer
		{
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("Writer is not allowed to be null");
				}

				writer = value;
			}
		}

		/// <summary>
		/// The TextReader used to obtain user input. Can be changed using set.
		/// </summary>
		/// <exception cref="ArgumentNullException">Thrown if set to null</exception>
		public TextReader Reader
		{
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("Reader is not allowed to be null");
				}

				reader = value;
			}
		}

		public int TotalQuestions { get; private set; }
		public int CorrectAnswers { get; private set; }

		/// <summary>
		/// Quizzes the user with the provided questions.
		/// </summary>
		/// <returns>Final score as a float</returns>
		public float RunQuiz()
		{
			float score = 0f;
			Question question;
			bool isCorrect = false;

			printInstructions();

			writer.WriteLine("Beginning quiz...");

			// It should be more versatile to read in the questions one at a time,
			// especially since there isn't a reason to store them.
			while(questionReader.HasMoreContent())
			{
				// Read the next question. There are no specific requirements for invalid input.
				// If we read garbage, keep going. Hopefully it's just a typo and there's more valid content.
				try
				{
					question = questionReader.ReadNextQuestion();
				}
				catch(ArgumentException ae)
				{
					writer.WriteLine("Warning: Error creating question: ");
					writer.WriteLine(ae.ToString());
					writer.WriteLine("Ignoring and proceeding to next.");
					writer.WriteLine();
					continue;
				}
				if (question == null)
				{
					writer.WriteLine("Warning: Invalid question.");
					writer.WriteLine("Ignoring and proceeding to next.");
					writer.WriteLine();
					continue;
				}

				// Write the question
				writer.WriteLine(string.Format("({0}) {1}", question.QuestionNumber, question.QuestionText));
				for (int i = 0; i < question.ResponseNumbers.Length; i++)
				{
					writer.WriteLine(string.Format("{0}. {1}", question.ResponseNumbers[i], question.ResponseTexts[i]));
				}
				writer.WriteLine();

				// Check response and increment counters
				if (question.IsResponseCorrect(reader.ReadLine()))
				{
					isCorrect = true;
					CorrectAnswers++;
				}
				else 
				{
					isCorrect = false;
				}
				TotalQuestions++;

				// Give feedback
				writer.WriteLine(string.Format("{0}! The correct response is {1}", isCorrect?"Correct":"Incorrect", question.CorrectResponseNumber));
				writer.WriteLine();

			}

			writer.WriteLine("... Quiz completed!");
			score = (0f + CorrectAnswers) / TotalQuestions;
			writer.WriteLine(string.Format("You scored {0} out of {1}. That is {2}", CorrectAnswers, TotalQuestions, score.ToString("P0")));

			return score;
		}

		// Provide the user with some minimal instructions
		private void printInstructions()
		{
			writer.WriteLine("For each question, type in the ID number of your response and press Enter.");
			writer.WriteLine();
			writer.WriteLine("EXAMPLE:");
			writer.WriteLine("(0) What is the right answer?");
			writer.WriteLine("1. Wrong answer");
			writer.WriteLine("2. Right answer");
			writer.WriteLine("3. Wrong answer");
			writer.WriteLine();
			writer.WriteLine("You would type the following ");
			writer.WriteLine("2");
			writer.WriteLine(" and then you would press Enter.");
			writer.WriteLine();
		}
	}
}
