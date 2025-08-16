using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FintrakBanking.Common
{
    class CustomResponse
    {
        public object data { get; set; }
        public string message { get; set; }
        public bool translate { get; set; }
        public bool status { get; set; }

    }
}
