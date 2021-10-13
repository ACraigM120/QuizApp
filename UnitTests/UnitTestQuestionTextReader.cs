using NUnit.Framework;
using System;
using System.IO;
using System.Reflection;
using QuizApp;

namespace UnitTests
{
	public class UnitTestQuestionTextReader
	{
		//Some text to use for testing.
		string loremIpsum = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.";
	   
		string readerName = "reader";

		//File names
		string validFileName1 = "foo.bar";				//Content unimportant
		string validFileName1a = "meh.txt";				//Content unimportant
		string shortFileName = "short.txt";				//Contains one question
		string longFileName = "long.txt";				//Contains many questions
		string strangeContentFileName = "strange.txt";  //Contains questions that are technically valid
		string badContentFileName = "bad.txt";          //Contains some good data and many format mistakes
		string oneCharFileName = "oneChar.txt";			//Contains a single character
		string emptyFileName = "empty.txt";				//Empty text file
		string invalidFileName1 = "bar.foo";

		string empty = string.Empty; //because I don't feel like typing it out every time
		string space = " ";

		MockQuestion[] shortExpected, longExpected, strangeExpected, badExpected; //All others null is expected or content is not important

		Type testObjectType = typeof(QuestionTextReader);
		FieldInfo readerInfo;
		
		QuestionTextReader constructedTestObject;
		QuestionTextReader testObject1;

		// Generate my test files' content. They are also included in the project for convenient reference.
		[OneTimeSetUp]
		public void Initialize()
		{
			StreamWriter writer;
			readerInfo = testObjectType.GetField(readerName, BindingFlags.NonPublic | BindingFlags.Instance);

			#region "files where content isn't important"

			File.Create(validFileName1).Close(); //If we don't close it we won't be able to read it
			File.Create(validFileName1a).Close();
			File.Delete(invalidFileName1);

			#endregion

			#region "short file"

			writer = new StreamWriter(File.Create(shortFileName)); 
			shortExpected = new MockQuestion[1]; 
			shortExpected[0] = new MockQuestion("1", "What is the hex color for white?", new string[] {"1", "2"}, new string[] { "#FFFFFF", "#000000" }, "1");
			//for(int i = 0; i < shortExpected.Length; i++)
			//{ 
			//	writer.Write(shortExpected[i].ToString());
			//	writer.Flush();
			//}
			writer.Write(shortExpected[0].ToString());
			writer.Flush();
			writer.Close();

			#endregion

			#region "long file"

			writer = new StreamWriter(File.Create(longFileName));
			longExpected = new MockQuestion[1000]; //Large size to give a rough idea of the performance reading all questions at once for very large quizzes
			for(int i = 0; i < longExpected.Length; i++)
			{
				longExpected[i] = new MockQuestion(i.ToString(), loremIpsum.Substring(i % loremIpsum.Length),
					new string[] { i.ToString(), (i + 1).ToString(), (i + 2).ToString(), (i + 3).ToString() },
					new string[] { loremIpsum.Substring(0, i % loremIpsum.Length), loremIpsum.Substring(0, (1 + i) % loremIpsum.Length), loremIpsum.Substring(0, (2 + i) % loremIpsum.Length), loremIpsum.Substring(0, (3 + i) % loremIpsum.Length)},
					(i + i % 4).ToString());
				writer.Write(longExpected[i].ToString());
				writer.Flush();
			}
			writer.Close();

			#endregion

			#region "strange file"

			writer = new StreamWriter(File.Create(strangeContentFileName));
			strangeExpected = new MockQuestion[12];
			strangeExpected[0] = new MockQuestion("A", "What is your name?", new string[] { "1", "2", "3" }, new string[] { "King Arthur", "Merlin", "Sir Lancelot" }, "3");
			strangeExpected[1] = new MockQuestion("9", "What is your quest?", new string[] { "A", "B", "C" }, new string[] { "To seek the Holy Grail", "Destroy the One Ring", "Avenge the town of Starting Point" }, "A");
			strangeExpected[2] = new MockQuestion("c", "What is your favorite color?", new string[] { "1", "2", "3", "4" }, new string[] { "red", "green", "blue", "yellow" }, "3");
			strangeExpected[3] = new MockQuestion("9", "What is your name?", new string[] { "a", "b", "c" }, new string[] { "Darth Vader", "Sir Robin", "Sir Lancelot" }, "b");
			strangeExpected[4] = new MockQuestion("%", "What is your quest?", new string[] { "1", "2", "3" }, new string[] { "blue", "Save the princess", "To seek the Holy Grail" }, "3");
			strangeExpected[5] = new MockQuestion("9", "What is the capital of Assyria?", new string[] { "!", "@", "#", "$", "%" }, new string[] { "Assur", "Damascus", "Mos Eisley", "Tar Valon", "I don't know that!" }, "!");
			strangeExpected[6] = new MockQuestion("7)1", "What is your name?", new string[] { "1", "2" }, new string[] { "Sir Galahad", "Frodo" }, "1");
			strangeExpected[7] = new MockQuestion("9", "What is your quest?", new string[] { "1.1", "2.2" }, new string[] { "To find a date for the dance", "To seek the Holy Grail" }, "2.2");
			strangeExpected[8] = new MockQuestion(")", "What is your favorite color?", new string[] { "1", "2" }, new string[] { "blue", "yellow" }, "2");
			strangeExpected[9] = new MockQuestion("9", "What is your name?", new string[] { ".", "?", "!" }, new string[] { "Arthur, King of the Brittons", "Merlin", "Gollum" }, ".");
			strangeExpected[10] = new MockQuestion("-3.14159", "What is your quest?", new string[] { "This", "That", "Other" }, new string[] { "To destroy the Death Star", "To seek the Holy Grail", "To win $1 million" }, "That");
			strangeExpected[11] = new MockQuestion(" 1 2 ", "What is the airspeed velocity of an unladen swallow?", new string[] { " 1 ", " 2 " }, new string[] { "African", "European" }, " 2 ");
			for (int i = 0; i < strangeExpected.Length; i++)
			{
				writer.Write(strangeExpected[i].ToString());
				writer.Flush();
			}
			writer.Close();

			#endregion

			#region "bad file"

			writer = new StreamWriter(File.Create(badContentFileName));
			badExpected = new MockQuestion[10];
			badExpected[0] = new MockQuestion(") ", "What does the fox say?", new string[] { "1", "2" }, new string[] { "Yes", "No" }, "2");
			badExpected[1] = new MockQuestion("X", "Kaboom?", new string[] { "1", "2" }, new string[] { "Yes", "No" }, "1");
			badExpected[1].number = empty;
			badExpected[2] = new MockQuestion("1", "X", new string[] { "1", "2" }, new string[] { "Yes", "No" }, "1");
			badExpected[2].text = empty;
			badExpected[3] = new MockQuestion("2", "To be?", new string[] { "1", "2" }, new string[] { "Yes", "No" }, "1");
			badExpected[3].choices = new string[] { "1" };
			badExpected[3].choiceTexts = new string[] { "Yes" };
			badExpected[4] = new MockQuestion("3", "Or not to be?", new string[] { "1", "2" }, new string[] { "Yes", "No" }, "1");
			badExpected[4].choices = new string[] { };
			badExpected[4].choiceTexts = new string[] { };
			badExpected[5] = new MockQuestion("4", "?", new string[] { ". ", "2" }, new string[] { "Yes", "No" }, ". ");
			badExpected[6] = new MockQuestion("5", "!", new string[] { "1", "2" }, new string[] { "Yes", "No" }, "1");
			badExpected[6].choices = new string[] { "1", "1" };
			badExpected[7] = new MockQuestion("6", "!", new string[] { "1", "2" }, new string[] { "Yes", "No" }, "1");
			badExpected[7].choiceTexts = new string[] {"Yes", "Yes"};
			badExpected[8] = new MockQuestion("X", "X", new string[] { "1", "2" }, new string[] { "X", "Y" }, "1");
			badExpected[8].number = empty;
			badExpected[8].text = empty;
			badExpected[8].choices = new string[] { empty, empty };
			badExpected[8].choiceTexts = new string[] { empty };
			badExpected[8].correct = empty;
			badExpected[9] = new MockQuestion("99", "Is there anything salvageable from this mess?", new string[] { "1", "2" }, new string[] { "Yes", "No" }, "1");
			for (int i = 0; i < badExpected.Length; i++)
			{
				if(i % 3 == 0)
				{
					writer.WriteLine();
				}
				writer.Write(badExpected[i].ToString());
				writer.Flush();
			}
			writer.WriteLine("3) Garbage question?");
			writer.WriteLine("1. Yes");
			writer.WriteLine("2. No");
			writer.WriteLine("1");
			writer.WriteLine();
			writer.Flush();
			writer.Close();

			#endregion

			#region "single character file"

			writer = new StreamWriter(File.Create(oneCharFileName));
			writer.Write("a");
			writer.Flush();
			writer.Close();

			#endregion

			#region "empty file"

			writer = new StreamWriter(File.Create(emptyFileName));
			writer.Flush();
			writer.Close();

			#endregion
		}

		[SetUp]
		public void Setup()
		{
			testObject1 = new QuestionTextReader(validFileName1);
		}

		[TearDown]
		public void Cleanup()
		{
			//Need to make sure the stream is closed.
			((StreamReader)readerInfo.GetValue(testObject1)).Close();

			if(constructedTestObject != null)
			{
				((StreamReader)readerInfo.GetValue(constructedTestObject)).Close();
			}
		}

		[Test]
		public void Constructor()
		{
			#region "Normal"

			if(!File.Exists(validFileName1a))
			{
				Assert.Inconclusive("Valid input file does not exist, test aborted.");
			}
			try
			{
				constructedTestObject = new QuestionTextReader(validFileName1a);
				Assert.AreEqual(validFileName1a, constructedTestObject.FileName);
			}
			catch(Exception e)
			{
				Assert.Fail("Unexpected Exeption: " + e.ToString());
			}

			#endregion

			#region "Empties and nulls"

			checkForArgumentNullException(null, true);
			checkForArgumentNullException(empty, true);
			checkForArgumentNullException(space, true);

			#endregion

			#region "File not found"

			if(File.Exists(invalidFileName1))
			{
				Assert.Inconclusive("Invalid file unexpectedly exists. Test aborted.");
			}
			try
			{
				constructedTestObject = new QuestionTextReader(invalidFileName1);
				Assert.Fail();
			}
			catch(FileNotFoundException)
			{
				//Do nothing, this is good
			}
			catch(Exception e)
			{
				Assert.Fail("Unexpected Exception: " + e.ToString());
			}

			#endregion
		}

		[Test]
		public void SetFileName()
		{
			#region "Normal"

			if (!File.Exists(validFileName1a))
			{
				Assert.Inconclusive("Valid input file does not exist, test aborted.");
			}
			try
			{
				testObject1.FileName = validFileName1a;
				Assert.AreEqual(validFileName1a, testObject1.FileName);
			}
			catch (Exception e)
			{
				Assert.Fail("Unexpected Exeption: " + e.ToString());
			}

			#endregion

			#region "Empties and nulls"

			checkForArgumentNullException(null);
			checkForArgumentNullException(empty);
			checkForArgumentNullException(space);

			#endregion

			#region "File not found"

			if (File.Exists(invalidFileName1))
			{
				Assert.Inconclusive("Invalid file unexpectedly exists. Test aborted.");
			}
			try
			{
				testObject1.FileName = invalidFileName1;
				Assert.Fail();
			}
			catch (FileNotFoundException)
			{
				//Do nothing, this is good
			}
			catch (Exception e)
			{
				Assert.Fail("Unexpected Exception: " + e.ToString());
			}

			#endregion
		}

		[Test]
		public void ReadAllQuestions()
		{
			Question[] output;

			//Try a file with one question
			testObject1.FileName = shortFileName;
			output = testObject1.ReadAllQuestions();
			checkAllQuestions(shortExpected, output);

			//Try a file with lots of questions
			testObject1.FileName = longFileName;
			output = testObject1.ReadAllQuestions();
			checkAllQuestions(longExpected, output);

			//Try a file with some errors in it
			testObject1.FileName = badContentFileName;
			output = testObject1.ReadAllQuestions();
			Assert.AreEqual(1, output.Length);
			checkQuestion(badExpected[9], output[0]);

			//Try an empty file
			testObject1.FileName = emptyFileName;
			output = testObject1.ReadAllQuestions();
			Assert.AreEqual(0, output.Length);
		}

		[Test]
		public void HasMoreContent()
		{
			//Empty file
			StreamReader testReader;
			testObject1.FileName = emptyFileName;
			Assert.IsFalse(testObject1.HasMoreContent());

			//Single character
			testObject1.FileName = oneCharFileName;
			Assert.IsTrue(testObject1.HasMoreContent());
			testReader = ((StreamReader)readerInfo.GetValue(testObject1));
			testReader.Read();
			Assert.IsFalse(testObject1.HasMoreContent());

			//Normal-ish usage
			testObject1.FileName = shortFileName;
			Assert.IsTrue(testObject1.HasMoreContent());
			testReader = ((StreamReader)readerInfo.GetValue(testObject1));
			testReader.Read();
			Assert.IsTrue(testObject1.HasMoreContent());
			testReader.ReadLine();
			Assert.IsTrue(testObject1.HasMoreContent());
			testReader.ReadToEnd();
			Assert.IsFalse(testObject1.HasMoreContent());
		}

		[Test]
		public void ReadNextQuestion()
		{
			Question testOutput;

			//single question file
			testObject1.FileName = shortFileName;
			testOutput = testObject1.ReadNextQuestion();
			checkQuestion(shortExpected[0], testOutput);

			//strange but valid content
			testObject1.FileName = strangeContentFileName;
			for (int i = 0; i < strangeExpected.Length; i++)
			{
				testOutput = testObject1.ReadNextQuestion();
				checkQuestion(strangeExpected[i], testOutput);
			}
			testOutput = testObject1.ReadNextQuestion();
			Assert.IsNull(testOutput);

			//bad content
			testObject1.FileName = badContentFileName;
			for(int i = 0; i < badExpected.Length; i++)
			{
				switch(i)
				{
					case 9:
						testOutput = testObject1.ReadNextQuestion();
						checkQuestion(badExpected[i], testOutput);
						break;
					default:
						try
						{
							testOutput = testObject1.ReadNextQuestion();
							Assert.Fail("Unexpected read of bad data " + i);
						}
						catch (ArgumentException)
						{
							//Expected due to test case
						}
						break;
				}
			}
			testOutput = testObject1.ReadNextQuestion();
			Assert.IsNull(testOutput);

			//single character
			testObject1.FileName = oneCharFileName;
			testOutput = testObject1.ReadNextQuestion();
			Assert.IsNull(testOutput);

			//empty file
			testObject1.FileName = emptyFileName;
			Assert.IsNull(testOutput);
		}

		private void checkForArgumentNullException(string input, bool isConstructor = false)
		{
			try
			{
				if (isConstructor)
				{
					constructedTestObject = new QuestionTextReader(input);
				}
				else
				{
					testObject1.FileName = input;
				}
				Assert.Fail();
			}
			catch (ArgumentNullException)
			{
				//Do nothing, this is good
			}
			catch (Exception e)
			{
				Assert.Fail("Unexpected Exception: " + e.ToString());
			}
		}

		private void checkAllQuestions(MockQuestion[] expected, Question[] actual)
		{
			Assert.AreEqual(expected.Length, actual.Length);
			for (int i = 0; i < expected.Length; i++)
			{
				checkQuestion(expected[i], actual[i]);
			}
		}

		private void checkQuestion(MockQuestion expected, Question actual)
		{
			Assert.AreEqual(expected.number, actual.QuestionNumber);
			Assert.AreEqual(expected.text, actual.QuestionText);
			Assert.AreEqual(expected.choices.Length, actual.ResponseNumbers.Length);
			Assert.AreEqual(expected.choiceTexts.Length, actual.ResponseTexts.Length);
			for (int j = 0; j < expected.choices.Length; j++)
			{
				Assert.AreEqual(expected.choices[j], actual.ResponseNumbers[j]);
				Assert.AreEqual(expected.choiceTexts[j], actual.ResponseTexts[j]);
			}
			Assert.AreEqual(expected.correct, actual.CorrectResponseNumber);
		}
	}
}