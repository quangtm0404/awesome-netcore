# Awesome NetCore
- RateLimit
- HealthCheck
- Cache
- EFCore


# RateLimiting
RateLimiting là kỹ thuật lock một resource lại để hạn chế spamming, DDOS
## TokenBucket Limit
- Có một bucket, khi có `req` token sẽ được lấy ra khỏi bucket, tới khi bucket k còn token thì sẽ `lock` đến khi token được cung cấp lại
## Fixed window limit
- Một window là một timespacing, req sẽ được cấp lại resource khi qua window mới
## Sliding Window limit 

## RateLimiter class
- RateLimiter is a base class to implement Rate Limit in .NET 7
- `Acquire` and `WaitAsync()` is a method trying to gain permit (giay phep) for a resource
- `Acquire` will check if the permitCount is enough or not and return a `RateLimitLease` which determine you are successfully access to the resource or not
- `WaitAsync()` is the same feature with `Acquire` but it provide the Queing method 
- `RateLimitLease` has a property `IsAcquired` to see if it is permit or not 

## PartitionedRateLimiter
- Abstraction class like RateLimiter but accept TResource