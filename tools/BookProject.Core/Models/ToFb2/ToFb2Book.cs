using System.Collections.Generic;
using BookProject.Core.Utilities;

namespace BookProject.Core.Models.ToFb2
{
    public class ToFb2Book
    {
        public ToFb2Book()
        {
            Pages = new List<ToFb2Page>();
        }

        public List<ToFb2Page> Pages { get; set; }

        public static ToFb2Book Construct(Domain.Book domainBook)
        {
            // todo: validate domain book

            var toFb2Book = new ToFb2Book();

            ToFb2Helper.TransferOverlappingParagraphs(domainBook);

            return toFb2Book;
        }
    }
}