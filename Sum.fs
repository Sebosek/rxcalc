module RxCalc.Sum

open System
open System.Reactive.Subjects

open Microsoft.AspNetCore.Http

open Giraffe.HttpContextExtensions
open Giraffe.HttpHandlers
open Giraffe.Tasks

open RxCalc.Infrastructure
open RxCalc.Model

type ISumProcessor =
    abstract member Compute : Envelope<Sum> -> unit

type SumProcessor () =
    let subject = new Subject<Envelope<Sum>>()
    
    interface ISumProcessor with
        member this.Compute (envelope : Envelope<Sum>) =
            subject.OnNext envelope

    interface IObservable<Envelope<Sum>> with
        member this.Subscribe observer = subject.Subscribe observer

    interface IDisposable with
        member this.Dispose () =
            subject.Dispose()

let created id =
    fun (next : HttpFunc) (ctx : HttpContext) ->
        let url = sprintf "http://localhost:6600/%s" id
        (setStatusCode 201 >=> json {id = id; url = url} >=> setHttpHeader "x-resource-url" url) next ctx

let guidToString (guid : Guid) =
    guid.ToString()

let controller =
    fun (next : HttpFunc) (ctx : HttpContext) ->
        task {
            let! operands = ctx.BindJson<Sum>()
            let publisher = ctx.GetService<ISumProcessor>()
            let envelope = envelopeWithDefaults operands
            publisher.Compute envelope

            return! created (envelope.id |> guidToString) next ctx
        }