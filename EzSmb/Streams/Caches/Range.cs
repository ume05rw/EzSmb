namespace EzSmb.Streams.Caches
{
    public class Range
    {
        public long Start { get; internal set; }
        public long End { get; internal set; }

        public long Position => this.Start;
        public long Count => (this.End - this.Start + 1);

        public Range(long start, long end)
        {
            if (start <= end)
            {
                this.Start = start;
                this.End = end;
            }
            else
            {
                this.Start = end;
                this.End = start;
            }
        }
    }
}
