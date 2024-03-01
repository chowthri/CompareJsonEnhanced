using System;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

class clsJsonCompareVersion1
{
    static void Main()
    {
        string json1 = "{\"person\":{\"name\":\"John\",\"age\":30,\"city\":\"New York\",\"skills\":[\"C#\",\"JavaScript\"]}}";
        string json2 = "{\"person\":{\"name\":\"Jane\",\"age\":30,\"city\":\"Los Angeles\",\"skills\":[\"JavaScript\",\"Python\"]}}";

        List<DiffResult> differences = CompareJson(json1, json2);

        foreach (var diff in differences)
        {
            Console.WriteLine($"Field: {diff.FieldName}, Value1: {diff.Value1}, Value2: {diff.Value2}");
        }
    }

    static List<DiffResult> CompareJson(string json1, string json2)
    {
        JToken obj1 = JToken.Parse(json1);
        JToken obj2 = JToken.Parse(json2);

        return CompareJsonObjects(obj1, obj2);
    }

    static List<DiffResult> CompareJsonObjects(JToken obj1, JToken obj2, string currentPath = "")
    {
        List<DiffResult> differences = new List<DiffResult>();

        if (obj1 is JObject && obj2 is JObject)
        {
            var jObj1 = (JObject)obj1;
            var jObj2 = (JObject)obj2;

            foreach (var property1 in jObj1.Properties())
            {
                string fieldName = currentPath + property1.Name;
                JToken value1 = property1.Value;
                JToken value2 = jObj2[property1.Name];

                if (value2 == null)
                {
                    differences.Add(new DiffResult(fieldName, FormatValue(value1), "null"));
                }
                else if (value1.Type == JTokenType.Object && value2.Type == JTokenType.Object)
                {
                    differences.AddRange(CompareJsonObjects(value1, value2, fieldName + "."));
                }
                else if (value1.Type == JTokenType.Array && value2.Type == JTokenType.Array)
                {
                    differences.AddRange(CompareJsonArrays((JArray)value1, (JArray)value2, fieldName + "."));
                }
                else if (!JToken.DeepEquals(value1, value2))
                {
                    differences.Add(new DiffResult(fieldName, FormatValue(value1), FormatValue(value2)));
                }
            }

            foreach (var property2 in jObj2.Properties())
            {
                if (jObj1[property2.Name] == null)
                {
                    string fieldName = currentPath + property2.Name;
                    differences.Add(new DiffResult(fieldName, "null", FormatValue(property2.Value)));
                }
            }
        }

        return differences;
    }

    static List<DiffResult> CompareJsonArrays(JArray array1, JArray array2, string currentPath)
    {
        List<DiffResult> differences = new List<DiffResult>();

        for (int i = 0; i < Math.Max(array1.Count, array2.Count); i++)
        {
            JToken value1 = i < array1.Count ? array1[i] : null;
            JToken value2 = i < array2.Count ? array2[i] : null;

            if (value1 == null)
            {
                differences.Add(new DiffResult($"{currentPath}[{i}]", "null", FormatValue(value2)));
            }
            else if (value2 == null)
            {
                differences.Add(new DiffResult($"{currentPath}[{i}]", FormatValue(value1), "null"));
            }
            else if (value1.Type == JTokenType.Object && value2.Type == JTokenType.Object)
            {
                differences.AddRange(CompareJsonObjects(value1, value2, $"{currentPath}[{i}]."));
            }
            else if (value1.Type == JTokenType.Array && value2.Type == JTokenType.Array)
            {
                differences.AddRange(CompareJsonArrays((JArray)value1, (JArray)value2, $"{currentPath}[{i}]."));
            }
            else if (!JToken.DeepEquals(value1, value2))
            {
                differences.Add(new DiffResult($"{currentPath}[{i}]", FormatValue(value1), FormatValue(value2)));
            }
        }

        return differences;
    }

    static string FormatValue(JToken value)
    {
        return (value == null || value.Type == JTokenType.Null) ? "null" : value.ToString();
    }
}

class DiffResult
{
    public string FieldName { get; }
    public string Value1 { get; }
    public string Value2 { get; }

    public DiffResult(string fieldName, string value1, string value2)
    {
        FieldName = fieldName;
        Value1 = value1;
        Value2 = value2;
    }
}
