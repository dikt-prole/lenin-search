using BookProject.Core.Models.Domain;

namespace BookProject.Core.Models.ViewModel
{
    public class BlockEventArgs
    {
        public BlockEventArgs(Page page, Block block)
        {
            Block = block;
            Page = page;
        }

        public Block Block { get; private set; }
        public Page Page { get; private set; }
    }
}