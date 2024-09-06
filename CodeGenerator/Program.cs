using System;
using System.IO;
using System.Reflection;
using TypeLitePlus;

namespace CodeGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            Assembly dto = typeof(FirewallCtlUI.DTO.Settings).Assembly;
            var x = TypeScript.Definitions();
            foreach (Type type in dto.GetTypes())
            {
                x.For(type);
            }
            x.WithMemberFormatter((identifier) => Char.ToLower(identifier.Name[0]) + identifier.Name.Substring(1));
            File.WriteAllText(args[0], x.Generate());
        }
    }
}
