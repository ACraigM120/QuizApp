using NUnit.Framework;
using System;
using System.IO;
using System.Reflection;
using QuizApp;

namespace UnitTests
{
	public class UnitTestQuiz
	{
		Quiz testObject1;
		MockReader testQuestionReader1, testQuestionReader2;
		MockQuestion[] testQuestions1, allFirstAnswer, empty;
		StreamWriter testWriter, setupWriter;
		StreamReader testReader, outputReader;

		Random random = new Random(DateTime.Now.Millisecond + DateTime.Now.Second * 1000);
		int targetScore;

		int introTextLines = 13;

		string questionReaderName = "questionReader";
		string writerName = "writer";
		string readerName = "reader";

		//Input files
		string fileInput1 = "input1.txt";
		string fileInputAll1 = "inputAll1.txt";
		string fileInputAll2 = "inputAll2.txt";
		string fileInputRandom = "inputRandom.txt";
		string fileInput = "input.txt";		//Use this one when it doesn't matter

		//Output files
		string fileOutput1 = "output1.txt";
		string fileOutput = "output.txt";	//Use this one when it doesn't matter

		//Keywords we expect to see in output
		string correct = "Correct";
		string incorrect = "Incorrect";
		string outOf = "out of";
		string warning = "Warning";
		string error = "Error";
		string invalid = "Invalid";
		string ignoring = "Ignoring";

		Type testObjectType = typeof(Quiz);
		FieldInfo questionReaderInfo, writerInfo, readerInfo;

		[OneTimeSetUp]
		public void Initialize()
		{
			questionReaderInfo = testObjectType.GetField(questionReaderName, BindingFlags.NonPublic | BindingFlags.Instance);
			writerInfo = testObjectType.GetField(writerName, BindingFlags.NonPublic | BindingFlags.Instance);
			readerInfo = testObjectType.GetField(readerName, BindingFlags.NonPublic | BindingFlags.Instance);

			#region "Questions"

			//Normal-ish test case
			testQuestions1 = new MockQuestion[2];
			testQuestions1[0] = new MockQuestion("0", "Test", new string[] { "1", "2", "3", "4" }, new string[] { "F", "D", "C", "Z" }, "3");
			testQuestions1[1] = new MockQuestion("1", "Test", new string[] { "1", "2", "3", "4" }, new string[] { "A", "D", "E", "F" }, "1");

			//All correct answers are the 1st choice
			allFirstAnswer = new MockQuestion[10];
			for (int i = 0; i < allFirstAnswer.Length; i++)
			{
				allFirstAnswer[i] = new MockQuestion(i.ToString(), "Test question", new string[] { "1", "2" }, new string[] { "Test answer1", "Test answer2" }, "1");
			}

			//Empty set of questions
			empty = new MockQuestion[0];

			#endregion

			#region "Answers"

			//All 1st choice
			setupWriter = new StreamWriter(File.Create(fileInputAll1));
			for(int i = 0; i < 10; i++)
			{
				setupWriter.WriteLine("1");
				setupWriter.Flush();
			}
			setupWriter.Close();

			//All 2nd choice
			setupWriter = new StreamWriter(File.Create(fileInputAll2));
			for (int i = 0; i < 10; i++)
			{
				setupWriter.WriteLine("2");
				setupWriter.Flush();
			}
			setupWriter.Close();

			//Random % correct
			targetScore = random.Next(1, 10);
			setupWriter = new StreamWriter(File.Create(fileInputRandom));
			for (int i = 0; i < 10; i++)
			{
				setupWriter.WriteLine(i < targetScore?"1":"2");
				setupWriter.Flush();
			}
			setupWriter.Close();

			//Normal-ish test case
			setupWriter = new StreamWriter(File.Create(fileInput1));
			setupWriter.WriteLine("3");
			setupWriter.WriteLine("4");
			setupWriter.Flush();
			setupWriter.Close();

			#endregion
		}

		[SetUp]
		public void Setup()
		{
			testQuestionReader1 = new MockReader();
			testQuestionReader2 = new MockReader();
			testWriter = new StreamWriter(File.Create(fileOutput));
			testReader = new StreamReader(File.Create(fileInput));
			testObject1 = new Quiz(testQuestionReader1, testWriter, testReader);
		}

		[TearDown]
		public void Cleanup()
		{
			testWriter.Close();
			testReader.Close();
		}

		[Test]
		public void Constructor()
		{
			Quiz constructedTestObject;
			object propertyValue;

			#region "Normal"

			constructedTestObject = new Quiz(testQuestionReader1, Console.Out, Console.In);
			propertyValue = questionReaderInfo.GetValue(constructedTestObject);
			Assert.AreEqual(testQuestionReader1, (IQuestionReader)propertyValue);
			propertyValue = writerInfo.GetValue(constructedTestObject);
			Assert.AreEqual(Console.Out, (TextWriter)propertyValue);
			propertyValue = readerInfo.GetValue(constructedTestObject);
			Assert.AreEqual(Console.In, (TextReader)propertyValue);

			#endregion

			#region "nulls"

			checkForArgumentNullException(null, Console.Out, Console.In);
			checkForArgumentNullException(testQuestionReader1, null, Console.In);
			checkForArgumentNullException(testQuestionReader1, Console.Out, null);

			#endregion
		}

		[Test]
		public void SetQuestionReader()
		{
			object propertyValue;

			//Normal
			testObject1.QuestionReader = testQuestionReader2;
			propertyValue = questionReaderInfo.GetValue(testObject1);
			Assert.AreEqual(testQuestionReader2, (IQuestionReader)propertyValue);

			//Null
			try
			{
				testObject1.QuestionReader = null;
				Assert.Fail();
			}
			catch(ArgumentNullException)
			{
				//Do nothing, this is good
			}
			catch(Exception e)
			{
				Assert.Fail("Unexpected Exception " + e.ToString());
			}
		}

		[Test]
		public void SetWriter()
		{
			object propertyValue;

			//Normal
			testObject1.Writer = Console.Out;
			propertyValue = writerInfo.GetValue(testObject1);
			Assert.AreEqual(Console.Out, (TextWriter)propertyValue);

			//Null
			try
			{
				testObject1.Writer = null;
				Assert.Fail();
			}
			catch (ArgumentNullException)
			{
				//Do nothing, this is good
			}
			catch (Exception e)
			{
				Assert.Fail("Unexpected Exception " + e.ToString());
			}
		}

		[Test]
		public void SetReader()
		{
			object propertyValue;

			//Normal
			testObject1.Reader = Console.In;
			propertyValue = readerInfo.GetValue(testObject1);
			Assert.AreEqual(Console.In, (TextReader)propertyValue);

			//Null
			try
			{
				testObject1.Reader = null;
				Assert.Fail();
			}
			catch (ArgumentNullException)
			{
				//Do nothing, this is good
			}
			catch (Exception e)
			{
				Assert.Fail("Unexpected Exception " + e.ToString());
			}
		}

		[Test]
		public void RunQuiz()
		{
			float output;
			string text;
			int index;

			#region "Score check"

			checkScore(allFirstAnswer, fileInputAll1, allFirstAnswer.Length, 1f);
			checkScore(allFirstAnswer, fileInputAll2, 0, 0f);
			checkScore(allFirstAnswer, fileInputRandom, targetScore);

			#endregion

			#region "Output check"

			testWriter.Close();
			testWriter = new StreamWriter(File.Create(fileOutput1));
			output = checkScore(testQuestions1, fileInput1, 1, 0.5f);

			testWriter.Close();
			testReader.Close();
			outputReader = new StreamReader(File.OpenRead(fileOutput1));

			//I don't really have an automated test for the instructions, nor do I see a need for one
			//It is better reviewed manually
			for (int i = 0; i < introTextLines; i++)
			{
				text = outputReader.ReadLine();
			}

			for(int i = 0; i < testQuestions1.Length; i++)
			{
				text = outputReader.ReadLine();
				Assert.IsTrue(text.Contains(testQuestions1[i].QuestionNumber));
				index = text.IndexOf(testQuestions1[i].QuestionNumber) + testQuestions1[i].QuestionNumber.Length;
				Assert.IsTrue(text.Substring(index).Contains(testQuestions1[i].QuestionText));
				for (int j = 0; j < testQuestions1[i].ResponseNumbers.Length; j++)
				{
					text = outputReader.ReadLine();
					Assert.IsTrue(text.Contains(testQuestions1[i].ResponseNumbers[j]));
					index = text.IndexOf(testQuestions1[i].ResponseNumbers[j]) + testQuestions1[i].ResponseNumbers[j].Length;
					Assert.IsTrue(text.Substring(index).Contains(testQuestions1[i].ResponseTexts[j]));
				}
				outputReader.ReadLine();
				text = outputReader.ReadLine();
				if (i == 0)
				{
					Assert.IsTrue(text.StartsWith(correct));
					index = correct.Length;
				}
				else
				{
					Assert.IsTrue(text.StartsWith(incorrect));
					index = incorrect.Length;
				}
				Assert.IsTrue(text.Substring(index).Contains(testQuestions1[i].CorrectResponseNumber));
				outputReader.ReadLine();
			}
			outputReader.ReadLine();
			text = outputReader.ReadLine();
			Assert.IsTrue(text.Contains(testObject1.CorrectAnswers.ToString()));
			index = text.IndexOf(testObject1.CorrectAnswers.ToString()) + testObject1.CorrectAnswers.ToString().Length;
			text = text.Substring(index);
			Assert.IsTrue(text.Contains(outOf));
			index = text.IndexOf(outOf) + outOf.Length;
			text = text.Substring(index);
			Assert.IsTrue(text.Contains(testObject1.TotalQuestions.ToString()));
			index = text.IndexOf(testObject1.TotalQuestions.ToString()) + testObject1.TotalQuestions.ToString().Length;
			text = text.Substring(index);
			Assert.IsTrue(text.Contains(output.ToString("P0")));
			outputReader.Close();

			#endregion

			#region "Empty quiz"

			testWriter = new StreamWriter(File.Create(fileOutput1));
			checkScore(empty, fileInput1, 0, float.NaN);

			#endregion

			#region "Error output check"

			testQuestionReader1.GarbageMode = true;
			testQuestionReader1.MyQuestions = allFirstAnswer;
			testWriter.Close();
			testWriter = new StreamWriter(File.Create(fileOutput1));
			testObject1 = new Quiz(testQuestionReader1, testWriter, testReader);
			testObject1.RunQuiz();

			testWriter.Close();
			testReader.Close();

			outputReader = new StreamReader(File.OpenRead(fileOutput1));

			for (int i = 0; i < introTextLines; i++)
			{
				outputReader.ReadLine();
			}
			for(int i = 0; i < allFirstAnswer.Length; i++)
			{
				text = outputReader.ReadLine();
				Assert.IsTrue(text.StartsWith(warning, StringComparison.OrdinalIgnoreCase));
				switch (i % 4)
				{
					case 0:
					case 1:
					case 2:
						Assert.IsTrue(text.Contains(error, StringComparison.OrdinalIgnoreCase));

						//It's probably best not to assume the exception string has a fixed length...
						for (int j = 0; j <= 50; j++)
						{
							text = outputReader.ReadLine();
							if (text.Contains(ignoring, StringComparison.OrdinalIgnoreCase))
							{
								break;
							}
							else
							{
								//... even so, 50 lines would be pretty long
								if(j == 50)
								{
									Assert.Fail();
								}
							}
						}
						break;
					default:
						Assert.IsTrue(text.Contains(invalid, StringComparison.OrdinalIgnoreCase));
						text = outputReader.ReadLine();
						Assert.IsTrue(text.Contains(ignoring, StringComparison.OrdinalIgnoreCase));
						break;
				}
				outputReader.ReadLine();
			}
			outputReader.Close();
			#endregion

		}

		private void checkForArgumentNullException(IQuestionReader question, TextWriter writer, TextReader reader, bool useConstructor = false)
		{
			try
			{
				Quiz constructedTestObject = new Quiz(question, writer, reader);
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

		private float checkScore(MockQuestion[] questions, string inputFile, int targetScore, float floatScore = -1f)
		{
			float output;
			if(floatScore < 0f)
			{
				floatScore = (0f + targetScore) / questions.Length;
			}
			testQuestionReader1.MyQuestions = questions;
			testReader.Close();
			testReader = new StreamReader(File.OpenRead(inputFile));
			testObject1 = new Quiz(testQuestionReader1, testWriter, testReader);
			output = testObject1.RunQuiz();
			Assert.AreEqual(floatScore, output);
			Assert.AreEqual(questions.Length, testObject1.TotalQuestions);
			Assert.AreEqual(targetScore, testObject1.CorrectAnswers);

			return output;
		}
	}
}