using System;
using QuizApp;

namespace UnitTests
{
	public class MockQuestion : Question
	{
		public string number;
		public string text;
		public string[] choices;
		public string[] choiceTexts;
		public string correct;

		public MockQuestion(string questionId, string question, string[] answerIds, string[] answers, string rightAnswer)
			: base(questionId, question, answerIds, answers, rightAnswer)
		{
			number = questionId;
			text = question;
			choices = answerIds;
			choiceTexts = answers;
			correct = rightAnswer;
		}

		//Useful for generating quiz files from mocks
		public override string ToString()
		{
			string output = "(" + number + ") " + text + Environment.NewLine;
			for (int i = 0; i < Math.Max(choices.Length, choiceTexts.Length); i++)
			{
				if (i < choices.Length)
				{
					output += choices[i];
				}
				output += ". ";
				if (i < choiceTexts.Length)
				{
					output += choiceTexts[i];
				}
				output += Environment.NewLine;
			}
			output += correct + Environment.NewLine;
			return output;
		}
	}
}
