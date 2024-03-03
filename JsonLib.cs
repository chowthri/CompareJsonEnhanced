using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Text;

class Program
{
    static void Main()
    {
        // Example JSON strings
        string json1 = "{\"id\": 1, \"name\": \"John\", \"details\": {\"age\": 25, \"city\": \"New York\"}, \"tags\": [\"tag1\", \"tag2\"]}";
        string json2 = "{\"id\": 2, \"name\": \"Jane\", \"details\": {\"age\": 25, \"city\": \"Los Angeles\"}, \"tags\": [\"tag1\", \"tag3\"]}";

        // Deserialize JSON strings into JToken
        JToken token1 = JToken.Parse(json1);
        JToken token2 = JToken.Parse(json2);

        // Compare and generate List<DiffResult>
        List<DiffResult> diffResults = CompareTokens(token1, token2);

        // Generate HTML table from List<DiffResult>
        string htmlTable = GenerateHtmlTable(diffResults);

        Console.WriteLine(htmlTable);

        Console.ReadLine();
    }

    static List<DiffResult> CompareTokens(JToken token1, JToken token2, string path = "")
    {
        List<DiffResult> differences = new List<DiffResult>();

        if (JToken.DeepEquals(token1, token2))
        {
            return differences;
        }

        switch (token1.Type)
        {
            case JTokenType.Object:
                var obj1 = (JObject)token1;
                var obj2 = (JObject)token2;

                foreach (var property in obj1.Properties())
                {
                    var propertyPath = path + "." + property.Name;
                    if (obj2.TryGetValue(property.Name, out var value2))
                    {
                        differences.AddRange(CompareTokens(property.Value, value2, propertyPath));
                    }
                    else
                    {
                        differences.Add(new DiffResult(propertyPath, property.Value, null));
                    }
                }

                foreach (var property in obj2.Properties())
                {
                    if (!obj1.ContainsKey(property.Name))
                    {
                        differences.Add(new DiffResult(path + "." + property.Name, null, property.Value));
                    }
                }

                break;

            case JTokenType.Array:
                var array1 = (JArray)token1;
                var array2 = (JArray)token2;

                for (int i = 0; i < Math.Max(array1.Count, array2.Count); i++)
                {
                    var elementPath = path + $"[{i}]";
                    if (i < array1.Count && i < array2.Count)
                    {
                        differences.AddRange(CompareTokens(array1[i], array2[i], elementPath));
                    }
                    else if (i < array1.Count)
                    {
                        differences.Add(new DiffResult(elementPath, array1[i], null));
                    }
                    else
                    {
                        differences.Add(new DiffResult(elementPath, null, array2[i]));
                    }
                }

                break;

            default:
                differences.Add(new DiffResult(path, token1, token2));
                break;
        }

        return differences;
    }

    static string GenerateHtmlTable(List<DiffResult> diffResults)
    {
        StringBuilder html = new StringBuilder();

        html.AppendLine("<table border='1'>");
        html.AppendLine("<tr><th>Property</th><th>Value in JSON 1</th><th>Value in JSON 2</th></tr>");

        foreach (var diff in diffResults)
        {
            html.AppendLine($"<tr><td>{diff.Property}</td><td>{FormatValue(diff.Value1)}</td><td>{FormatValue(diff.Value2)}</td></tr>");
        }

        html.AppendLine("</table>");

        return html.ToString();
    }

    static string FormatValue(object value)
    {
        return value?.ToString() ?? "null";
    }
}

class DiffResult
{
    public string Property { get; }
    public object Value1 { get; }
    public object Value2 { get; }

    public DiffResult(string property, object value1, object value2)
    {
        Property = property;
        Value1 = value1;
        Value2 = value2;
    }
}
//////////////////////////////////////


using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Text;

class CompareJsonEhancedV2
{
    static void Main()
    {
        // Example JSON strings
        string json1 = "{\"id\": 1, \"name\": \"John\", \"details\": {\"age\": 25, \"city\": \"New York\"}, \"tags\": [\"tag1\", \"tag2\"]}";
        string json2 = "{\"id\": 2, \"name\": \"Jane\", \"details\": {\"age\": 25, \"city\": \"Los Angeles\"}, \"tags\": [\"tag1\", \"tag3\"]}";

        // Deserialize JSON strings into JToken
        JToken token1 = JToken.Parse(json1);
        JToken token2 = JToken.Parse(json2);

        // Compare and generate List<DiffResult>
        List<DiffResult> diffResults = CompareTokens(token1, token2);

        // Generate HTML table from List<DiffResult>
        string htmlTable = GenerateHtmlTable(diffResults);

        Console.WriteLine(htmlTable);

        Console.ReadLine();
    }

    static List<DiffResult> CompareTokens(JToken token1, JToken token2, string path = "")
    {
        List<DiffResult> differences = new List<DiffResult>();

        if (JToken.DeepEquals(token1, token2))
        {
            differences.Add(new DiffResult(path, token1, token2, true));
            return differences;
        }

        switch (token1.Type)
        {
            case JTokenType.Object:
                var obj1 = (JObject)token1;
                var obj2 = (JObject)token2;

                foreach (var property in obj1.Properties())
                {
                    var propertyPath = path + "." + property.Name;
                    if (obj2.TryGetValue(property.Name, out var value2))
                    {
                        differences.AddRange(CompareTokens(property.Value, value2, propertyPath));
                    }
                    else
                    {
                        differences.Add(new DiffResult(propertyPath, property.Value, null, false));
                    }
                }

                foreach (var property in obj2.Properties())
                {
                    if (!obj1.ContainsKey(property.Name))
                    {
                        differences.Add(new DiffResult(path + "." + property.Name, null, property.Value, false));
                    }
                }

                break;

            case JTokenType.Array:
                var array1 = (JArray)token1;
                var array2 = (JArray)token2;

                for (int i = 0; i < Math.Max(array1.Count, array2.Count); i++)
                {
                    var elementPath = path + $"[{i}]";
                    if (i < array1.Count && i < array2.Count)
                    {
                        differences.AddRange(CompareTokens(array1[i], array2[i], elementPath));
                    }
                    else if (i < array1.Count)
                    {
                        differences.Add(new DiffResult(elementPath, array1[i], null, false));
                    }
                    else
                    {
                        differences.Add(new DiffResult(elementPath, null, array2[i], false));
                    }
                }

                break;

            default:
                differences.Add(new DiffResult(path, token1, token2, false));
                break;
        }

        return differences;
    }

    static string GenerateHtmlTable(List<DiffResult> diffResults)
    {
        StringBuilder html = new StringBuilder();

        html.AppendLine("<table border='1'>");
        html.AppendLine("<tr><th>Property</th><th>Value in JSON 1</th><th>Value in JSON 2</th><th>Difference</th></tr>");

        foreach (var diff in diffResults)
        {
            html.AppendLine($"<tr><td>{diff.Property}</td><td>{FormatValue(diff.Value1)}</td><td>{FormatValue(diff.Value2)}</td><td>{diff.Match}</td></tr>");
        }

        html.AppendLine("</table>");

        return html.ToString();
    }

    static string FormatValue(object value)
    {
        return value?.ToString() ?? "null";
    }
}

class DiffResult
{
    public string Property { get; }
    public object Value1 { get; }
    public object Value2 { get; }
    public bool Match { get; }

    public DiffResult(string property, object value1, object value2, bool match)
    {
        Property = property;
        Value1 = value1;
        Value2 = value2;
        Match = match;
    }
}

////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Text;

class CompareJsonV3
{
    static void Main()
    {
        // Example JSON strings
        string json1 = "{\"id\": 1, \"name\": \"John\", \"details\": {\"age\": 25, \"city\": \"New York\"}, \"tags\": [\"tag1\", \"tag2\"]}";
        string json2 = "{}";  // Empty JSON

        // Deserialize JSON strings into JToken
        JToken token1 = JToken.Parse(json1);
        JToken token2 = JToken.Parse(json2);

        // Compare and generate List<DiffResult>
        List<DiffResult> diffResults = CompareTokens(token1, token2);

        // Generate HTML table from List<DiffResult>
        string htmlTable = GenerateHtmlTable(diffResults);

        Console.WriteLine(htmlTable);

        Console.ReadLine();
    }

    static List<DiffResult> CompareTokens(JToken token1, JToken token2, string path = "")
    {
        List<DiffResult> differences = new List<DiffResult>();

        if (token1 == null && token2 == null)
        {
            return differences;  // Both tokens are null, nothing to compare
        }

        if (token1 == null || token2 == null)
        {
            // One token is null, treat it as a difference
            differences.Add(new DiffResult(path, token1, token2, false));
            return differences;
        }

        if (JToken.DeepEquals(token1, token2))
        {
            differences.Add(new DiffResult(path, token1, token2, true));
            return differences;
        }

        switch (token1.Type)
        {
            case JTokenType.Object:
                var obj1 = (JObject)token1;
                var obj2 = (JObject)token2;

                foreach (var property in obj1.Properties())
                {
                    var propertyPath = path + "." + property.Name;
                    if (obj2.TryGetValue(property.Name, out var value2))
                    {
                        differences.AddRange(CompareTokens(property.Value, value2, propertyPath));
                    }
                    else
                    {
                        differences.Add(new DiffResult(propertyPath, property.Value, null, false));
                    }
                }

                foreach (var property in obj2.Properties())
                {
                    if (!obj1.ContainsKey(property.Name))
                    {
                        differences.Add(new DiffResult(path + "." + property.Name, null, property.Value, false));
                    }
                }

                break;

            case JTokenType.Array:
                var array1 = (JArray)token1;
                var array2 = (JArray)token2;

                for (int i = 0; i < Math.Max(array1.Count, array2.Count); i++)
                {
                    var elementPath = path + $"[{i}]";
                    if (i < array1.Count && i < array2.Count)
                    {
                        differences.AddRange(CompareTokens(array1[i], array2[i], elementPath));
                    }
                    else if (i < array1.Count)
                    {
                        differences.Add(new DiffResult(elementPath, array1[i], null, false));
                    }
                    else
                    {
                        differences.Add(new DiffResult(elementPath, null, array2[i], false));
                    }
                }

                break;

            default:
                differences.Add(new DiffResult(path, token1, token2, false));
                break;
        }

        return differences;
    }

    static string GenerateHtmlTable(List<DiffResult> diffResults)
    {
        StringBuilder html = new StringBuilder();

        html.AppendLine("<table border='1'>");
        html.AppendLine("<tr><th>Property</th><th>Value in JSON 1</th><th>Value in JSON 2</th><th>Difference</th></tr>");

        foreach (var diff in diffResults)
        {
            html.AppendLine($"<tr><td>{diff.Property}</td><td>{FormatValue(diff.Value1)}</td><td>{FormatValue(diff.Value2)}</td><td>{diff.Match}</td></tr>");
        }

        html.AppendLine("</table>");

        return html.ToString();
    }

    static string FormatValue(object value)
    {
        return value?.ToString() ?? "null";
    }
}

class DiffResult
{
    public string Property { get; }
    public object Value1 { get; }
    public object Value2 { get; }
    public bool Match { get; }

    public DiffResult(string property, object value1, object value2, bool match)
    {
        Property = property;
        Value1 = value1;
        Value2 = value2;
        Match = match;
    }
}

////////////////////////////////////////////////////////////
static List<DiffResult> CompareTokensV2(JToken token1, JToken token2, string path = "")
{
    List<DiffResult> differences = new List<DiffResult>();

    if (token1 == null && token2 == null)
    {
        return differences;  // Both tokens are null, nothing to compare
    }

    if (token1 == null || token2 == null)
    {
        // One token is null, treat it as a difference
        differences.Add(new DiffResult(path, token1, token2, false));
        return differences;
    }

    if (JToken.DeepEquals(token1, token2))
    {
        differences.Add(new DiffResult(path, token1, token2, true));
        return differences;
    }

    switch (token1.Type)
    {
        case JTokenType.Object:
            var obj1 = (JObject)token1;
            var obj2 = (JObject)token2;

            foreach (var property in obj1.Properties())
            {
                var propertyPath = path + "." + property.Name;
                if (obj2.TryGetValue(property.Name, out var value2))
                {
                    differences.AddRange(CompareTokens(property.Value, value2, propertyPath));
                }
                else
                {
                    differences.Add(new DiffResult(propertyPath, property.Value, null, false));
                }
            }

            foreach (var property in obj2.Properties())
            {
                if (!obj1.ContainsKey(property.Name))
                {
                    differences.Add(new DiffResult(path + "." + property.Name, null, property.Value, false));
                }
            }

            break;

        case JTokenType.Array:
            var array1 = (JArray)token1;
            var array2 = (JArray)token2;

            for (int i = 0; i < Math.Max(array1.Count, array2.Count); i++)
            {
                var elementPath = path + $"[{i}]";
                if (i < array1.Count && i < array2.Count)
                {
                    differences.AddRange(CompareTokens(array1[i], array2[i], elementPath));
                }
                else if (i < array1.Count)
                {
                    differences.Add(new DiffResult(elementPath, array1[i], null, false));
                }
                else
                {
                    differences.Add(new DiffResult(elementPath, null, array2[i], false));
                }
            }

            break;

        default:
            differences.Add(new DiffResult(path, token1, token2, false));
            break;
    }

    return differences;
}
////////////////////////////////////////

static List<DiffResult> CompareTokensV3(JToken token1, JToken token2, string path = "")
{
    List<DiffResult> differences = new List<DiffResult>();

    if (token1 == null && token2 == null)
    {
        return differences;  // Both tokens are null, nothing to compare
    }

    if (token1 == null || token2 == null)
    {
        // One token is null, treat it as a difference
        differences.Add(new DiffResult(path, token1, token2, false));
        return differences;
    }

    if (JToken.DeepEquals(token1, token2))
    {
        differences.Add(new DiffResult(path, token1, token2, true));
        return differences;
    }

    switch (token1.Type)
    {
        case JTokenType.Object:
            var obj1 = (JObject)token1;
            var obj2 = (JObject)token2;

            foreach (var property in obj1.Properties())
            {
                var propertyPath = path + "." + property.Name;
                if (obj2.TryGetValue(property.Name, out var value2))
                {
                    differences.AddRange(CompareTokens(property.Value, value2, propertyPath));
                }
                else
                {
                    differences.Add(new DiffResult(propertyPath, property.Value, null, false));
                }
            }

            foreach (var property in obj2.Properties())
            {
                if (!obj1.ContainsKey(property.Name))
                {
                    differences.Add(new DiffResult(path + "." + property.Name, null, property.Value, false));
                }
            }

            break;

        case JTokenType.Array:
            var array1 = (JArray)token1;
            var array2 = (JArray)token2;

            for (int i = 0; i < Math.Max(array1.Count, array2.Count); i++)
            {
                var elementPath = path + $"[{i}]";
                if (i < array1.Count && i < array2.Count)
                {
                    differences.AddRange(CompareTokens(array1[i], array2[i], elementPath));
                }
                else if (i < array1.Count)
                {
                    differences.Add(new DiffResult(elementPath, array1[i], null, false));
                }
                else
                {
                    differences.Add(new DiffResult(elementPath, null, array2[i], false));
                }
            }

            break;

        default:
            differences.Add(new DiffResult(path, token1, token2, false));
            break;
    }

    return differences;
}

