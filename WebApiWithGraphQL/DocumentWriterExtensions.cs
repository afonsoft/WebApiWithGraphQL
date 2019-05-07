using System.IO;
using System.Text;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Http;

namespace WebApiWithGraphQL
{
    public static class DocumentWriterExtensions
    {
        /// <summary>
        /// Writes the <paramref name="value"/> to string.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static async Task<string> WriteToStringAsync(
            this IDocumentWriter writer,
            ExecutionResult value)
        {
            using (var stream = new MemoryStream())
            {
                await writer.WriteAsync(stream, value);
                stream.Position = 0;
                using (var reader = new StreamReader(stream, new UTF8Encoding(false)))
                {
                    return await reader.ReadToEndAsync();
                }
            }
        }
    }
}
