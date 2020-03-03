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

            stateMachine.Fire(Trigger.X);
            stateMachine.Fire(Trigger.Y);
            stateMachine.Fire(Trigger.X);

            Console.WriteLine("All triggers queued " + Environment.NewLine);
            Console.ReadLine();
        }
    }
    public enum Trigger { X, Y }

    public enum State { A, B }

    public class RxStateMachine
    {
        private readonly StateMachine<State, Trigger> _stateMachine;
        private readonly Subject<Trigger> subject = new Subject<Trigger>();

        public RxStateMachine()
        {
            _stateMachine = new StateMachine<State, Trigger>(State.A);

            _stateMachine.Configure(State.A)
                .OnEntry(() =>
                  {
                      Console.WriteLine("State is now A");
                      Console.WriteLine("Starting long-running 'task'");
                      var n = Fib(40);
                      Console.WriteLine("Done: " + n);
                  })
                .Permit(Trigger.X, State.B)
                .OnExit(() => Console.WriteLine("Exiting State A"));

            _stateMachine.Configure(State.B)
                .OnEntry(() => Console.WriteLine("State is now B"))
                .Permit(Trigger.Y, State.A)
                .OnExit(() => Console.WriteLine("Exiting State B"));

            subject.ObserveOn(TaskPoolScheduler.Default)
                 .Subscribe(HandleTrigger);
        }

        private void HandleTrigger(Trigger trigger)
        {
            Console.WriteLine("Handling trigger " + trigger);
            _stateMachine.Fire(trigger);
            Console.WriteLine("Handled trigger " + trigger + Environment.NewLine);
        }

        public void Fire(Trigger trigger)
        {
            Console.WriteLine("Queueing trigger " + trigger);
            subject.OnNext(trigger);
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
