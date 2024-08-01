# Awesome NetCore
# HealthCheck
- Là kỹ thuật check được service đang healthy để tiếp nhận request
- Built in HealthCheck
```csharp
builder.Services.AddHealthCheck();

var app = builder.Build();
builder.MapHealthChecks("/healthz");
```
# Cache
- Có 2 loại lib cache hỗ trợ chính cho .NET (IMemoryCache và IDistributedCache)
- MemoryCache: Cache built in của application, nhanh hơn so với Distributed, nhưng k khả thi khi scale service ra nhiều node
- Distributed Cache: Cache được lưu value ở một nơi khác, khi đó khi truy xuất sẽ luôn available khi truy xuất ở bất kì node nào (thường dùng Redis) - tuy nhiên các Db khác đều có thể hỗ trợ để làm cache
# EFCore



# RateLimiting
RateLimiting là kỹ thuật lock một resource lại để hạn chế spamming, DDOS
## TokenBucket Limit
- Có một bucket, khi có `req` token sẽ được lấy ra khỏi bucket, tới khi bucket k còn token thì sẽ `lock` đến khi token được cung cấp lại
## Fixed window limit
- Một window là một timespacing, req sẽ được cấp lại resource khi qua window mới
## Sliding Window limit 
- Cũng giống như window, tuy nhiên một window sẽ được chia thành nhiều segment
## Concurrency Limit
- Lock lại resource hạn chế chỉ được n request đồng thời
## RateLimiter class
- RateLimiter is a base class to implement Rate Limit in .NET 7
- `Acquire` and `WaitAsync()` is a method trying to gain permit (giay phep) for a resource
- `Acquire` will check if the permitCount is enough or not and return a `RateLimitLease` which determine you are successfully access to the resource or not
- `WaitAsync()` is the same feature with `Acquire` but it provide the Queing method 
- `RateLimitLease` has a property `IsAcquired` to see if it is permit or not 

## PartitionedRateLimiter
- Abstraction class like RateLimiter but accept TResource, TKey để lock resource theo key cố định