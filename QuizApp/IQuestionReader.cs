namespace QuizApp
{
	/// <summary>
	/// Interface for input question file readers to implement. 
	/// This leaves the door open to supporting other file formats in the future.
	/// </summary>
	public interface IQuestionReader
	{
		/// <summary>
		/// The name of the file containing the questions.
		/// </summary>
		public string FileName { get; set; }

		/// <summary>
		/// Reads questions all at once from an input file.
		/// </summary>
		/// <returns>An array of Questions read from the file.</returns>
		public Question[] ReadAllQuestions();

		/// <summary>
		/// Checks if the file contains more content to parse.
		/// </summary>
		/// <returns>True if we are not done parsing the file.</returns>
		public bool HasMoreContent();

		/// <summary>
		/// Reads a single question.
		/// </summary>
		/// <returns>The next valid question from the file. Null if we've reached the end of the file.</returns>
		public Question ReadNextQuestion();
	}
}
