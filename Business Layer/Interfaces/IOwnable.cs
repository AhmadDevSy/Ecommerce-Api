using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Layer.Interfaces
{
    public interface IOwnable
    {
        public int UserId { get; init; }
    }
}
