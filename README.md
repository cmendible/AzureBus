EasyAzureServiceBus
===================

This library, inspired by the simple [EasyNetQ](http://easynetq.com/ "EasyNetQ") API, helps you get started with [Service Bus 1.1 for Windows Server](http://msdn.microsoft.com/en-us/library/windowsazure/dn282144.aspx) pub/sub architecture!  

<pre><code>using EasyAzureServiceBus;

// Create a bus instance
IBus bus = AzureCloud.CreateBus();

// Publish a sample message.
bus.Publish(new SampleMessage("message value"));

// Sample consumer class
public class Consumer : IConsume<SampleMessage>
{
    public void Consume(SampleMessage message)
    {
        Console.WriteLine(string.Format("Message received: Value = {0}", message.Value));
    }
}

// Subscribe to messages using consumers in an assembly.
var autoSubscriber = new AutoSubscriber(bus, "subscriptionId");
autoSubscriber.Subscribe(Assembly.GetExecutingAssembly());
</code></pre>

##EasyAzureServiceBus is available as nuget package.

<pre><code>To install EasyAzureServiceBus, run the following command in the Package Manager Console

PM> Install-Package EasyAzureServiceBus</code></pre>
