namespace TokenCreator
{
    public class Word
    {
        public string Value
        {
            get => value;

            set
            {
                if (value.Trim().Contains(' ') || value.Contains('\t') || value.Contains('\n'))
                throw new ArgumentException("Error", value);
            }
        }

        public Word() { }
        public Word(string value) => Value = value;

        public int Length => Value?.Length ?? 0;

        public bool StartsWithConsonant()
        {
            if (string.IsNullOrEmpty(Value)) return false;
            char first = char.ToLower(Value[0]);
            return "бвгджзйклмнпрстфхцчшщbcdfghjklmnpqrstvwxyz".Contains(first);
        }

        public bool IsAWord(string raw)
        {
            int counter = 0;
            Word w = new();
            var matches = Regex.Matches(raw, @"\w+|[.!?]");
            foreach (Match m in matches)
            {
                if (Regex.IsMatch(m.Value, @"\w+"))
                {
                    counter++;
                }

            }
            if (counter > 1)
                {
                    return false;
                }
            return true;
        }
        public override string ToString() => Value;
    }
}
