# ThreadSafeStateless
The Stateless library is not thread safe. Code is usually multi-threaded. This repository demonstrates a minimal example of how to ensure thread safety when using stateless.

## Piping state changes through Reactive extensions
By using an observable we can execute the triggers sequentially. The triggers are queued up in RX for execution.
Rx takes care of emptying the "queue".
Each trigger is comitted to the queue, and the code returns immediately.
State is not mutated directly, but from a single point of execution.
