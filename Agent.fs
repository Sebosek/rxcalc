module RxCalc.Agent

open System
open System.Reactive
open System.Reactive.Subjects

open RxCalc.Infrastructure
open RxCalc.Model

let compute (operation : Envelope<Sum>) = 
    let a = operation.content.operandA
    let b = operation.content.operandB
    let result = {
        operandA = a
        operandB = b
        result = a + b }
    envelope operation.id result

type Agent<'T> = Microsoft.FSharp.Control.MailboxProcessor<'T>
type Agent (observable : IObservable<Envelope<Sum>>) =
    let subject = new Subject<Envelope<SumResult>>()
    let mutable subscription : IDisposable = null
    let agent = Agent<Envelope<Sum>>.Start(fun inbox ->
        let rec loop () = 
            async {
                let! message = inbox.Receive()
                let x = String.Format("Agent read {0}", message)
                Console.WriteLine(x)
                let result = compute message

                result |> subject.OnNext
                return! loop()
            }
        loop())
    do
        subscription <- agent.Post |> observable.Subscribe

    interface IObservable<Envelope<SumResult>> with
        member this.Subscribe observer = subject.Subscribe observer

    interface IDisposable with
        member this.Dispose () =
            subject.Dispose()
            subscription.Dispose()