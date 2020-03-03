using Stateless;
using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace RxStateMachine
{
    class Program
    {
        static void Main(string[] args)
        {
            RxStateMachine stateMachine = new RxStateMachine();

            stateMachine.Enqueue(Trigger.X);
            stateMachine.Enqueue(Trigger.Y);

            Console.WriteLine("All triggers queued " + Environment.NewLine);
            Console.ReadLine();
        }
    }
    public enum Trigger { X, Y }

    public enum State { A, B, C }

    public class RxStateMachine
    {
        private readonly StateMachine<State, Trigger> _stateMachine;
        private readonly Subject<Trigger> subject = new Subject<Trigger>();

        public RxStateMachine()
        {
            _stateMachine = new StateMachine<State, Trigger>(State.A);

            _stateMachine.Configure(State.A)
                .Permit(Trigger.X, State.B)
                .OnExit(() => Console.WriteLine("Exiting State A"));

            _stateMachine.Configure(State.B)
                .OnEntry(() =>
                  {
                      Console.WriteLine("Entering state B");
                      Console.WriteLine("Starting long-running 'task'");
                      var n = Fib(40);
                      Console.WriteLine("Done: " + n);
                  })
                .Permit(Trigger.Y, State.C)
                .OnExit(() => Console.WriteLine("Exiting State B"));

            _stateMachine.Configure(State.C)
                .OnEntry(() => Console.WriteLine("Entering state C"));

            subject.ObserveOn(TaskPoolScheduler.Default)
                 .Subscribe(HandleTrigger);
        }
        public void Enqueue(Trigger trigger)
        {
            Console.WriteLine("Queueing trigger " + trigger);
            subject.OnNext(trigger);
        }

        private void HandleTrigger(Trigger trigger)
        {
            Console.WriteLine("Handling trigger " + trigger);
            _stateMachine.Fire(trigger);
            Console.WriteLine("Handled trigger " + trigger + Environment.NewLine);
        }



        private int Fib(int n)
        {
            return GetNthFibonacci_Rec(n);
        }

        private static int GetNthFibonacci_Rec(int n)
        {
            if ((n == 0) || (n == 1))
            {
                return n;
            }
            else
                return GetNthFibonacci_Rec(n - 1) + GetNthFibonacci_Rec(n - 2);
        }

    }
}
