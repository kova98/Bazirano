namespace Bazirano.Models.Board
{
    /// <summary>
    /// The viewmodel class used for responding to a board post, i.e., adding a post to the thread.
    /// </summary>
    public class BoardRespondViewModel
    {
        /// <summary>
        /// The id of the thread which to respond to.
        /// </summary>
        public long ThreadId { get; set; }

        /// <summary>
        /// The board post to be posted as a response.
        /// </summary>
        public BoardPost BoardPost { get; set; }
    }
}
