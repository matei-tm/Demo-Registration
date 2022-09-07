﻿namespace Registration.Components.Activities.EventRegistration
{
    using System;
    using System.Threading.Tasks;
    using MassTransit;
    using Microsoft.Extensions.Logging;


    public class EventRegistrationActivity :
        IActivity<EventRegistrationArguments, EventRegistrationLog>
    {
        readonly ILogger<EventRegistrationActivity> _logger;

        public EventRegistrationActivity(ILogger<EventRegistrationActivity> logger)
        {
            _logger = logger;
        }

        public async Task<ExecutionResult> Execute(ExecuteContext<EventRegistrationArguments> context)
        {
            _logger.LogInformation("Registering for event: {0} ({1})", context.Arguments.EventId, context.Arguments.ParticipantEmailAddress);

            const decimal registrationTotal = 25.00m;

            var registrationId = NewId.NextGuid();

            _logger.LogInformation("Registered for event: {0} ({1})", registrationId, context.Arguments.ParticipantEmailAddress);

            return context.CompletedWithVariables(new Log(registrationId, context.Arguments.ParticipantEmailAddress), new
            {
                Amount = registrationTotal
            });
        }

        public async Task<CompensationResult> Compensate(CompensateContext<EventRegistrationLog> context)
        {
            _logger.LogInformation("Removing registration for event: {0} ({1})", context.Log.RegistrationId, context.Log.ParticipantEmailAddress);

            return context.Compensated();
        }


        class Log :
            EventRegistrationLog
        {
            public Log(Guid registrationId, string participantEmailAddress)
            {
                RegistrationId = registrationId;
                ParticipantEmailAddress = participantEmailAddress;
            }

            public Guid RegistrationId { get; }

            public string ParticipantEmailAddress { get; }
        }
    }
}