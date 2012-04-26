namespace Simple.Web
{
    using System;
    using System.IO;
    using System.Linq;
    using ContentTypeHandling;

    [ContentTypes("application/x-www-form-urlencoded")]
    sealed class FormDeserializer : IContentTypeHandler
    {
        public object Read(StreamReader streamReader, Type inputType)
        {
            string text = streamReader.ReadToEnd();
            var pairs = text.Split('\n').Select(s => Tuple.Create<string, string>(s.Split('=')[0], Uri.UnescapeDataString(s.Split('=')[1])));
            var obj = Activator.CreateInstance(inputType);
            foreach (var pair in pairs)
            {
                var property = inputType.GetProperty(pair.Item1);
                if (property != null)
                {
                    property.SetValue(obj, Convert.ChangeType(pair.Item2, property.PropertyType), null);
                }
            }
            return obj;
        }

        public void Write(IContent content, TextWriter textWriter)
        {
            throw new NotImplementedException();
        }
    }
}