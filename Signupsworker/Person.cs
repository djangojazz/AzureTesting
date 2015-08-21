using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table.DataServices;
using Microsoft.WindowsAzure.Storage.Table;

namespace Signupsworker
{
    class PersonEntity : TableEntity
        //TableServiceEntity
    {
        public PersonEntity(){}

        public string Email { get; set; }
    }
}
