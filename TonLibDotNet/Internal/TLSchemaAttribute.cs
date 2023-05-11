namespace TonLibDotNet.Internal
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class TLSchemaAttribute : Attribute
    {
        public TLSchemaAttribute(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException(nameof(value));
            }

            Value = value;
        }

        public string Value { get; set; }

        public string GetTLName()
        {
            return Value.Split(' ', 2)[0];
        }
    }
}
