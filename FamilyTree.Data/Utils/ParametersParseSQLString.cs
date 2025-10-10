using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FamilyTree.Data.Utils
{
    public static class ParametersParseSQLString
    {
        public static DBParameter[] GetParamsFromCommand(string command, Person person)
        {
            List<DBParameter> paramList = [];

            var pattern = @"@\w*";
            var regex = new Regex(pattern);

            var matches = regex.Matches(command);

            if (matches.Count > 0)
            {
                foreach (Match match in matches)
                {
                    paramList.Add(DBParameter.Create(match.Value, MatchParams(match.Value.Replace("@", ""), person)));
                }
            }

            return [.. paramList];
        }

        private static object? MatchParams(string propName, object parameter)
        {
            if (!parameter.GetType().IsClass)
            {
                return parameter;
            }

            foreach (var param in parameter.GetType().GetProperties())
            {
                if (param.Name.Equals(propName, StringComparison.OrdinalIgnoreCase))
                {
                    var result = param.GetValue(parameter);

                    return result;
                }
            }
            
            return null;
        }
    }
}
