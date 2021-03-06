﻿using System.Net.Http;

namespace RequestSimulation.Executing.Interceptors
{
    public interface IHttpRequestMessageInterceptor
    {
        void InterceptAsync(HttpRequestMessage message);
    }
}