﻿namespace eShop.Services.AuthAPI.RabbitMQSender
{
    public interface IRabbitMQAuthMessageSender
    {
        void SendMessage(object message, string queueName);
    }
}
