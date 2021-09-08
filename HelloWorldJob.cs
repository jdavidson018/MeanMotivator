using MeanMotivator.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

[DisallowConcurrentExecution]
public class HelloWorldJob : IJob
{
    private readonly ILogger<HelloWorldJob> _logger;
    private readonly ICommentsRepository _repository;
    private readonly string _authToken;
    public HelloWorldJob(ICommentsRepository repository, ILogger<HelloWorldJob> logger, IConfiguration configuration)
    {
        _logger = logger;
        _repository = repository;
        _authToken = configuration["TwilioAuthToken"];
    }

    public Task Execute(IJobExecutionContext context)
    {
        List<PhoneNumber> numbers = new List<PhoneNumber>();
        numbers.Add(new PhoneNumber("+14026161179"));

        var text = _repository.GetRandomCommentAsync().Result.Text;
        _logger.LogInformation("Sending a text message");
        var accountSid = "AC6e591a6c2f1d06a9076859b6685cd29d"; 
        TwilioClient.Init(accountSid, _authToken); 

        foreach (var number in numbers)
        {
            var messageOptions = new CreateMessageOptions( 
                number);  
            messageOptions.MessagingServiceSid = "MGf263a7c52095372b619bbb51ca96d8b0";  
            messageOptions.Body = text;
            var message = MessageResource.Create(messageOptions); 
        }
        _logger.LogInformation("Finished Sending Messages");
        return Task.CompletedTask;
    }
}