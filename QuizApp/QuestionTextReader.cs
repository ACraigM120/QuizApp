using System;
using System.IO;
using System.Collections.Generic;

namespace QuizApp
{
	/// <summary>
	/// Class for reading the given file format. This was given as follows:
	/// 
	/// Format
	/// (QUESTION_NUBMER) QUESTION TEXT
	/// RESPONSE_NUMBER.RESPONSE TEXT
	/// RESPONSE_NUMBER.RESPONSE TEXT
	/// RESPONSE_NUMBER.RESPONSE TEXT
	/// RESPONSE_NUMBER.RESPONSE TEXT
	/// CORRECT_RESPONSE_NUMBER
	///
	/// Example
	/// (1) Question text?
	/// 1. First possible answer
	/// 2. Second possible answer
	/// 3. Third possible answer
	/// 4. Fourth possible answer
	/// 3
	/// (3) What is the hex color for white?
	/// 1. #FFFFFF
	/// 2. #000000
	/// 1
	/// (2) Question text?
	/// etc...
	/// </summary>
	public class QuestionTextReader : IQuestionReader
	{
		private StreamReader reader;
		private string fileName;

		/// <summary>
		/// Constructor. Opens the FileStream to read the specified file.
		/// </summary>
		/// <param name="inFileName">Name of the file to read.</param>
		/// <exception cref="ArgumentNullException">Thrown if fileName is null or whitespace</exception>
		/// <exception cref="FileNotFoundException">Thrown if the file specified by fileName cannot be found</exception>
		public QuestionTextReader(string inFileName)
		{
			if(string.IsNullOrWhiteSpace(inFileName))
			{
				throw new ArgumentNullException("fileName cannot be null or whitespace");
			}
			if(!File.Exists(inFileName))
			{
				throw new FileNotFoundException("Unable to find file " + inFileName);
			}
			fileName = inFileName;
			reader = new StreamReader(fileName); //Just let any exceptions throw up the stack
		}

		/// <summary>
		/// The name of the file containing the questions.
		/// </summary>
		public string FileName 
		{
			get
			{
				return fileName;
			}
			set
			{
				if (string.IsNullOrWhiteSpace(value))
				{
					throw new ArgumentNullException("fileName cannot be null or whitespace");
				}
				if (!File.Exists(value))
				{
					throw new FileNotFoundException("Unable to find file " + value);
				}
				reader.Close();
				reader = null;

				fileName = value;
				reader = new StreamReader(File.OpenRead(fileName));
			} 
		}

		/// <summary>
		/// Reads questions all at once from an input file.
		/// Any exceptions from reading the file are ignored.
		/// </summary>
		/// <returns>An array of Questions read from the file.</returns>
		public Question[] ReadAllQuestions()
		{
			List<Question> questions = new List<Question>(10);
			Question nextQuestion;

			while(HasMoreContent())
			{
				try
				{
					nextQuestion = ReadNextQuestion();
				}
				catch(ArgumentException)
				{
					continue;
				}
				if (nextQuestion != null)
				{
					questions.Add(nextQuestion);
				}
			}

			return questions.ToArray();
		}

		/// <summary>
		/// Checks if the file contains more content to parse.
		/// </summary>
		/// <returns>True if we are not done parsing the file.</returns>
		public bool HasMoreContent()
		{
			return !reader.EndOfStream;
		}

		/// <summary>
		/// Reads a single question. Caller is responsible for handling any exceptions.
		/// </summary>
		/// <returns>The next valid question from the file. Null if we've reached the end of the file.</returns>
		public Question ReadNextQuestion()
		{
			Question nextQuestion = null;
			string nextLine, questionNumber, questionText, correctResponse;
			List<string> answers;
			string[] answerNumbers, answerTexts;

			do
			{
				nextLine = reader.ReadLine();
				if(nextLine == null)
				{
					return null; //There are no more questions
				}
			} while (!nextLine.StartsWith("(") && !reader.EndOfStream);

			if (nextLine.StartsWith("("))
			{
				questionNumber = nextLine.Substring(1, nextLine.IndexOf(") ") - 1);
				questionText = nextLine.Substring(nextLine.IndexOf(") ") + 2);

				answers = new List<string>(4);

				nextLine = reader.ReadLine();
				while(nextLine.Contains(". ") && !reader.EndOfStream)
				{
					answers.Add(nextLine);
					nextLine = reader.ReadLine();
				}
				correctResponse = nextLine;
				answerNumbers = new string[answers.Count];
				answerTexts = new string[answers.Count];
				for(int i = 0; i < answerNumbers.Length; i++)
				{
					answerNumbers[i] = answers[i].Substring(0, answers[i].IndexOf(". "));
					answerTexts[i] = answers[i].Substring(answers[i].IndexOf(". ") + 2);
				}

				nextQuestion = new Question(questionNumber, questionText, answerNumbers, answerTexts, correctResponse);
			}

			return nextQuestion;
		}
	}
}
