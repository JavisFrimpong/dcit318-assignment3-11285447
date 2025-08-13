using System;
using System.Collections.Generic;
using System.IO;

namespace StudentGradingSystem
{
    // Custom Exceptions
    public class InvalidScoreFormatException : Exception
    {
        public InvalidScoreFormatException(string message) : base(message) { }
    }

    public class MissingFieldException : Exception
    {
        public MissingFieldException(string message) : base(message) { }
    }

    // Student class
    public class Student
    {
        public int Id;
        public string FullName;
        public int Score;

        public Student(int id, string fullName, int score)
        {
            Id = id;
            FullName = fullName;
            Score = score;
        }

        public string GetGrade()
        {
            if (Score >= 80) return "A";
            if (Score >= 70) return "B";
            if (Score >= 60) return "C";
            if (Score >= 50) return "D";
            return "F";
        }
    }

    // Processor class
    public class StudentResultProcessor
    {
        public List<Student> ReadStudentsFromFile(string inputFilePath)
        {
            var students = new List<Student>();

            using (StreamReader reader = new StreamReader(inputFilePath))
            {
                string? line;
                int lineNumber = 0;

                while ((line = reader.ReadLine()) != null)
                {
                    lineNumber++;
                    try
                    {
                        var parts = line.Split(',');

                        for (int i = 0; i < parts.Length; i++)
                            parts[i] = parts[i].Trim();

                        if (parts.Length != 3)
                            throw new MissingFieldException($"Line {lineNumber}: Missing fields → \"{line}\"");

                        if (!int.TryParse(parts[0], out int id))
                            throw new MissingFieldException($"Line {lineNumber}: Invalid ID → \"{line}\"");

                        string name = parts[1];
                        string scoreText = parts[2].ToLower();

                        int score;
                        if (!int.TryParse(scoreText, out score))
                        {
                            score = ConvertWordsToNumber(scoreText);
                            if (score == -1)
                                throw new InvalidScoreFormatException($"Line {lineNumber}: Invalid score word → \"{scoreText}\"");
                        }

                        students.Add(new Student(id, name, score));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"\u26A0\uFE0F Error: {ex.Message}");
                        continue;
                    }
                }
            }

            return students;
        }

        public void WriteReportToFile(List<Student> students, string outputFilePath)
        {
            using (StreamWriter writer = new StreamWriter(outputFilePath))
            {
                foreach (var student in students)
                {
                    writer.WriteLine($"{student.FullName} (ID: {student.Id}): Score = {student.Score}, Grade = {student.GetGrade()}");
                }
            }
        }

        private int ConvertWordsToNumber(string input)
        {
            var singleDigits = new Dictionary<string, int>
            {
                {"zero", 0}, {"one", 1}, {"two", 2}, {"three", 3}, {"four", 4},
                {"five", 5}, {"six", 6}, {"seven", 7}, {"eight", 8}, {"nine", 9}
            };

            var teens = new Dictionary<string, int>
            {
                {"ten", 10}, {"eleven", 11}, {"twelve", 12}, {"thirteen", 13},
                {"fourteen", 14}, {"fifteen", 15}, {"sixteen", 16},
                {"seventeen", 17}, {"eighteen", 18}, {"nineteen", 19}
            };

            var tens = new Dictionary<string, int>
            {
                {"twenty", 20}, {"thirty", 30}, {"forty", 40}, {"fifty", 50},
                {"sixty", 60}, {"seventy", 70}, {"eighty", 80}, {"ninety", 90}
            };

            input = input.Replace("-", " ");
            var words = input.Split(" ", StringSplitOptions.RemoveEmptyEntries);

            int result = 0;

            if (words.Length == 1)
            {
                if (singleDigits.ContainsKey(words[0])) return singleDigits[words[0]];
                if (teens.ContainsKey(words[0])) return teens[words[0]];
                if (tens.ContainsKey(words[0])) return tens[words[0]];
            }
            else if (words.Length == 2)
            {
                if (tens.ContainsKey(words[0]) && singleDigits.ContainsKey(words[1]))
                {
                    return tens[words[0]] + singleDigits[words[1]];
                }
            }

            return -1;
        }
    }

    class Program
    {
        static void Main()
        {
            string inputPath = "students.txt";
            string outputPath = "report.txt";

            var processor = new StudentResultProcessor();

            try
            {
                var students = processor.ReadStudentsFromFile(inputPath);

                if (students.Count == 0)
                {
                    Console.WriteLine("\u274C No valid students found. Report not generated.");
                }
                else
                {
                    processor.WriteReportToFile(students, outputPath);
                    Console.WriteLine($"\u2705 Report written to: {outputPath}");
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine($"\u274C File not found: {inputPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\u274C Unexpected Error: {ex.Message}");
            }
        }
    }
}
