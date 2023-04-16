using System;
using LenLib.Standard.Core.Search;
using Newtonsoft.Json;

namespace LenLib.Api.Dto.V1
{
    public class TgSearchRequest
    {
        /// <summary>
        /// Идентификатор корпуса, например 'lenin-v2'
        /// </summary>
        public string CorpusId { get; set; }

        /// <summary>
        /// Поисковый запрос, например 'диктатура пролетариата'
        /// </summary>
        public string Query { get; set; }

        /// <summary>
        /// Искать по тексте = 0, по заголовку = 1
        /// </summary>
        public SearchMode Mode { get; set; }

        /// <summary>
        /// Страница - 0, 1, 2 ...
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// Размер страницы, например 100
        /// </summary>
        public int PageSize { get; set; }

        [JsonIgnore]
        public string[] Tokens => Query?.Split(' ', StringSplitOptions.RemoveEmptyEntries);
    }
}