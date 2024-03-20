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