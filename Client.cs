﻿using Autogenerated = Dapr.Client.Autogen.Grpc.v1.Dapr;
using Grpc.Net.Client;
using System.Runtime.Serialization; 
using Dapr.NET.Components; 

namespace Dapr.NET
{
    public class Client
    {
        private Autogenerated.DaprClient client; 

        public Client(string hostname, bool https, string apiToken) { 

            if (!https)
            {
                // Set correct switch to maksecure gRPC service calls. This switch must be set before creating the GrpcChannel.
                AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            }

            var uri = String.Format("http{0}://{1}", https ? "s" : "", hostname);
            Uri gRPCEndpoint = new Uri(uri);
            GrpcChannelOptions channelOptions = new GrpcChannelOptions();

            var channel = GrpcChannel.ForAddress(gRPCEndpoint, channelOptions);
            client = new Autogenerated.DaprClient(channel);
        }

        public StateStore<T> stateStore<T>(string storeName) {
           return new StateStore<T>(storeName, client);
        }
        
    }
        /// <summary>
    /// The base type of exceptions thrown by the Dapr .NET SDK.
    /// </summary>
    [Serializable]
    public class DaprException : Exception
    {
        /// <summary>
        /// Initializes a new instance of <see cref="DaprException" /> with the provided <paramref name="message" />.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public DaprException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="DaprException" /> with the provided 
        /// <paramref name="message" /> and <paramref name="innerException" />.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The inner exception.</param>
        public DaprException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DaprException"/> class with a specified context.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo" /> object that contains serialized object data of the exception being thrown.</param>
        /// <param name="context">The <see cref="StreamingContext" /> object that contains contextual information about the source or destination. The context parameter is reserved for future use and can be null.</param>
        protected DaprException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
} 