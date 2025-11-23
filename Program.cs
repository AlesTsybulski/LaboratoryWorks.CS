
using System.Text;


struct GeneticData
{
    public string protein;
    public string organism;
    public string amino_acids;
}

class Program
{
    static string RLDecoding(string amino_acids)
    {
        if (string.IsNullOrEmpty(amino_acids)) return "";
        StringBuilder sb = new StringBuilder();
        int i = 0;
        while (i < amino_acids.Length)
        {
            if (char.IsDigit(amino_acids[i]))
            {
                int num_start = i;
                while (i < amino_acids.Length && char.IsDigit(amino_acids[i]))
                {
                    i++;
                }
                int count = int.Parse(amino_acids.Substring(num_start, i - num_start));

                if (i < amino_acids.Length && !char.IsDigit(amino_acids[i]))
                {
                    char c = amino_acids[i];
                    for (int j = 0; j < count; j++)
                    {
                        sb.Append(c);
                    }
                    i++;
                }
            }
            else
            {
                sb.Append(amino_acids[i]);
                i++;
            }
        }
        return sb.ToString();
    }

    static string RLEncoding(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return "";
        }

        StringBuilder result = new StringBuilder();
        int i = 0;
        while (i < text.Length)
        {
            char c = text[i];
            int count = 1;
            int j = i + 1;
            while (j < text.Length && text[j] == c)
            {
                count++;
                j++;
            }

            if (count > 2)
            {
                result.Append(count);
            }
            result.Append(c);
            i = j;
        }

        return result.ToString();
    }

    static void Search(StreamWriter sw, List<GeneticData> data, string seq)
    {
        sw.WriteLine("organism				protein");
        bool found = false;
        foreach (var gd in data)
        {
            if (gd.amino_acids.Contains(seq))
            {
                sw.WriteLine(gd.organism + "		" + gd.protein);
                found = true;
            }
        }
        if (!found)
        {
            sw.WriteLine("NOT FOUND");
        }
    }

    static void Diff(StreamWriter sw, Dictionary<string, string> proteinToAcids, string p1, string p2)
    {
        proteinToAcids.TryGetValue(p1, out string? s1);
        proteinToAcids.TryGetValue(p2, out string? s2);

        sw.WriteLine("amino-acids difference:");
        if (s1 == null || s2 == null)
        {
            string missing = "MISSING:";
            if (s1 == null) missing += " " + p1;
            if (s2 == null) missing += " " + p2;
            sw.WriteLine(missing.Trim());
        }
        else
        {
            int len1 = s1.Length;
            int len2 = s2.Length;
            int minLen = Math.Min(len1, len2);
            int matches = 0;
            for (int i = 0; i < minLen; i++)
            {
                if (s1[i] == s2[i]) matches++;
            }
            int maxLen = Math.Max(len1, len2);
            int difference = maxLen - matches;
            sw.WriteLine(difference);
        }
    }

    static void Mode(StreamWriter sw, Dictionary<string, string> proteinToAcids, string p)
    {
        sw.WriteLine("amino-acid occurs:");
        if (proteinToAcids.TryGetValue(p, out string? s) && s != null)
        {
            if (string.IsNullOrEmpty(s))
            {
                sw.WriteLine(' ' + "          " + 0);
                return;
            }

            var counts = s.GroupBy(c => c).ToDictionary(g => g.Key, g => g.Count());
            int maxCount = counts.Values.Max();
            char maxChar = counts.Where(kv => kv.Value == maxCount).Min(kv => kv.Key);

            sw.WriteLine(maxChar + "          " + maxCount);
        }
        else
        {
            sw.WriteLine("MISSING: " + p);
        }
    }

    static void Main(string[] args)
    {
        List<GeneticData> data = new List<GeneticData>();
        using (StreamReader sr = new StreamReader("sequences.0.txt"))
        {
            string? line;
            while ((line = sr.ReadLine()) != null)
            {
                string[] parts = line.Split('	');
                if (parts.Length == 3)
                {
                    GeneticData gd;
                    gd.protein = parts[0];
                    gd.organism = parts[1];
                    gd.amino_acids = RLDecoding(parts[2]);
                    data.Add(gd);
                }
            }
        }
        
        var proteinToAcids = data.ToDictionary(gd => gd.protein, gd => gd.amino_acids);

        using (StreamWriter sw = new StreamWriter("genedata.0.txt"))
        {
            sw.WriteLine("Ales Tsybulski");
            sw.WriteLine("Genetic Searching");
            sw.WriteLine(new string('-', 74));

            int opNum = 1;
            using (StreamReader srCmd = new StreamReader("commands.0.txt"))
            {
                string? line;
                while ((line = srCmd.ReadLine()) != null)
                {
                    string[] parts = line.Split('	');
                    if (parts.Length < 1) continue;
                    string cmd = parts[0];

                    string opLine = string.Format("{0:000}   {1}", opNum, cmd);
                    if (parts.Length > 1)
                    {
                        string param1 = (cmd == "search") ? RLDecoding(parts[1]) : parts[1];
                        opLine += "   " + param1;
                    }
                    if (parts.Length > 2)
                    {
                        opLine += "   " + parts[2];
                    }
                    sw.WriteLine(opLine);
                    opNum++;

                    switch (cmd)
                    {
                        case "search":
                            if (parts.Length < 2) continue;
                            string seq = RLDecoding(parts[1]);
                            Search(sw, data, seq);
                            break;
                        case "diff":
                            if (parts.Length < 3) continue;
                            Diff(sw, proteinToAcids, parts[1], parts[2]);
                            break;
                        case "mode":
                            if (parts.Length < 2) continue;
                            Mode(sw, proteinToAcids, parts[1]);
                            break;
                    }

                    sw.WriteLine(new string('-', 74));
                }
            }
        }
    }
}
