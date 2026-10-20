namespace UP_Markov_Matveev
{
    
    public partial class Entities
    {
        private static Entities _context;

        
        public static Entities GetContext()
        {
            if (_context == null)
                _context = new Entities();
            return _context;
        }

        public static void ResetContext()
        {
            _context?.Dispose();
            _context = new Entities();
        }
    }
}
