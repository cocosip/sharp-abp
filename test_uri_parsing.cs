using System;

class Program
{
    static void Main()
    {
        TestUri("topic:my-topic");
        TestUri("queue:my-queue");
        TestUri("kafka://localhost:9092/my-topic");
        TestUri("exchange:my-exchange");
    }

    static void TestUri(string uriString)
    {
        var uri = new Uri(uriString);
        Console.WriteLine($"\nURI: {uriString}");
        Console.WriteLine($"  Scheme: {uri.Scheme}");
        Console.WriteLine($"  Host: {uri.Host}");
        Console.WriteLine($"  AbsolutePath: '{uri.AbsolutePath}'");

        var topicName = uri.Scheme == "topic" ? uri.Host : uri.AbsolutePath.TrimStart('/');
        Console.WriteLine($"  Extracted topicName: '{topicName}'");
    }
}
