namespace TonLibDotNet.Types
{
    [TLSchema("exportedKey word_list:vector<secureString> = ExportedKey")]
    public class ExportedKey : TypeBase
    {
        public ExportedKey(List<string> wordList)
        {
            WordList = wordList ?? throw new ArgumentNullException(nameof(wordList));
        }

        public List<string> WordList { get; set; }
    }
}
