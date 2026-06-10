using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseScheduleService.Application.Interfaces.EventBus
{
    public interface IEventBus
    {
        Task PublishAsync<T>(string topic, T @event) where T : class;
    }
}