###### async local

AsyncLocal<T> solves this by attaching the data to the ExecutionContext rather than the physical thread. As your async code flows across thread hops, .NET automatically flows the AsyncLocal values right along with it.