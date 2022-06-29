using _ = Dapr.Client.Autogen.Grpc.v1;
using Grpc.Core; 
using Dapr.NET.Utils;

namespace Dapr.NET.Components {
    public struct StoreObject<T> {
        public T Data { get; set; }
        public int Etag { get; set; }
    }

    public class StateStore<T> {
        private _.Dapr.DaprClient client; 
        private string storeName;

        
        public StateStore(string _storeName, _.Dapr.DaprClient _client) { 
            this.client = _client; 
            this.storeName = _storeName;
        }

        public async Task<bool> Store(string key, T thing, Dictionary<string, string> metadata) { 
            var envelope = new _.SaveStateRequest() {
                StoreName = storeName, 
            };

            envelope.States.Add(new _.StateItem() {
                    Key = key, 
                    Value = TypeConverters.ToJsonByteString<T>(thing),
            });

            CancellationToken cancellationToken = new CancellationToken();
            var options = new CallOptions(headers: new Metadata(), cancellationToken: cancellationToken);

            try
            {
               await client.SaveStateAsync(envelope, options);
               return true;
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.Aborted) {
                return false;
            } 
            catch (RpcException ex)
            {
                throw new DaprException("State operation failed: the Dapr endpoint indicated a failure. See InnerException for details.", ex);
            } 
        }

        public async Task<StoreObject<T>> Get(string key, Dictionary<string, string> metadata) {
           var envelope = new _.GetStateRequest()
            {
                StoreName = storeName,
                Key = key,
            };

            if (metadata != null)
            {
                foreach (var kvp in metadata)
                {
                    envelope.Metadata.Add(kvp.Key, kvp.Value);
                }
            }
           
            CancellationToken cancellationToken = new CancellationToken();
            var options = new CallOptions(headers: new Metadata(), cancellationToken: cancellationToken);

            try
            {
                _.GetStateResponse response = await client.GetStateAsync(envelope, options);
                T obj = TypeConverters.FromJsonByteString<T>(response.Data);
                return new StoreObject<T> { Data = obj, Etag = 10 };
            }
            catch (RpcException ex)
            {
                throw new DaprException("State operation failed: the Dapr endpoint indicated a failure. See InnerException for details.", ex);
            } 
        }

    }
}
