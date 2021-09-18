using System;
using System.Net.Http;

namespace EventBusClient.Dummy
{
    // interface
    public interface IEventBusClient
    {
        EventBusResponse DoWork(string inputStr);
        EventBusResponse DoWorkWithCustomiseException();
        EventBusResponse DoWorkWithHttpRequestException();
    }

    // decorator

    public class PaymentsClient : IEventBusClient
    {


        public EventBusResponse DoWork(string inputStr)
        {
            Console.Error.WriteLine($"I'm doing the work. msg: {inputStr}");
            return new PaymentsEventBusResponse();
        }

        public EventBusResponse DoWorkWithCustomiseException()
        {
            throw new PaymentsEventBusException();
        }

        public EventBusResponse DoWorkWithHttpRequestException()
        {
            throw new HttpRequestException();
        }
    }


    // Models
    public abstract class EventBusResponse
    {
        public string ReturnStr;
    }

    public class PaymentsEventBusResponse : EventBusResponse
    {
       public new string ReturnStr = "Message about Payments EventBus Client.";
    }

    public class PaymentsEventBusException : Exception
    {
        public override string Message => "This is Payments Event Bus Customised Exception.";
    }
}