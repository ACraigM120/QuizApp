using System;
using QuizApp;

namespace UnitTests
{
	public class MockReader : IQuestionReader
	{
		private int inc = 0;
		private MockQuestion[] myQuestions;

		public string FileName { get; set; }

		//Used to make the reader spit out errors and nulls
		public bool GarbageMode { get; set; } = false;

		public MockQuestion[] MyQuestions
		{
			get
			{
				return myQuestions;
			}
			set
			{
				inc = 0;
				myQuestions = value;
			}
		}

		/// <summary>
		/// Reads questions all at once from an input file.
		/// </summary>
		/// <returns>An array of Questions read from the file.</returns>
		public Question[] ReadAllQuestions()
		{
			return myQuestions;
		}

		/// <summary>
		/// Checks if the file contains more content to parse.
		/// </summary>
		/// <returns>True if we are not done parsing the file.</returns>
		public bool HasMoreContent()
		{
			return inc < myQuestions.Length;
		}

		/// <summary>
		/// Reads a single question.
		/// </summary>
		/// <returns>The next valid question from the file. Null if we've reached the end of the file.</returns>
		public Question ReadNextQuestion()
		{
			Question output = null;
			if (HasMoreContent())
			{
				if (!GarbageMode)
				{
					output = myQuestions[inc];
					inc++;
				}
				else
				{
					//Simulate the exceptions and other bad output we expect to get
					switch (inc % 4)
					{
						case 0:
							inc++;
							throw new ArgumentException("Test");
						case 1:
							inc++;
							throw new ArgumentNullException("Test");
						case 2:
							inc++;
							throw new ArgumentOutOfRangeException("Test");
						default:
							output = null;
							inc++;
							break;
					}
				}
			}
			return output;
		}
	}
}
