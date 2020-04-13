using System;
using System.Collections.Generic;
using System.Linq;

namespace HammingCalculator
{
    class Program
    {
        static void Main(string[] args)
        {
            do
            {
                Console.WriteLine("Pick an option:\n");
                Console.WriteLine("1: Hamming code word generation");
                Console.WriteLine("2: Hamming code error detection\n");

                int option = int.Parse(Console.ReadLine());

                Console.WriteLine("\nInput hamming code:");

                var sequence = Console.ReadLine().Split(" ").Select(int.Parse).ToList();

                switch (option)
                {
                    case 1:
                        GenerateHammingCodeWord(sequence);
                        break;
                    case 2:
                        HammingCodeErrorDetection(sequence);
                        break;
                }

                Console.WriteLine("\nPress ESC to exit or any other key to continue");

                if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                {
                    break;
                }

                Console.WriteLine("\n");

            } while (true);
        }

        static void HammingCodeErrorDetection(List<int> sequence)
        {
            int numberOfParityBits = FindNumberOfParityBits(sequence.Count());
            int powerOfTwo = 1;
            int correctionIndex = Int32.MinValue;
            
            for (int i = 0; i < numberOfParityBits; i++, powerOfTwo += powerOfTwo)
            {
                var currentList = FindHammingSequence(powerOfTwo, sequence);
                Console.WriteLine($"p{powerOfTwo}: " + string.Join(" ", currentList));

                int bit = FindParityBit(currentList);

                if (bit != sequence[powerOfTwo - 1])
                {
                    correctionIndex = correctionIndex == Int32.MinValue ? powerOfTwo : correctionIndex += powerOfTwo;
                }
            }

            if (correctionIndex != Int32.MinValue)
            {
                Console.WriteLine($"Error found at bit number {correctionIndex}");
                sequence[correctionIndex - 1] = sequence[correctionIndex - 1] == 1 ? 0 : 1;
            }
            else
            {
                Console.WriteLine("No errors found");
            }

            string result = string.Join(" ", sequence);
            Console.WriteLine($"Hamming code corrected: {result}");
        }

        static void GenerateHammingCodeWord(List<int> sequence)
        {
            int paddingBits = FindNumberOfParityBits(sequence.Count(), true);
            int powerOfTwo = 1;

            for (int i = 0; i < paddingBits; i++)
            {
                sequence.Insert(powerOfTwo - 1, 0);
                powerOfTwo += powerOfTwo;
            }
            int numberOfParityBits = FindNumberOfParityBits(sequence.Count());
            powerOfTwo = 1;

            for (int i = 0; i < numberOfParityBits; i++, powerOfTwo += powerOfTwo)
            {
                var currentList = FindHammingSequence(powerOfTwo, sequence);
                Console.WriteLine($"p{powerOfTwo}: " + string.Join(" ", currentList));

                int bit = FindParityBit(currentList);
                sequence[powerOfTwo - 1] = bit;
            }

            string result = string.Join(" ", sequence);
            Console.WriteLine($"Generated Hamming Code: {result}");
        }

        static int FindNumberOfParityBits(int sequenceLength, bool pad = false)
        {
            if (sequenceLength <= 2)
            {
                return sequenceLength;
            }
            
            int i = 1;

            while (Math.Pow(2, i) < sequenceLength + i + 1)
            {
                i++;
            }

            // If words are generated the number of padding bits are returned
            return pad ? i : i - 1;
        }

        static List<int> FindHammingSequence(int powerOfTwo, List<int> sequence)
        {
            var currentSequenceList = new List<int>();
            for (int i = powerOfTwo - 1; i < sequence.Count; i += (2 * powerOfTwo))
            {
                for (int j = 0; j < powerOfTwo; j++)
                {
                    if ((j + i) >= sequence.Count) 
                    {
                        break;
                    }

                    currentSequenceList.Add(sequence[j + i]);
                }

                if (i + (2 * powerOfTwo) == sequence.Count)
                {
                    i--;
                }
            }

            if (currentSequenceList.Count > 2)
            {
                currentSequenceList.RemoveAt(0);
            }

            return currentSequenceList;
        }

        static int FindParityBit(List<int> sequence)
        {
            // Check whether the count of 1s is even or odd 
            return sequence.Where(x => x == 1).Count() % 2 == 0 ? 0 : 1;
        }
    }
}

// TEST CASES
// 0 0 1 0 1 1 1 works
// 0 1 1 0 1 1 0 works
// 1 0 0 1 1 1 1 1 0 1 0 1 1 0 1 works
// 0 1 0 1 1 0 now works
// 1 1 now works
// 1 0 1 0 1 finally works
// 1 0 1 1 0 1 1 res => 7 index