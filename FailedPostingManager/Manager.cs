using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FailedPostingManager
{
    public class Manager
    {
        private Reader reader;
        private Sender sender;
        public Manager()
        {
            reader = new Reader();
            sender = new Sender(reader);

        }

        public void process()
        {
            var data = reader.get();
            sender.Send(data, reader);

        }
    }
}
