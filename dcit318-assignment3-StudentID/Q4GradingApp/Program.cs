
using System;
using System.Collections.Generic;
using System.IO;

namespace Q4GradingApp
{
    // a) Student class
    public class Student
    {
        public int Id { get; set; }
        public string FullName { get; set; } = "";
        public int Score { get; set; }

        public string GetGrade()
        {
            return Score switch
            {
                >= 80 and <= 100 => "A",
                >= 70 and <= 79  => "B",
                >= 60 and <= 69  => "C",
                >= 50 and <= 59  => "D",
                _ => "F"
            };
        }
    }

    // b & c) Custom exceptions
    public class InvalidScoreFormatException : Exception { public InvalidScoreFormatException(string message) : base(message) {} }
    public class MissingFieldException : Exception { public MissingFieldException(string message) : base(message) {} }

    // d) Processor
    public class StudentResultProcessor
    {
        public List<Student> ReadStudentsFromFile(string inputFilePath)
        {
            var result = new List<Student>();
            using var reader = new StreamReader(inputFilePath);
            string? line;
            int lineNo = 0;
            while ((line = reader.ReadLine()) != null)
            {
                lineNo++;
                if (string.IsNullOrWhiteSpace(line)) continue;
                var parts = line.Split(',');
                if (parts.Length < 3)
                    throw new MissingFieldException($"Line {lineNo}: Expected 3 fields, found {parts.Length}.");

                var idStr = parts[0].Trim();
                var name = parts[1].Trim();
                var scoreStr = parts[2].Trim();

                if (!int.TryParse(idStr, out var id))
                    throw new FormatException($"Line {lineNo}: Invalid ID format '{idStr}'.");

                if (!int.TryParse(scoreStr, out var score))
                    throw new InvalidScoreFormatException($"Line {lineNo}: Invalid score format '{scoreStr}'.");

                result.Add(new Student { Id = id, FullName = name, Score = score });
            }
            return result;
        }

        public void WriteReportToFile(List<Student> students, string outputFilePath)
        {
            using var writer = new StreamWriter(outputFilePath);
            foreach (var s in students)
            {
                writer.WriteLine($"{s.FullName} (ID: {s.Id}): Score = {s.Score}, Grade = {s.GetGrade()}");
            }
        }
    }

    public static class Program
    {
        public static void Main()
        {
            Console.WriteLine("Enter input file path:");
            var input = Console.ReadLine() ?? "students_input.txt";
            Console.WriteLine("Enter output file path:");
            var output = Console.ReadLine() ?? "students_report.txt";

            try
            {
                var proc = new StudentResultProcessor();
                var students = proc.ReadStudentsFromFile(input);
                proc.WriteReportToFile(students, output);
                Console.WriteLine($"Report written to: {output}");
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"File not found: {ex.Message}");
            }
            catch (InvalidScoreFormatException ex)
            {
                Console.WriteLine($"Invalid score: {ex.Message}");
            }
            catch (MissingFieldException ex)
            {
                Console.WriteLine($"Missing field: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
            }
        }
    }
}
