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

        public async Task<bool> CanConnect()
        {
            try
            {
                return await _context.Database.CanConnectAsync();
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public async Task EnsureCreated()
        {
            await _context.Database.EnsureCreatedAsync();
        }
    }
}
