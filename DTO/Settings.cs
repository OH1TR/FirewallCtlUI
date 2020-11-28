using System;
using System.Collections.Generic;

namespace FirewallCtlUI.DTO
{
    public class Settings
    {
        public string Device { get; set; }

        public List<string> MyNetworks { get; set; } = new List<string>();
    }
}
