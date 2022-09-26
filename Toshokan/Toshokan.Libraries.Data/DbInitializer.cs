using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toshokan.Libraries.Data
{
    public class DbInitialiser
    {
        private readonly Context _context;

        public DbInitialiser(Context context)
        {
            _context = context;
        }

        public void Run()
        {
            //_context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
        }
    }
}
