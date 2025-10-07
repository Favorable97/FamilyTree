using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyTree.Data.Models
{
    public record DBParameter
    {
        public string Name { get; init; }
        public object? Value { get; init; }

        public static DBParameter Create(string name, object? value) => new() { Name = name, Value = value ?? DBNull.Value };
    }
}
