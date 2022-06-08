// See https://aka.ms/new-console-template for more information
using System;
// using test;
using System.IO;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using Microsoft.VisualBasic.FileIO;

class counter
{
    public int number = 0;

    public void increment(int n)
    {
        number += n;
        Console.WriteLine("Number equals: " + number);
    }
}

class Program
{
    public static void Main(string[] args)
    {
        // using(var reader = new StreamReader(@"C:\Users\Rhino\RiderProjects\TestExecutable\ConsoleApp1\ShapeList2.csv"))
        using(var reader = new StreamReader(@"ShapeList2.csv"))
        {
            List<string> listA = new List<string>();
            List<string> listB = new List<string>();
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var values = line.Split(',');

                break;
                listA.Add(values[0]);
                listB.Add(values[1]);
            }
        }
    }
    
}
