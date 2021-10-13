using System;
using System.Linq;

namespace QuizApp
{
	/// <summary>
	/// Class representing a single Question in the Quiz.
	/// </summary>
	public class Question
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="questionNumber">The ID number of the question.</param>
		/// <param name="questionText">The question text.</param>
		/// <param name="responseNumbers">All available response ID numbers.</param>
		/// <param name="responseTexts">Text of each available response.</param>
		/// <param name="correctResponse">The ID number of the correct response.</param>
		/// <exception cref="ArgumentNullException">Thrown for null or whitespace questionNumber, questionText, responseNumbers, or correctResponse.
		/// Also thrown for null responseTexts (whitespace responseTexts are considered valid).</exception>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if there number of responseTexts is different than the number of responseNumbers, 
		/// or if correctResponse is not found in responseNumbers,
		/// or if responseNumbers is less than 2.</exception>
		public Question( string questionNumber, string questionText, string[] responseNumbers, string[] responseTexts, string correctResponse)
		{
			#region "InputCheck"
			
			if(responseNumbers == null)
			{
				throw new ArgumentNullException("responseNumbers cannot be null");
			}

			if(responseTexts == null)
			{
				throw new ArgumentNullException("responseTexts cannot be null");
			}

			if(responseNumbers.Length != responseTexts.Length)
			{
				throw new ArgumentOutOfRangeException("Number of responseNumbers is not the same as number of responseTexts");
			}

			if(responseNumbers.Length < 2)
			{
				// Technically not a requirement, but if there's only 1 possible response it stops being a question.
				// 0 or less would violate the expected format
				throw new ArgumentOutOfRangeException("Question does not have enough possible responses");
			}

			if(string.IsNullOrWhiteSpace(questionNumber))
			{
				//Technically this wouldn't cause problems right now since questionNumber isn't used for anything,
				//but it feels counterintuitive to allow null or whitespace here but not for responses.
				throw new ArgumentNullException("questionNumber cannot be null or whitespace");
			}

			if(string.IsNullOrWhiteSpace(questionText))
			{
				throw new ArgumentNullException("questionText cannot be null or whitespace");
			}

			if(string.IsNullOrWhiteSpace(correctResponse))
			{
				throw new ArgumentNullException("correctResponse cannot be null or whitespace");
			}

			for(int i = 0; i < responseNumbers.Length; i++)
			{
				if (string.IsNullOrWhiteSpace(responseNumbers[i]))
				{
					throw new ArgumentNullException("responseNumbers cannot include null or whitespace");
				}
				//We allow whitespace and empty string because they could foreseeably be valid answers to questions.
				//E.g. What would be the output of Console.Out.Write("");?
				if(responseTexts[i] == null)
				{
					throw new ArgumentNullException("responseTexts cannot be null");
				}
			}

			if(!responseNumbers.Contains(correctResponse))
			{
				throw new ArgumentOutOfRangeException("correctResponse not found in responseNumbers");
			}

			#endregion

			QuestionNumber = questionNumber;
			QuestionText = questionText;
			ResponseNumbers = new string[responseNumbers.Length];
			ResponseTexts = new string[responseTexts.Length];
			for(int i = 0; i < ResponseNumbers.Length; i++)
			{
				if (i > 0)
				{
					if (ResponseNumbers.Contains(responseNumbers[i]) || ResponseTexts.Contains(responseTexts[i]))
					{
						//Technically there's no requirement to check for duplicates, but 
						//the only desirable use case I can think of for this scenario is as a gag.
						throw new ArgumentException(string.Format("There is a duplicate answer: {0}. {1}", responseNumbers[i], responseTexts[i]));
					}
				}
				ResponseNumbers[i] = responseNumbers[i];
				ResponseTexts[i] = responseTexts[i];
			}
			CorrectResponseNumber = correctResponse;
		}

		/// <summary>
		/// The text of the question.
		/// </summary>
		public string QuestionText { get; private set; }

		/// <summary>
		/// The number of the question. While the format specifies that
		/// this is a number, there are no requirements for invalid input beyond
		/// "incorporate error handling". Use of letters is not incompatible with
		/// the correct format. And technically a float would be correct and I can
		/// easily guarentee the same representation of question 3.50 this way.
		/// </summary>
		public string QuestionNumber { get; private set; }

		/// <summary>
		/// All possible response texts to the question.
		/// </summary>
		public string[] ResponseTexts { get; private set; }

		/// <summary>
		/// All possible response numbers. While the format specifies that
		/// this is a number, there are no requirements for invalid input beyond
		/// "incorporate error handling". Use of letters is not incompatible with
		/// the correct format and is prevalent in the real world to the point that
		/// it makes sense to support text.
		/// </summary>
		public string[] ResponseNumbers { get; private set; }

		/// <summary>
		/// The correct response number. While the format specifies that
		/// this is a number, there are no requirements for invalid input beyond
		/// "incorporate error handling". Use of letters is not incompatible with
		/// the correct format and is prevalent in the real world to the point that
		/// it makes sense to support text.
		/// </summary>
		public string CorrectResponseNumber { get; private set; }

		/// <summary>
		/// Checks if a given answer matches CorrectResponseNumber. Invalid responses
		/// will be considered wrong.
		/// </summary>
		/// <param name="givenAnswer">The selected response number to the question</param>
		/// <returns>True if the correct answer was given, false otherwise</returns>
		public bool IsResponseCorrect(string givenAnswer)
		{
			if (givenAnswer == null) return false;
			return givenAnswer.Equals(CorrectResponseNumber);
		}
	}
}
