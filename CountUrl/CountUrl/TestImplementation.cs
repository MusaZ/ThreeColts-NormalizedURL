using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;

namespace UnitTestProject
{  
  public class TestImplementation
  {
    private static void Main(string[] args)
    {
      WriteLine(CountUniqueUrls(
        new string[]{
          "http://example.com?a=1&b=2", "https://example.com?b=2&a=1"
        }
      ));

      foreach(var keyValuePair in CountUniqueUrlsPerTopLevelDomain(new string[]{
        "https://example.com", "https://subdomain.example.com"}))
      {
        Write("{"+$"\"{keyValuePair.Key}\", {keyValuePair.Value}" + "}, ");
      }
      WriteLine();
      foreach (var keyValuePair in CountUniqueUrlsPerTopLevelDomain(new string[]{
        "https://test.example.com", "https://test.test.com"
      }))
      {
        Write("{" + $"\"{keyValuePair.Key}\", {keyValuePair.Value}" + "}, ");
      }
      WriteLine();

      ReadLine();
    }
    //------------------------------------------------------------------------------------------------------------------------------
    public static int CountUniqueUrls(string[] urls)
    {      
      int retVal = NormalizedUrls(urls).Item1.Count;

      return retVal;
    }
    //------------------------------------------------------------------------------------------------------------------------------
    public static (HashSet<string>, List<string>) NormalizedUrls(string[] urls)
    {
      List<string> directoryIndexes = new() { "/index.", "/default." };

      HashSet<string> baseUrls = new();
      List<string> baseUrlsList = new();

      for (int a = 0; a < urls.Count(); a++)
      {
        var nativeUrl = "";
        var lowerCase = urls[a].ToLower();
        var checkLastDiv = lowerCase[lowerCase.Length - 1];
        if (checkLastDiv != '/')
          lowerCase += "/";
        var posIndex = lowerCase.IndexOf(directoryIndexes[0]);
        var posDefault = lowerCase.IndexOf(directoryIndexes[1]);
        if (posIndex != -1)
          nativeUrl = lowerCase.Substring(0, posIndex - 1);
        else if (posDefault != -1)
          nativeUrl = lowerCase.Substring(0, posDefault - 1);
        else
          nativeUrl = lowerCase;

        var noDot = lowerCase.Replace("./", "");
        var no2Dot = noDot.Replace("./", "");
        var descentPos = no2Dot.IndexOf("#");
        var questionMarkPos = no2Dot.IndexOf("?");

        if (descentPos != -1)
          nativeUrl = noDot.Substring(0, descentPos - 1);
        else if (questionMarkPos != -1)
          nativeUrl = noDot.Substring(0, questionMarkPos - 1);
        else
          nativeUrl = no2Dot;

        baseUrls.Add(nativeUrl);
        baseUrlsList.Add(nativeUrl);
        //var locTwoDot = lowerCase.IndexOf(':');
        //var withoutHTTP = lowerCase.Substring(locTwoDot + 2, lowerCase.Length - locTwoDot - 2);
      }

      return (baseUrls, baseUrlsList);
    }
    //------------------------------------------------------------------------------------------------------------------------------
    public static Dictionary<string, int> CountUniqueUrlsPerTopLevelDomain(string[] urls)
    {
      List<string> baseurls = NormalizedUrls(urls).Item2;
      Dictionary<string, int> result = new Dictionary<string, int>();

      for(int i = 0; i < baseurls.Count; i++)
      {
        var posHeadAddress = baseurls.ElementAt(i).IndexOf(':');
        var withoutHeaderUrl = baseurls.ElementAt(i).Substring(posHeadAddress + 3);
        var posEndOfBaseUrl = withoutHeaderUrl.IndexOf("/");        
        var baseUrl = withoutHeaderUrl.Substring(0, posEndOfBaseUrl);
        var posDotEnd = baseUrl.LastIndexOf('.');
        var posDotStart = baseUrl.IndexOf('.');
        var basestURL = "";
        if(posDotEnd != posDotStart)
          basestURL = baseUrl.Substring(posDotStart + 1, posDotEnd - posDotStart - 1);
        else
          basestURL = baseUrl.Substring(0, posDotEnd);
        int getVal = 0;
        if(!result.TryGetValue(basestURL, out getVal))
          result.Add(basestURL, ++getVal);
        else
          result[basestURL]++;        
      }

      return result;
    }
  }
}
