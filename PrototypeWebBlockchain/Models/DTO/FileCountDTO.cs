using Nethereum.ABI.FunctionEncoding.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Web;

namespace PrototypeWebBlockchain.Models
{
    [FunctionOutput]
    public class FileCountDTO
    {
        [Parameter("uint256", "count", 1)]
        public int count { get; set; }

    }
}