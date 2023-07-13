using System.Text.Json.Serialization;

namespace TonLibDotNet.Types
{
    [TLSchema("exportedKey word_list:vector<secureString> = ExportedKey")]
    public class ExportedKey : TypeBase
    {
        [JsonConstructor]
        public ExportedKey(List<string> wordList)
        {
            WordList = wordList ?? throw new ArgumentNullException(nameof(wordList));
        }

        public ExportedKey(params string[] wordList)
        {
            WordList = wordList.ToList();
        }

        public List<string> WordList { get; set; }
    }
}
