using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Layer.Options
{
    public class ConnectionStrings
    {
        public static readonly string Default = Environment.GetEnvironmentVariable("ConnectionString") ?? "";
    }
}
