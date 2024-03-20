\
// read text file 
// ceeate dict
// add frequency of the words to dict 

using System.Net.Mime;
// create dict 
class Program
{
    static void Main()
    {
        string myText = File.ReadAllText("textFile.txt");
        Dictionary<char, int> charFrequency = myText
            .GroupBy(c => c)
            .ToDictionary(g => g.Key, g => g.Count());
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

        List<Node> priorityQueue = freq.Select(entry => new Node { Char = entry.Key, Freq = entry.Value }).ToList(); // create priority queue
        priorityQueue.Sort();
    }
}