using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FirewallCtlUI.DB
{
    public class Parameter
    {
        [Key] public string Id { get; set; } = Guid.NewGuid().ToString();
        public DateTime Modified { get; set; } = DateTime.UtcNow;
        public string Name { get; set; }
        public string Value { get; set; }
    }
}

