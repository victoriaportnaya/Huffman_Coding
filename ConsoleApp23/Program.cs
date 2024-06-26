﻿using Path = System.IO.Path;
// read text file 
// ceeate dict
// add frequency of the words to dict 

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;


// min heap for huffman code
public class MinHeap
{
    private Node[] elements;
    private int size;

    public MinHeap(int maxSize)
    {
        elements = new Node[maxSize];
    }
    private int GetLeftChildIndex(int elementIndex) => 2 * elementIndex + 1;
    private int GetRightChildIndex(int elementIndex) => 2 * elementIndex + 2;
    private int GetParentIndex(int elementIndex) => (elementIndex - 1) / 2;

    private bool HasLeftChild(int elementIndex) => GetLeftChildIndex(elementIndex) < size;
    private bool HasRightChild(int elementIndex) => GetRightChildIndex(elementIndex) < size;
    private bool IsRoot(int elementIndex) => elementIndex == 0;

    public int GetSize()
    {
        return size;
    }

    public bool IsEmpty() => size == 0;
    
    // methods
    public Node Peek()
    {
        if (size == 0)
        {
            throw new Exception("Nothing to peek!");

        }

        return elements[0];
    }

    public Node Pop()
    {
        if (size == 0)
        {
            throw new Exception("Nothing to pop!");

        }

        var root = elements[0];
        elements[0] = elements[--size];
        HeapifyDown();
        return root;
    }

    public void Add(Node element)
    {
        if (size == elements.Length)
            throw new Exception("Exceeded number of cells!");
        elements[size] = element;
        HeapifyUp(size++);


    }
    private void HeapifyDown() // when pop 
    {
        int index = 0;
        while (HasLeftChild(index))
        {
            var smallerChildIndex = GetLeftChildIndex(index);
            if (HasRightChild(index) && elements[GetRightChildIndex(index)].Freq < elements[smallerChildIndex].Freq)
            {
                smallerChildIndex = GetRightChildIndex(index);
            }

            if (elements[index].Freq <= elements[smallerChildIndex].Freq)
            {
                break; 
            }
            Swap(index, smallerChildIndex);
            index = smallerChildIndex;
        }
    }

    private void HeapifyUp(int index) // when add
    {
        while (!IsRoot(index) && elements[index].Freq < elements[GetParentIndex(index)].Freq)
        {
            Swap(index, GetParentIndex(index));
            index = GetParentIndex(index);
        }
    }

    private void Swap(int firstIndex, int secondIndex)
    {
        (elements[firstIndex], elements[secondIndex]) = (elements[secondIndex], elements[firstIndex]);
    }
}


// general version of the huffman coding
public class Node : IComparable<Node>
{   
    public char Char { get; set; } // node constructor
    public int Freq { get; set; }
    public Node Left { get; set; }
    public Node Right { get; set; }

    public int CompareTo(Node other) // compare frequencies of two nodes
    {
        return Freq.CompareTo(other.Freq);
    }
}

public class Huffman // huffman encoding
{
    public static Node BuildTree(string inputText) // build huffman tree 
    {
        Dictionary<char, int> freq = new Dictionary<char, int>(); // dict for char-freq
        foreach (char character in inputText) // calculate frequence -- better to replace with LINQ
        {
            if (freq.ContainsKey(character))
            {
                freq[character] += 1;
            }
            else
            {
                freq[character] = 1;
            }
        }

        MinHeap priorityQueue = new MinHeap(freq.Count);

        foreach (var node in freq.Select(entry => new Node { Char = entry.Key, Freq = entry.Value }))
        {
            priorityQueue.Add(node);
        }

        // build tree until only root 
        while (priorityQueue.GetSize() > 1)
        {
            Node left = priorityQueue.Pop();
            Node right = priorityQueue.Pop();

            Node merged = new Node { Char = '\0', Freq = left.Freq + right.Freq };
            merged.Left = left;
            merged.Right = right;

            priorityQueue.Add(merged);

        }

        return priorityQueue.Pop(); // get min
    }

    public static void AssignCode(Node node, string code, Dictionary<char, string> binaryCodes)
    {
        if (node == null) return;
        if (node.Char != '\0')
        {
            binaryCodes[node.Char] = code;
            return;
        }

        AssignCode(node.Left, code + "0", binaryCodes); // if left node - add 0
        AssignCode(node.Right, code + "1", binaryCodes); // if right node - add 1
    }

    public static Tuple<string, Dictionary<char, string>> Encode(string inputText)
    {
        Node root = BuildTree(inputText);
        Dictionary<char, string> binaryCodes = new Dictionary<char, string>();
        AssignCode(root, "", binaryCodes);

        string encodedString = string.Join("", inputText.Select(c => binaryCodes[c]));
        return new Tuple<string, Dictionary<char, string>>(encodedString, binaryCodes);
    }

    public static string ReadBinary(string filePath)
    {
        using (BinaryReader reader = new BinaryReader(File.Open(filePath, FileMode.Open)))
        {
            byte[] bytes = reader.ReadBytes((int)reader.BaseStream.Length);
            StringBuilder binaryString = new StringBuilder();
            foreach (byte b in bytes)
            {
                binaryString.Append(Convert.ToString(b, 2).PadLeft(8, '0'));

            }

            return binaryString.ToString();
        }
    }

    // decoder add
    public static string Decode(string encodedString, Dictionary<char, string> binaryCodes)
    {
        var reversedCodes = binaryCodes.ToDictionary(x => x.Value, x => x.Key);
        var currentCode = "";
        var decodedString = "";

        foreach (var bit in encodedString)
        {
            currentCode += bit;
            if (reversedCodes.ContainsKey(currentCode))
            {
                decodedString += reversedCodes[currentCode];
                currentCode = "";
            }

        }

        return decodedString;
    }

    public static string DecodeBinary(string encodedString, Dictionary<char, string> binaryCodes)
    {
        var decodedString = new StringBuilder();
        var currentCode = "";

        foreach (var bit in encodedString)
        {
            currentCode += bit;
            foreach (var kvp in binaryCodes)
            {
                if (kvp.Value == currentCode)
                {
                    decodedString.Append(kvp.Key);
                    currentCode = "";
                    break;
                }
            }
        }
        

        return decodedString.ToString();
    }

    public static void SaveTable(Dictionary<char, string> binaryCodes, string filePath)
    {
        using (BinaryWriter writer = new BinaryWriter(File.Open(filePath, FileMode.Create)))
        {
            foreach (var code in binaryCodes)
            {
                writer.Write(code.Key);
                writer.Write(code.Value.Length);
                writer.Write(code.Value);
            }
        }
    }

    public static Dictionary<char, string> ReadTable(string filePath)
    {
        var binaryCodes = new Dictionary<char, string>();
        using (BinaryReader reader = new BinaryReader(File.Open(filePath, FileMode.Open)))
        {
            while (reader.BaseStream.Position != reader.BaseStream.Length)
            {
                char character = reader.ReadChar();
                int length = reader.ReadInt32();
                string code = new string(reader.ReadChars(length));
                binaryCodes.Add(character, code);
            }
        }
        return binaryCodes;
    }

    public static void SaveEncoded(string encodedText, string filePath)
    {
        byte[] bytes = new byte[(encodedText.Length + 7) / 8];
        for (int i = 0; i < bytes.Length; i+= 8)
        {
            string eightBits = encodedText.Substring(i, Math.Min(8, encodedText.Length - i)).PadRight(8, '0');
            bytes[i / 8] = Convert.ToByte(eightBits, 2);
        }
        File.WriteAllBytes(filePath, bytes);
    }

    public static string ReadEncoded(string filePath)
    {
        byte[] bytes = File.ReadAllBytes(filePath);
        StringBuilder binaryString = new StringBuilder();
        foreach (byte b in bytes)
        {
            binaryString.Append(Convert.ToString(b, 2).PadLeft(8, '0'));
        }

        return binaryString.ToString();
    }
    

    public static void Main()
    {
        string myText = File.ReadAllText("C:\\Users\\victo\\RiderProjects\\ConsoleApp23\\ConsoleApp23\\textFile.txt");

        var result = Encode(myText);
        var encodedText = result.Item1;
        var binaryCodes = result.Item2;

        Console.WriteLine($"Encoded text: {encodedText}");
        Console.WriteLine("Huffman Code Table");
        foreach (var code in binaryCodes)
        {
            Console.WriteLine($"{code.Key} : {code.Value}");
        }
    
        string tableFilePath = "C:\\Users\\victo\\RiderProjects\\ConsoleApp23\\ConsoleApp23\\encoded.bin";
        SaveTable(binaryCodes, tableFilePath);

        string encodedFilePath = "C:\\Users\\victo\\RiderProjects\\ConsoleApp23\\ConsoleApp23\\encodedText.bin";
        SaveEncoded(encodedText, encodedFilePath);

        var myCodes = ReadTable(tableFilePath);
    
        string decodedEncodedText = ReadEncoded(encodedFilePath);
        string decodedText = DecodeBinary(decodedEncodedText, myCodes);

        string decodedFilePath = "C:\\Users\\victo\\RiderProjects\\ConsoleApp23\\ConsoleApp23\\decoded.txt";
        File.WriteAllText(decodedFilePath, decodedText);
        Console.WriteLine(decodedText);

    }
}
