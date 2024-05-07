

namespace IssueTracker.ContractTests;

public static class Constants
{
    public static string GetLocationForPathRegex(string resource) =>
        $"http://localhost/{resource}/(?:\\{{{{0,1}}(?:[0-9a-fA-F]){{8}}-(?:[0-9a-fA-F]){{4}}-(?:[0-9a-fA-F]){{4}}-(?:[0-9a-fA-F]){{4}}-(?:[0-9a-fA-F]){{12}}\\}}{{0,1}})";

}

