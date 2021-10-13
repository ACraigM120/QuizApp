using NUnit.Framework;
using System;
using QuizApp;

namespace UnitTests
{
	public class UnitTestQuestion
	{
		string empty = string.Empty; //because I don't feel like typing it out every time
		string space = " ";

		#region "valid questions"

		string goodNumber1 = "1";
		string goodText1 = "Did this test pass?";
		string[] goodChoices1 = { "1", "2", "3", "4" };
		string[] goodChoicesTexts1 = { "Yes", "No", "Unknown", "All of the above" };
		string goodRightAnswer1 = "1";

		string goodNumber2 = "2";
		string goodText2 = "Ancay ouyay eadray igpay atinlay?";
		string[] goodChoices2 = { "1", "2", "3", "4", "5", "6" };
		string[] goodChoicesTexts2 = { "Yes", "No", "What?", "Esyay", "Onay", "Oink" };
		string goodRightAnswer2 = "4";

		string goodNumber3 = "3";
		string goodText3 = "How about a double digit answer?";
		string[] goodChoices3 = { "1", "22" };
		string[] goodChoicesTexts3 = { "No", "Okay" };
		string goodRightAnswer3 = "22";

		#endregion

		#region "invalid questions"

		string[] badChoices1 = { "1" };
		string[] badChoicesTexts1 = { "Meh" };

		string[] badChoices2 = { };
		string[] badChoicesTexts2 = { };

		string[] badChoices3 = { "1", "1" };
		string[] badChoicesTexts3 = { "Answer", "Answer" };

		string[] shortChoices = { "1", "2", "3" };
		string[] longChoicesTexts = { "Yes", "No", "Unknown", "All of the above", "None of the above" };

		string[] nullChoices = { "1", null, "3", "4" };
		string[] nullChoiceTexts = { null, "No", "Unknown", "All of the above" };

		string[] emptyChoices = { "1", "2", string.Empty, "4" };
		string[] emptyChoiceTexts = { "Yes", "No", "Unknown", string.Empty };

		string[] spaceChoices = { " ", "2", "3", "4" };
		string[] spaceChoiceTexts = { "Yes", "No", " ", "All of the above" };

		string badRightAnswer = "0";

		#endregion

		Question testObject1, testObject2, testObject3;

		[SetUp]
		public void Setup()
		{
			testObject1 = new Question(goodNumber1, goodText1, goodChoices1, goodChoicesTexts1, goodRightAnswer1);
			testObject2 = new Question(goodNumber2, goodText2, goodChoices2, goodChoicesTexts2, goodRightAnswer2);
			testObject3 = new Question(goodNumber3, goodText3, goodChoices3, goodChoicesTexts3, goodRightAnswer3);
		}

		[Test]
		public void Constructor()
		{
			#region "Normal case"

			checkValidConstruction(goodNumber1, goodText1, goodChoices1, goodChoicesTexts1, goodRightAnswer1);

			#endregion

			#region "Strange but valid"

			checkValidConstruction(goodNumber1, goodText1, goodChoices1, emptyChoiceTexts, goodRightAnswer1);
			checkValidConstruction(goodNumber1, goodText1, goodChoices1, spaceChoiceTexts, goodRightAnswer1);

			#endregion

			#region "Nulls and empties"

			checkForArgumentNullException(null, goodText1, goodChoices1, goodChoicesTexts1, goodRightAnswer1);
			checkForArgumentNullException(empty, goodText1, goodChoices1, goodChoicesTexts1, goodRightAnswer1);
			checkForArgumentNullException(space, goodText1, goodChoices1, goodChoicesTexts1, goodRightAnswer1);

			checkForArgumentNullException(goodNumber1, null, goodChoices1, goodChoicesTexts1, goodRightAnswer1);
			checkForArgumentNullException(goodNumber1, empty, goodChoices1, goodChoicesTexts1, goodRightAnswer1);
			checkForArgumentNullException(goodNumber1, space, goodChoices1, goodChoicesTexts1, goodRightAnswer1);

			checkForArgumentNullException(goodNumber1, goodText1, null, goodChoicesTexts1, goodRightAnswer1);
			checkForArgumentNullException(goodNumber1, goodText1, nullChoices, goodChoicesTexts1, goodRightAnswer1);
			checkForArgumentNullException(goodNumber1, goodText1, emptyChoices, goodChoicesTexts1, goodRightAnswer1);
			checkForArgumentNullException(goodNumber1, goodText1, spaceChoices, goodChoicesTexts1, goodRightAnswer1);

			checkForArgumentNullException(goodNumber1, goodText1, goodChoices1, null, goodRightAnswer1);
			checkForArgumentNullException(goodNumber1, goodText1, goodChoices1, nullChoiceTexts, goodRightAnswer1);

			checkForArgumentNullException(goodNumber1, goodText1, goodChoices1, goodChoicesTexts1, null);
			checkForArgumentNullException(goodNumber1, goodText1, goodChoices1, goodChoicesTexts1, empty);
			checkForArgumentNullException(goodNumber1, goodText1, goodChoices1, goodChoicesTexts1, space);

			#endregion

			#region "Invalid array lengths"
		
			checkForArgumentOutOfRangeException(goodNumber1, goodText1, badChoices1, badChoicesTexts1, goodRightAnswer1);
			checkForArgumentOutOfRangeException(goodNumber1, goodText1, badChoices2, badChoicesTexts2, goodRightAnswer1);

			#endregion

			#region "Uneven array lengths"

			checkForArgumentOutOfRangeException(goodNumber1, goodText1, shortChoices, goodChoicesTexts1, goodRightAnswer1);
			checkForArgumentOutOfRangeException(goodNumber1, goodText1, goodChoices1, longChoicesTexts, goodRightAnswer1);

			#endregion

			#region "Answer not in choices"

			checkForArgumentOutOfRangeException(goodNumber1, goodText1, goodChoices1, goodChoicesTexts1, badRightAnswer);

			#endregion

			#region "Duplicates"

			checkForArgumentException(goodNumber1, goodText1, badChoices3, goodChoicesTexts1, goodRightAnswer1);
			checkForArgumentException(goodNumber1, goodText1, goodChoices1, badChoicesTexts3, goodRightAnswer1);

			#endregion
		}

		[Test]
		public void IsResponseCorrect()
		{
			try
			{
				#region "Right answer"

				Assert.IsTrue(testObject1.IsResponseCorrect(goodRightAnswer1));
				Assert.IsTrue(testObject2.IsResponseCorrect(goodRightAnswer2));
				Assert.IsTrue(testObject3.IsResponseCorrect(goodRightAnswer3));

				#endregion

				#region "Wrong answer"

				Assert.IsFalse(testObject1.IsResponseCorrect(goodRightAnswer2));
				Assert.IsFalse(testObject2.IsResponseCorrect(goodRightAnswer3));
				Assert.IsFalse(testObject3.IsResponseCorrect(goodRightAnswer1));

				#endregion

				#region "Invalid answer"

				Assert.IsFalse(testObject1.IsResponseCorrect(null));
				Assert.IsFalse(testObject2.IsResponseCorrect(empty));
				Assert.IsFalse(testObject3.IsResponseCorrect(space));
				Assert.IsFalse(testObject1.IsResponseCorrect("0"));

				#endregion
			}
			catch(Exception e)
			{
				Assert.Fail("Unexpected Exception: " + e.ToString());
			}
		}

		private void checkValidConstruction(string number, string text, string[] choices, string[] choiceTexts, string answer)
		{
			try
			{
				Question constructedTestObject = new Question(number, text, choices, choiceTexts, answer);
				Assert.AreEqual(number, constructedTestObject.QuestionNumber);
				Assert.AreEqual(text, constructedTestObject.QuestionText);
				Assert.AreEqual(choices.Length, constructedTestObject.ResponseNumbers.Length);
				Assert.AreEqual(choiceTexts.Length, constructedTestObject.ResponseTexts.Length);
				for (int i = 0; i < choices.Length; i++)
				{
					Assert.AreEqual(choices[i], constructedTestObject.ResponseNumbers[i]);
					Assert.AreEqual(choiceTexts[i], constructedTestObject.ResponseTexts[i]);
				}
				Assert.AreEqual(answer, constructedTestObject.CorrectResponseNumber);
			}
			catch (Exception e)
			{
				Assert.Fail("Unexpected exception " + e.ToString());
			}
		}

		private void checkForArgumentNullException(string number, string text, string[] choices, string[] choiceTexts, string answer)
		{
			try
			{
				Question constructedTestObject = new Question(number, text, choices, choiceTexts, answer);
				Assert.Fail();
			}
			catch (ArgumentNullException)
			{
				//Do nothing, this is good
			}
			catch (Exception e)
			{
				Assert.Fail("Unexpected exception " + e.ToString());
			}
		}

		private void checkForArgumentOutOfRangeException(string number, string text, string[] choices, string[] choiceTexts, string answer)
		{
			try
			{
				Question constructedTestObject = new Question(number, text, choices, choiceTexts, answer);
				Assert.Fail();
			}
			catch (ArgumentOutOfRangeException)
			{
				//Do nothing, this is good
			}
			catch (Exception e)
			{
				Assert.Fail("Unexpected exception " + e.ToString());
			}
		}

		//Do not use to check is a child of ArgumentException was thrown
		private void checkForArgumentException(string number, string text, string[] choices, string[] choiceTexts, string answer)
		{
			try
			{
				Question constructedTestObject = new Question(number, text, choices, choiceTexts, answer);
				Assert.Fail();
			}
			catch (ArgumentException)
			{
				//Do nothing, this is good
			}
			catch (Exception e)
			{
				Assert.Fail("Unexpected exception " + e.ToString());
			}
		}
	}
}