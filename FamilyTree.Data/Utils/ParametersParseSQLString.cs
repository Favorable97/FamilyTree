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
        private static readonly Regex regex = new(@"@\w*", RegexOptions.Compiled);
        public static DBParameter[] GetParamsFromCommand<T>(string command, T data)
        {
            List<DBParameter> paramList = [];

            var matches = regex.Matches(command);

            if (matches.Count > 0)
            {
                foreach (Match match in matches)
                {
                    paramList.Add(DBParameter.Create(match.Value, MatchParams(match.Value.Replace("@", ""), data!)));
                }
            }

            return [.. paramList.DistinctBy(x => x.Name)];
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
